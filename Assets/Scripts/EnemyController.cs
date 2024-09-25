using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3[] positionList;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timer;

    private bool hasBeenInit = false;

    private int startPositionIndex;
    private int endPositionIndex;

    private void Update()
    {
        if (!hasBeenInit) return;

        // placeholder movement
        timer += Time.deltaTime / (endPosition - startPosition).magnitude;
        transform.position = Vector3.Lerp(startPosition, endPosition, timer);
        if (timer >= 1f)
        {
            this.startPositionIndex = this.endPositionIndex;
            this.startPosition = this.endPosition;

            do
            {
                this.endPositionIndex = Random.Range(0, this.positionList.Length);
            } while (this.endPositionIndex == this.startPositionIndex);

            this.endPosition = this.positionList[this.endPositionIndex];

            timer = 0f;
        }
    }

    public void EnemyInit(Transform[] positionList, int initialPositionIndex)
    {
        this.timer = 0f;

        this.positionList = new Vector3[positionList.Length + 1];
        for (int index = 0; index < positionList.Length; index++)
        {
            this.positionList[index] = positionList[index].position;
        }
        this.positionList[positionList.Length] = Vector3.zero;

        this.startPositionIndex = initialPositionIndex;
        this.startPosition = this.positionList[this.startPositionIndex];
        this.endPositionIndex = positionList.Length;
        this.endPosition = this.positionList[this.endPositionIndex];

        this.hasBeenInit = true;
    }
}