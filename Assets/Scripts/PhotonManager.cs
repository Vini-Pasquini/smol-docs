using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;

    public GameObject PlayerPrefab { get { return this.playerPrefab; } }

    private void Awake()
    {
        Instance = this;
        GameObject.DontDestroyOnLoad(this);
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
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("[PhotonManager] Successfully Disconnected from Server");
        MainMenuController.Instance.BackButtonCallback();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[PhotonManager] Successfully Connected to Lobby");
        MainMenuController.Instance.PlayButtonCallback();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("[PhotonManager] Successfully Disconnected from Lobby");
        Debug.Log("[PhotonManager] Disconnecting from Server...");
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("[PhotonManager] Successfully Created Room");
        MainMenuController.Instance.CreateRoomButtonCallback();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[PhotonManager] Successfully Joined Room");
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PhotonManager] Player {newPlayer.NickName} Joined Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        MainMenuController.Instance.RoomListUpdate(roomList);
    }
}
