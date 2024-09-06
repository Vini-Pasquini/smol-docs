using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PhotonView myPhotonView;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
    }

    public void SendChatMessage(string outMessage)
    {
        myPhotonView.RPC(nameof(ReceiveChat), RpcTarget.All, outMessage);
    }

    [PunRPC] public void ReceiveChat(string inMessage, PhotonMessageInfo info)
    {
        Debug.Log($"nova mensagem: {inMessage}");
        PUCCPhoton.Instance.UpdateMessageContainer(inMessage, info);
    }
}
