using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    private PhotonView photonView;
    private Rigidbody doctorRigidbody;

    private float movementSpeed = 2f;
    private Vector3 newVelocity = Vector3.zero;

    private void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.doctorRigidbody = this.GetComponent<Rigidbody>();

        this.enabled = false; // until game starts
    }

    private void Update()
    {
        if (!this.photonView.IsMine) return;

        newVelocity = Vector3.zero;

        newVelocity.x = movementSpeed * (Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0));
        newVelocity.y = movementSpeed * (Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0));

        this.doctorRigidbody.velocity = newVelocity;
    }
}
