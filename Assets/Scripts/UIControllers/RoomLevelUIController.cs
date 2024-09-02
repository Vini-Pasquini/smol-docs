using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLevelUIController : MonoBehaviour
{
    public static RoomLevelUIController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnLeaveGameButtonPress()
    {
        PhotonNetwork.LoadLevel("RoomLobby");
    }
}
