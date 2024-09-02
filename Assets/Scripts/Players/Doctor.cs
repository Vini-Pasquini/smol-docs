using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    private PhotonView photonView;

    private float movementSpeed = 2f;
    private Vector3 newPosition = Vector3.zero;

    private void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!this.photonView.IsMine) return;

        newPosition = transform.position;

        newPosition += ((Time.deltaTime * movementSpeed) * (Input.GetKey(KeyCode.W) ? Vector3.up : (Input.GetKey(KeyCode.S) ? Vector3.down : Vector3.zero)));
        newPosition += ((Time.deltaTime * movementSpeed) * (Input.GetKey(KeyCode.A) ? Vector3.left : (Input.GetKey(KeyCode.D) ? Vector3.right : Vector3.zero)));

        transform.position = newPosition;
    }
}
