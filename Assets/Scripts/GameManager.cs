using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int myViewID { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.Instantiate("Doctor", Vector3.zero, Quaternion.identity);
    }
}
