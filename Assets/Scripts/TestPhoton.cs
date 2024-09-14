using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestPhoton : MonoBehaviourPunCallbacks
{
    public static TestPhoton Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.NickName = System.Environment.MachineName;

        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Connecting to Server... </color>");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Successfully Connected to Server </color>");
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Connecting to Lobby... </color>");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Successfully Disconnected from Server </color>");
    }

    public override void OnJoinedLobby()
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Successfully Connected to Lobby </color>");
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Creating and/or Joining \"DEV_TEST\" Room... </color>");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("DEV_TEST", roomOptions, TypedLobby.Default);
    }

    public override void OnLeftLobby()
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Successfully Disconnected from Lobby </color>");
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Disconnecting from Server... </color>");
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Successfully Created Room </color>");
    }

    public override void OnJoinedRoom()
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Successfully Joined Room </color>");

        // RoomManager.Instance.DEV_TEST();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomUIController.Instance.WriteToChat($"<color=#a0ffa0> Player {newPlayer.NickName} Joined Room </color>");
    }
}
