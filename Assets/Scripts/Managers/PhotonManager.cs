using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private MainMenuUIController mainMenuUIController;
    private RoomUIController roomUIController;
    private RoomManager roomManager;

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);

        mainMenuUIController = GameObject.Find("MainMenuUIController").GetComponent<MainMenuUIController>();
    }

    public void InitRoomStuff()
    {
        roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.NickName = System.Environment.MachineName;
    }

    public override void OnConnectedToMaster()
    {
#if UNITY_EDITOR
        Debug.Log("[PhotonManager] Successfully Connected to Server");
        Debug.Log("[PhotonManager] Connecting to Lobby...");
#endif
        mainMenuUIController.SetStatusMassege("Conectado com Sucesso ao Server - Conectando ao Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
#if UNITY_EDITOR
        Debug.Log("[PhotonManager] Successfully Disconnected from Server");
#endif
        mainMenuUIController.SetStatusMassege("Desconectado com Sucesso do Server");
        mainMenuUIController.BackButtonCallback();
    }

    public override void OnJoinedLobby()
    {
#if UNITY_EDITOR
        Debug.Log("[PhotonManager] Successfully Connected to Lobby");
#endif
        mainMenuUIController.SetStatusMassege("Conectado com Sucesso ao Lobby");
        mainMenuUIController.PlayButtonCallback();
    }

    public override void OnLeftLobby()
    {
#if UNITY_EDITOR
        Debug.Log("[PhotonManager] Successfully Disconnected from Lobby");
        Debug.Log("[PhotonManager] Disconnecting from Server...");
#endif
        mainMenuUIController.SetStatusMassege("Desconectado com Sucesso do Lobby - Desconectando do Server...");
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
#if UNITY_EDITOR
        Debug.Log("[PhotonManager] Successfully Created Room");
#endif
        mainMenuUIController.SetStatusMassege("Sala Criada com Sucesso");
        mainMenuUIController.CreateRoomButtonCallback();
    }

    public override void OnJoinedRoom()
    {
#if UNITY_EDITOR
        Debug.Log("[PhotonManager] Successfully Joined Room");
#endif
        mainMenuUIController.SetStatusMassege("Entrou na Sala com Sucesso");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
#if UNITY_EDITOR
        Debug.Log($"[PhotonManager] Player {otherPlayer.NickName} Joined Room");
#endif
        roomUIController.WriteToChat($"<color=#a0ffa0>Jogador {otherPlayer.NickName} Entrou na Sala </color>");
        roomUIController.UpdateWaitingForPlayersOverlay();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
#if UNITY_EDITOR
        Debug.Log($"[PhotonManager] Player {otherPlayer.NickName} Left Room");
#endif
        roomUIController.WriteToChat($"<color=#ffa0a0>Jogador {otherPlayer.NickName} Deixou a Sala </color>");
        roomUIController.UpdateWaitingForPlayersOverlay();
        roomManager.ResetGame();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        mainMenuUIController.RoomListUpdate(roomList);
    }
}
