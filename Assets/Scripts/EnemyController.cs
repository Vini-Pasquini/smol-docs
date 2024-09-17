using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3 startPosition;
    private float timer = 0;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime / 30;
        transform.position = Vector3.Lerp(startPosition, Vector3.zero, timer);
    }
}
