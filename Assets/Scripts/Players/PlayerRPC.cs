using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPC : MonoBehaviour
{
    /* Calls */
    public void RPCSelectDoctor(DoctorType doctorType)
    {
        RoomManager.Instance.myPhotonView.RPC(nameof(OtherDoctorSelected), RpcTarget.Others, doctorType);
    }

    public void RPCSetReady(bool readyState)
    {
        RoomManager.Instance.myPhotonView.RPC(nameof(OtherPlayerSetReady), RpcTarget.Others, readyState);
    }

    public void RPCSendMessage(string outMessage)
    {
        RoomManager.Instance.myPhotonView.RPC(nameof(RecieveMessage), RpcTarget.Others, outMessage);
    }

    /* Callbacks */
    [PunRPC] public void RecieveMessage(string inMessage)
    {
        RoomUIController.Instance.OnRecieveMessageCallback(inMessage);
    }
    
    [PunRPC] public void OtherDoctorSelected(DoctorType doctorType)
    {
        RoomManager.Instance.UpdateDoctorType(doctorType, true);
    }

    [PunRPC] public void OtherPlayerSetReady(bool readyState)
    {
        RoomManager.Instance.UpdatePlayerReady(readyState, true);
    }
}
