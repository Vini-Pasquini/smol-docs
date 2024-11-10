using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
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

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timer;

    private bool hasBeenInit = false;

    RaycastHit hitInfo;

    public Enemy(GameObject enemyObject)
    {
        this._gameObject = enemyObject;

        this.transform = this._gameObject.transform;

        this.spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.animator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    public void Init(EnemyType enemyType, RuntimeAnimatorController runtimeAnimatorController)
    {
        this.timer = 0f;

        this._enemyType = enemyType;
        this.animator.runtimeAnimatorController = runtimeAnimatorController;

        this.startPosition = this.endPosition = this.transform.position;

        this.endPosition = this.FindDestination();

        this.hasBeenInit = true;
    }

    public void CustomUpdate()
    {
        spriteRenderer.sortingOrder = (int)(this.transform.position.y * -100f);

        if (!hasBeenInit) return;

        timer += Time.deltaTime / (endPosition - startPosition).magnitude;
        transform.position = Vector3.Lerp(startPosition, endPosition, timer);
        if (timer >= 1f)
        {
            this.endPosition = this.FindDestination();
            timer = 0f;
        }
    }

    private Vector3 FindDestination()
    {
        this.startPosition = this.endPosition;

        int iterations = 0;

        Vector3 randomDirection = Vector3.zero;

        do
        {
            iterations++;

            randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            if (Physics.Raycast(this.transform.position, randomDirection, out hitInfo, int.MaxValue))
            {
                if (!hitInfo.collider.CompareTag("Wall")) continue;

                return hitInfo.point;
            }
        } while (this.startPosition == this.endPosition || iterations < 50);

        return this.startPosition + Vector3.down;
    }
}
