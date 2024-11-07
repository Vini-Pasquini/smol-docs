using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController leukocyteAnimatorController;
    [SerializeField] private RuntimeAnimatorController pathogenVirusAnimatorController;
    [SerializeField] private RuntimeAnimatorController pathogenBacteriaAnimatorController;

    [SerializeField] private GameObject enemyPrefab;

    private Enemy[] enemies;

    private int _enemyCapacity = 25;
    private int _enemyCount = 0;

    private bool _canSpawn = true;
    public bool CanSpawn { get { return this._canSpawn; } }

    private void Awake()
    {
        this.enemies = new Enemy[this._enemyCapacity];
    }

    private void Update()
    {
        for (int i = 0; i < this._enemyCount; i++)
        {
            this.enemies[i].CustomUpdate();
        }
    }

    public void SpawnEnemy(Transform[] enemySpawnList)
    {
        if (this._enemyCount >= this._enemyCapacity) { return; }

        Debug.Log("Spawn");

        EnemyType newType = (EnemyType)Random.Range(1, (int)EnemyType._count);
        RuntimeAnimatorController runtimeAnimatorController = (newType == EnemyType.Leukocyte ? this.leukocyteAnimatorController : (newType == EnemyType.PathogenVirus ? this.pathogenVirusAnimatorController : this.pathogenBacteriaAnimatorController));

        int spawnIndex = Random.Range(0, enemySpawnList.Length);
        GameObject newEnemyObject = GameObject.Instantiate(enemyPrefab, enemySpawnList[spawnIndex].position, Quaternion.identity);

        this.enemies[this._enemyCount] = new Enemy(newEnemyObject);
        this.enemies[this._enemyCount].Init(newType, runtimeAnimatorController, enemySpawnList, spawnIndex);

        this._enemyCount++;

        this._canSpawn = this._enemyCount < this._enemyCapacity;
    }

    public EnemyType KillEnemy(GameObject enemy)
    {
        EnemyType result = EnemyType.None;

        bool found = false;
        for (int i = 0; i < this._enemyCount; i++)
        {
            if (this.enemies[i].EnemyObject == enemy)
            {
                found = true;
                result = this.enemies[i].EnemyType;
                GameObject.Destroy(this.enemies[i].EnemyObject);
                this.enemies[i] = null;
                continue;
            }

            if (!found) continue;

            this.enemies[i - 1] = this.enemies[i];
        }

        this.enemies[--this._enemyCount] = null;

        this._canSpawn = this._enemyCount < this._enemyCapacity;

        return result;
    }
}
