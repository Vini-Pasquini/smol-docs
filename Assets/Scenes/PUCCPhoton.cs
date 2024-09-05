using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PUCCPhoton : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI userList;

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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        string log = "";
        foreach (RoomInfo room in roomList)
        {
            log += $"ROOM: {room.Name}\n";
        }
        Debug.Log($"[PUCCPhoton] {log}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[PUCCPhoton] Client Successfully Joined Room");
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PUCCPhoton] Player {newPlayer.NickName} Joined Room");
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PUCCPhoton] Player {otherPlayer.NickName} Left Room");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        string list = "Player List:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            list += $"{player.NickName}\n";
        }
        userList.text = list;
    }
}
