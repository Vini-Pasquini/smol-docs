using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPC : MonoBehaviour
{
    /* Calls */
    public void RPCSelectDoctor(DoctorType doctorType)
    {
        RoomManager.Instance.myPhotonView.RPC(nameof(DoctorSelected), RpcTarget.Others, doctorType);
    }

    public void RPCSendMessage(string outMessage)
    {
        RoomManager.Instance.myPhotonView.RPC(nameof(RecieveMessage), RpcTarget.Others, outMessage);
    }

    /* Callbacks */
    [PunRPC] public void DoctorSelected(DoctorType doctorType)
    {
        RoomUIController.Instance.OnDoctorSelectedCallback(doctorType);
    }

    [PunRPC] public void RecieveMessage(string inMessage)
    {
        RoomUIController.Instance.OnRecieveMessageCallback(inMessage);
    }
}