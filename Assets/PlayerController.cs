using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PhotonView myPhotonView;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
    }

    public void SendChatMessage(string message)
    {
        myPhotonView.RPC(nameof(ReceiveChat), RpcTarget.All, message);
    }

    [PunRPC] public void ReceiveChat(string message, PhotonMessageInfo info)
    {
        Debug.Log($"nova mensagem: {message}");
    }
}
