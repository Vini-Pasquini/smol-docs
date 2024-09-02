using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GameObject.Instantiate(PhotonManager.Instance.PlayerPrefab, Vector3.left, Quaternion.identity);
    }
}
