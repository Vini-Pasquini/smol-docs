using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    protected float movementSpeed = 0f;
    protected Vector3 newPosition = Vector3.zero;

    private void Awake()
    {
        movementSpeed = Time.deltaTime;
    }

    private void Update()
    {
        newPosition = transform.position;

        newPosition += (movementSpeed * (Input.GetKey(KeyCode.W) ? Vector3.up : (Input.GetKey(KeyCode.S) ? Vector3.down : Vector3.zero)));
        newPosition += (movementSpeed * (Input.GetKey(KeyCode.A) ? Vector3.left : (Input.GetKey(KeyCode.D) ? Vector3.right : Vector3.zero)));

        transform.position = newPosition;
    }
}
