using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PUCCPhoton : MonoBehaviourPunCallbacks
{
    public static PUCCPhoton Instance;

    private PlayerController myself;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("[PUCCPhoton] Connecting to Server...");
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.NickName = "JEZUIZ";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PUCCPhoton] Successfully Connected to Server");
        Debug.Log("[PUCCPhoton] Connecting to Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[PUCCPhoton] Successfully Connected to Lobby");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        Debug.Log("[PUCCPhoton] Joinning or Creating Room...");
        PhotonNetwork.JoinOrCreateRoom("PUCCRoom", roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[PUCCPhoton] Client Successfully Joined Room");
        myself = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        UIController.Instance.UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PUCCPhoton] Player {newPlayer.NickName} Joined Room");
        UIController.Instance.UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PUCCPhoton] Player {otherPlayer.NickName} Left Room");
        UIController.Instance.UpdatePlayerList();
    }

    public void OnSendButtonPress()
    {
        myself.myPhotonView.RPC("ReceiveChat", RpcTarget.All, UIController.Instance.inputMessage.text);
        UIController.Instance.inputMessage.text = "";
    }

    public void UpdateMessageContainer(string newMessage, PhotonMessageInfo info)
    {
        UIController.Instance.messagesContainer.text += newMessage + "\n";
    }
}
