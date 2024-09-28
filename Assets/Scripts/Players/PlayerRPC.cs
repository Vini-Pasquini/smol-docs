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

    /* Calls */
    public void RPCSelectDoctor(DoctorType doctorType)
    {
        roomManager.MyPhotonView.RPC(nameof(OtherDoctorSelected), RpcTarget.Others, doctorType);
    }

    public void RPCSetReady(bool readyState)
    {
        roomManager.MyPhotonView.RPC(nameof(OtherPlayerSetReady), RpcTarget.Others, readyState);
    }

    public void RPCSendMessage(string outMessage)
    {
        roomManager.MyPhotonView.RPC(nameof(RecieveMessage), RpcTarget.Others, outMessage);
    }

    public void RPCKillEnemy(Vector3 killPosition, EnemyType enemyType)
    {
        roomManager.MyPhotonView.RPC(nameof(EnemyKilled), RpcTarget.Others, killPosition, (int)enemyType);
    }

    /* Callbacks */
    [PunRPC]
    public void RecieveMessage(string inMessage)
    {
        roomUIController.OnRecieveMessageCallback(inMessage);
    }

    [PunRPC]
    public void OtherDoctorSelected(DoctorType doctorType)
    {
        roomManager.UpdateDoctorType(doctorType, true);
    }

    [PunRPC]
    public void OtherPlayerSetReady(bool readyState)
    {
        roomManager.UpdatePlayerReady(readyState, true);
    }

    [PunRPC]
    public void EnemyKilled(Vector3 killPosition, int enemyType)
    {
        roomManager.SpawnEnemyPile(killPosition, (EnemyType)enemyType);
    }

    /* ph RPC */
    [PunRPC]
    public void ReloadAmmo(float ammoAmount)
    {
        roomManager.MyDoctor.AddAmmo(ammoAmount);
    }
}
