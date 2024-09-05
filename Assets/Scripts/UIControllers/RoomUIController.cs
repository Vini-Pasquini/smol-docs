using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUIController : MonoBehaviour
{
    public static RoomUIController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
