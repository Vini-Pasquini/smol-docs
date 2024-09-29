using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPC : MonoBehaviour // TODO: me livrar do mono (se der)
{
    private RoomManager roomManager;
    private RoomUIController roomUIController;

    private void Awake()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
    }

    public void RPCSelectDoctor(DoctorType doctorType)
    {
        roomManager.MyPhotonView.RPC(nameof(OtherDoctorSelected), RpcTarget.Others, doctorType);
    }

    [PunRPC] public void OtherDoctorSelected(DoctorType doctorType)
    {
        roomManager.UpdateDoctorType(doctorType, true);
    }

    public void RPCSetReady(bool readyState)
    {
        roomManager.MyPhotonView.RPC(nameof(OtherPlayerSetReady), RpcTarget.Others, readyState);
    }

    [PunRPC] public void OtherPlayerSetReady(bool readyState)
    {
        roomManager.UpdatePlayerReady(readyState, true);
    }

    public void RPCSendMessage(string outMessage)
    {
        roomManager.MyPhotonView.RPC(nameof(RecieveMessage), RpcTarget.Others, outMessage);
    }

    [PunRPC] public void RecieveMessage(string inMessage)
    {
        roomUIController.OnRecieveMessageCallback(inMessage);
    }

    public void RPCKillEnemy(Vector3 killPosition, EnemyType enemyType)
    {
        roomManager.MyPhotonView.RPC(nameof(EnemyKilled), RpcTarget.Others, killPosition, (int)enemyType);
    }

    [PunRPC] public void EnemyKilled(Vector3 killPosition, int enemyType)
    {
        roomManager.SpawnEnemyPile(killPosition, (EnemyType)enemyType);
    }

    /* Solo RPC */
    
    [PunRPC] public void SendAmmoReloadRequest()
    {
        if (roomManager.MyDoctorType == DoctorType.GatheringDoctor)
        {
            roomManager.MyPhotonView.RPC(nameof(AmmoReloadAnswer), RpcTarget.Others, roomManager.MyDoctor.LeukocyteAmount, roomManager.MyDoctor.PathogenAmount);
            roomManager.MyDoctor.ResetResources();
        }
    }

    [PunRPC] public void AmmoReloadAnswer(float morphine, float vaccine)
    {
        if (roomManager.MyDoctorType == DoctorType.CombatDoctor)
        {
            roomManager.MyDoctor.AddAmmo(morphine, vaccine);
        }
    }

    [PunRPC] public void ResetDoctorSize()
    {
        roomManager.ResetPlayerSize();
    }

    [PunRPC] public void OtherPlayerBackToLobby()
    {
        if (!roomUIController.GameOverCanvas.activeSelf) { roomUIController.UpdateWaitingForPlayersOverlay(); }
        roomManager.MyPhotonView.RPC(nameof(OtherPlayerBackToLobbyCallback), RpcTarget.Others, roomUIController.GameOverCanvas.activeSelf);
    }

    [PunRPC] public void OtherPlayerBackToLobbyCallback(bool isOnLobby)
    {
        roomUIController.UpdateWaitingForPlayersOverlay(isOnLobby);
    }
}
