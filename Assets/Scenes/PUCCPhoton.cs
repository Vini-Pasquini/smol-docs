using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PUCCPhoton : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        Debug.Log("[PUCCPhoton] Connecting to Server...");
        PhotonNetwork.GameVersion = Application.version;
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
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PUCCPhoton] Player {newPlayer.NickName} Joined Room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PUCCPhoton] Player {otherPlayer.NickName} Left Room");
    }
}
