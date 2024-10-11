using Photon.Pun;
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

    [PunRPC] public void AmmoReloadAnswer(int morphine, int vaccine)
    {
        if (roomManager.MyDoctorType == DoctorType.CombatDoctor)
        {
            roomManager.MyDoctor.AddAmmo(morphine, vaccine);
        }
    }

    public void RPCDecreaseDoctorSize(int doses)
    {

        roomManager.MyPhotonView.RPC(nameof(DecreaseDoctorSize), RpcTarget.All, doses);
    }

    [PunRPC] public void DecreaseDoctorSize(int doses)
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

    [PunRPC] public void RequestMapSeed()
    {
        roomManager.MyPhotonView.RPC(nameof(RequestMapSeedCallback), RpcTarget.Others, roomManager.RequestMapSeed());
    }

    [PunRPC] public void RequestMapSeedCallback(int seed)
    {
        roomManager.SetMapSeed(seed);
    }

    // combat

    public void RPCDeployCavalo(Vector3 position)
    {
        roomManager.MyPhotonView.RPC(nameof(DeployCavalo), RpcTarget.All, position);
    }

    [PunRPC] public void DeployCavalo(Vector3 position)
    {
        roomManager.SetDeployedCavalo(GameObject.Instantiate(Resources.Load<GameObject>("Cavalo"), position, Quaternion.identity));
    }

    public void RPCPickupCavalo()
    {
        roomManager.MyPhotonView.RPC(nameof(PickupCavalo), RpcTarget.All);
    }

    [PunRPC] public void PickupCavalo()
    {
        roomManager.ResetDeployedCavalo();
    }

    // gathering

    // cavalo

    public void RPCFillCavaloResources(int morphineAmount, int vaccineAmount)
    {
        roomManager.MyPhotonView.RPC(nameof(FillCavaloResources), RpcTarget.Others, morphineAmount, vaccineAmount);
    }

    [PunRPC] public void FillCavaloResources(int morphineAmount, int vaccineAmount)
    {
        roomManager.DeployedCavalo.GetComponent<CavaloController>().FillResources(morphineAmount, vaccineAmount);
    }

    public void RPCEmptyCavaloResources()
    {
        roomManager.MyPhotonView.RPC(nameof(EmptyCavaloResources), RpcTarget.All);
    }

    [PunRPC] public void EmptyCavaloResources()
    {
        roomManager.DeployedCavalo.GetComponent<CavaloController>().EmptyResources();
    }
}
