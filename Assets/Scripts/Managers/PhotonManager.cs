using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
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
        Debug.Log("[PhotonManager] Successfully Connected to Server");
        Debug.Log("[PhotonManager] Connecting to Lobby...");
        mainMenuUIController.SetStatusMassege("Conectado com Sucesso ao Server - Conectando ao Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("[PhotonManager] Successfully Disconnected from Server");
        mainMenuUIController.SetStatusMassege("Desconectado com Sucesso do Server");
        mainMenuUIController.BackButtonCallback();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[PhotonManager] Successfully Connected to Lobby");
        mainMenuUIController.SetStatusMassege("Conectado com Sucesso ao Lobby");
        mainMenuUIController.PlayButtonCallback();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("[PhotonManager] Successfully Disconnected from Lobby");
        Debug.Log("[PhotonManager] Disconnecting from Server...");
        mainMenuUIController.SetStatusMassege("Desconectado com Sucesso do Lobby - Desconectando do Server...");
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("[PhotonManager] Successfully Created Room");
        mainMenuUIController.SetStatusMassege("Sala Criada com Sucesso");
        mainMenuUIController.CreateRoomButtonCallback();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[PhotonManager] Successfully Joined Room");
        mainMenuUIController.SetStatusMassege("Entrou na Sala com Sucesso");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.Log($"[PhotonManager] Player {otherPlayer.NickName} Joined Room");
        roomUIController.WriteToChat($"<color=#a0ffa0>Jogador {otherPlayer.NickName} Entrou na Sala </color>");
        roomUIController.UpdateWaitingForPlayersOverlay();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PhotonManager] Player {otherPlayer.NickName} Joined Room");
        roomUIController.WriteToChat($"<color=#ffa0a0>Jogador {otherPlayer.NickName} Deixou a Sala </color>");
        roomUIController.UpdateWaitingForPlayersOverlay();
        roomManager.ResetGame();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        mainMenuUIController.RoomListUpdate(roomList);
    }
}
