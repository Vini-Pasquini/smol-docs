using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerA;
    [SerializeField] private GameObject PlayerB;

    private void Start()
    {
        GameObject.Instantiate(PlayerA, Vector3.left, Quaternion.identity);
        GameObject.Instantiate(PlayerB, Vector3.right, Quaternion.identity);
    }
}
