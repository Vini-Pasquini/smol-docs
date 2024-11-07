using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy
{
    private EnemyType _enemyType;
    public EnemyType EnemyType { get { return this._enemyType; } }

    private GameObject _gameObject;
    public GameObject EnemyObject { get { return this._gameObject; } }

    private Transform transform;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector3[] positionList;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timer;

    private bool hasBeenInit = false;

    private int startPositionIndex;
    private int endPositionIndex;

    public Enemy(GameObject enemyObject)
    {
        this._gameObject = enemyObject;

        this.transform = this._gameObject.transform;

        this.spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.animator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    public void Init(EnemyType enemyType, RuntimeAnimatorController runtimeAnimatorController, Transform[] positionList, int initialPositionIndex)
    {
        this.timer = 0f;

        this._enemyType = enemyType;
        this.animator.runtimeAnimatorController = runtimeAnimatorController;

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

    public void CustomUpdate()
    {
        spriteRenderer.sortingOrder = (int)(this.transform.position.y * -100f);

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
}
