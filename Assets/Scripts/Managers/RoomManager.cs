using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        if (GameObject.FindObjectsByType<RoomManager>(FindObjectsSortMode.None).Length > 1) { GameObject.Destroy(this.gameObject); return; }
        Instance = this;
        GameObject.DontDestroyOnLoad(this);
    }
}
