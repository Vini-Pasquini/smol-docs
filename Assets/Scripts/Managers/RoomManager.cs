using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public GameObject myPlayer { get; private set; }
    public PlayerRPC myPlayerRPC { get; private set; }
    public PhotonView myPhotonView { get; private set; }

    private void Awake()
    {
        // if (GameObject.FindObjectsByType<RoomManager>(FindObjectsSortMode.None).Length > 1) { GameObject.Destroy(this.gameObject); return; }
        Instance = this;
        // GameObject.DontDestroyOnLoad(this);
    }

    private void Start()
    {
        myPlayer = PhotonNetwork.Instantiate("Doctor", Vector3.zero, Quaternion.identity);
        myPlayerRPC = myPlayer.GetComponent<PlayerRPC>();
        myPhotonView = myPlayer.GetComponent<PhotonView>();
    }
}
