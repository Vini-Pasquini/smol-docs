using System.Collections;
using System.Collections.Generic;
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

        this.SetDestination(); // eh aki o problema, 100% de certeza

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
            this.SetDestination();
            timer = 0f;
        }
    }

    private void SetDestination()
    {
        this.startPosition = this.endPosition;
        
        Vector3 randomDirection = Vector3.zero;

        for (int maxIterations = 0; maxIterations < 10; maxIterations++) // tenta 10 vezes, se nao achar parede em 10 tentativas, só por deus
        {
            randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            if (Physics.Raycast(this.transform.position, randomDirection, out hitInfo, 500))
            {
                if (!hitInfo.collider.CompareTag("Wall")) continue;

                this.endPosition = hitInfo.point;
                return;
            }
        }

        this.endPosition = this.startPosition + randomDirection;
        return;
    }
}
