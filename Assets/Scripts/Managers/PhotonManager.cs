using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private MainMenuUIController mainMenuUIController;
    private RoomUIController roomUIController;

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);

        mainMenuUIController = GameObject.Find("MainMenuUIController").GetComponent<MainMenuUIController>();
    }

    public void InitRoomUIController()
    {
        roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.NickName = System.Environment.MachineName;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PhotonManager] Successfully Connected to Server");
        Debug.Log("[PhotonManager] Connecting to Lobby...");
        mainMenuUIController.SetStatusMassege("Successfully Connected to Server - Connecting to Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("[PhotonManager] Successfully Disconnected from Server");
        mainMenuUIController.SetStatusMassege("Successfully Disconnected from Server");
        mainMenuUIController.BackButtonCallback();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[PhotonManager] Successfully Connected to Lobby");
        mainMenuUIController.SetStatusMassege("Successfully Connected to Lobby");
        mainMenuUIController.PlayButtonCallback();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("[PhotonManager] Successfully Disconnected from Lobby");
        Debug.Log("[PhotonManager] Disconnecting from Server...");
        mainMenuUIController.SetStatusMassege("Successfully Disconnected from Lobby - Disconnecting from Server...");
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("[PhotonManager] Successfully Created Room");
        mainMenuUIController.SetStatusMassege("Successfully Created Room");
        mainMenuUIController.CreateRoomButtonCallback();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[PhotonManager] Successfully Joined Room");
        mainMenuUIController.SetStatusMassege("Successfully Joined Room");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.Log($"[PhotonManager] Player {otherPlayer.NickName} Joined Room");
        roomUIController.WriteToChat($"<color=#a0ffa0>Player {otherPlayer.NickName} Joined </color>");
        roomUIController.UpdateWaitingForPlayersOverlay();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PhotonManager] Player {otherPlayer.NickName} Joined Room");
        roomUIController.WriteToChat($"<color=#ffa0a0>Player {otherPlayer.NickName} Left </color>");
        roomUIController.UpdateWaitingForPlayersOverlay();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        mainMenuUIController.RoomListUpdate(roomList);
    }
}
