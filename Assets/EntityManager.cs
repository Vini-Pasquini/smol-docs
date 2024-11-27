using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private GameObject capsulePrefab;

    [SerializeField] private Sprite capsuleMid;
    [SerializeField] private Sprite capsuleEmpty;

    public Sprite CapsuleMid { get { return capsuleMid; } }
    public Sprite CapsuleEmpty { get { return capsuleEmpty; } }

    private RoomManager _roomManager;
    private EnemyManager _enemyManager;

    private float enemySpawnInterval = .5f;
    private float enemySpawnTimer = 5f;

    private float capsuleSpawnInterval = 30f;
    private float capsuleSpawnTimer = 5f;

    private int capsuleCount = 0;

    RaycastHit hitInfo;
    RaycastHit secondHitInfo;

    Vector3 randomDirection = Vector3.zero;

    private void Awake()
    {
        this._roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        this._enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }

    private void Update()
    {
        this.TrySpawnEnemy();
        this.TrySpawnCapsule();
    }

    private void TrySpawnEnemy()
    {
        enemySpawnTimer -= Time.deltaTime;

        if (this._roomManager.MyDoctorType != DoctorType.CombatDoctor || enemySpawnTimer > 0 || !this._enemyManager.CanSpawn) { return; }
        
        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        if (Physics.Raycast(this.transform.position, randomDirection, out hitInfo, 500))
        {
            if (!hitInfo.collider.CompareTag("Wall")) return;

            this._enemyManager.SpawnEnemy(hitInfo.point + (this.transform.position - hitInfo.point).normalized);
            enemySpawnTimer = enemySpawnInterval;
        }
    }

    private void TrySpawnCapsule()
    {
        capsuleSpawnTimer -= Time.deltaTime;

        if (this._roomManager.MyDoctorType != DoctorType.GatheringDoctor || capsuleSpawnTimer > 0 || this.capsuleCount >= 7) { return; }

        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        if (Physics.Raycast(this.transform.position, randomDirection, out hitInfo, 500))
        {
            if (!hitInfo.collider.CompareTag("Wall")) return;

            if (Physics.Raycast(this.transform.position, hitInfo.normal, out secondHitInfo, 500))
            {
                if (!secondHitInfo.collider.CompareTag("Wall")) return;

                Vector3 hitVector = secondHitInfo.point - hitInfo.point;

                GameObject.Instantiate(capsulePrefab, hitVector.normalized * (hitVector.magnitude / 2f), Quaternion.identity);

                this.capsuleCount++;

                capsuleSpawnTimer = capsuleSpawnInterval;
            }
        }
    }

    public void NukeCapsules()
    {
        // ph
        foreach (GameObject currentCapsule in GameObject.FindGameObjectsWithTag("Capsule")) { GameObject.Destroy(currentCapsule); }
    }
}
