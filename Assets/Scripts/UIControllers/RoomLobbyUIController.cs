using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLobbyUIController : MonoBehaviour
{
    public static RoomLobbyUIController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnBackToLobbyButtonPress()
    {
        Debug.Log("Nao da pra voltar ainda");
    }

    public void OnStartGameButtonPress()
    {
        PhotonNetwork.LoadLevel("RoomLevel");
    }
}
