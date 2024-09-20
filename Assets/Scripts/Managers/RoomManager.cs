using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private Transform gatheringDoctorSpawn;
    [SerializeField] private Transform combatDoctorSpawn;

    [SerializeField] private GameObject enemyPilePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnList;

    RaycastHit hitInfo;

    public GameObject otherPlayer { get; private set; }
    public GameObject myPlayer { get; private set; }
    public PlayerRPC myPlayerRPC { get; private set; }
    public PhotonView myPhotonView { get; private set; }

    public Doctor myDoctor { get; private set; }
    public Doctor otherDoctor { get; private set; }
    public DoctorType myDoctorType { get; private set; }
    public DoctorType otherDoctorType { get; private set; }

    public bool myPlayerReady { get; private set; }
    public bool otherPlayerReady { get; private set; }

    public bool runningLevel { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        runningLevel = false;

        myPlayer = PhotonNetwork.Instantiate("Doctor", Vector3.forward * -20f, Quaternion.identity);
        myPlayerRPC = myPlayer.GetComponent<PlayerRPC>();
        myPhotonView = myPlayer.GetComponent<PhotonView>();
        
        myDoctor = myPlayer.GetComponent<Doctor>();
        
        myDoctorType = DoctorType.None;
        otherDoctorType = DoctorType.None;

        RoomUIController.Instance.UpdateWaitingForPlayersOverlay();
    }

    private void Update()
    {
        if (!runningLevel) return;
        switch (myDoctorType)
        {
            case DoctorType.GatheringDoctor:
                LocalGatheringDoctorUpdate();
                break;
            case DoctorType.CombatDoctor:
                LocalCombatDoctorUpdate();
                break;
            default:
                break;
        }
    }

    /* Gathering Doctor Stuff */

    public void LocalGatheringDoctorUpdate()
    {
        // vai ter coisa aki, eu juro
    }

    public void SpawnEnemyPile(Vector3 spawPosition)
    {
        GameObject.Instantiate(enemyPilePrefab, spawPosition, Quaternion.identity);
    }

    /* Combat Doctor Stuff */

    private float enemySpawnInterval = 5;
    private float enemySpawnTimer = 5;

    public void LocalCombatDoctorUpdate()
    {
        enemySpawnTimer -= Time.deltaTime;
        if (enemySpawnTimer <= 0)
        {
            int spawnIndex = Random.Range(0, enemySpawnList.Length);
            // TODO: adicionar um cap nessa kceta, pq ta spawnando a rodo, e se o player nao limpar, da kakinha
            GameObject.Instantiate(enemyPrefab, enemySpawnList[spawnIndex].position, Quaternion.identity).GetComponent<EnemyController>().EnemyInit(this.enemySpawnList, spawnIndex);
            enemySpawnTimer = enemySpawnInterval;
        }
    }

    /* Other Stuff */

    public void UpdateDoctorType(DoctorType newDoctorType, bool otherDoctor = false)
    {
        if (otherDoctor) { this.otherDoctorType = (this.otherDoctorType == DoctorType.None || this.otherDoctorType != newDoctorType ? newDoctorType : DoctorType.None); }
        else { this.myDoctorType = (this.myDoctorType == DoctorType.None || this.myDoctorType != newDoctorType ? newDoctorType : DoctorType.None); }

        RoomUIController.Instance.UpdateSelectedDoctorsPanel();
    }

    public void ResetDoctorType()
    {
        this.myDoctorType = DoctorType.None;
        this.otherDoctorType = DoctorType.None;

        RoomUIController.Instance.UpdateSelectedDoctorsPanel();
    }

    public void UpdatePlayerReady(bool readyState, bool otherPlayer = false)
    {
        if (otherPlayer) { this.otherPlayerReady = readyState; }
        else { this.myPlayerReady = readyState; }

        RoomUIController.Instance.UpdateReadyButton();

        if (this.myPlayerReady && this.otherPlayerReady)
        {
            this.StartGame();
        }
    }

    public void ResetPlayerReady()
    {
        this.myPlayerReady = false;
        this.otherPlayerReady = false;

        RoomUIController.Instance.UpdateReadyButton();
    }

    public void StartGame()
    {
        this.runningLevel = true;
        
        foreach (GameObject currentPlayer in GameObject.FindGameObjectsWithTag("Player")) // TODO: adicionar spectator
        {
            if (currentPlayer == myPlayer) continue;
            otherPlayer = currentPlayer;
            break;
        }

        myDoctor.enabled = true;
        myDoctor.DoctorInit(myDoctorType, (myDoctorType == DoctorType.GatheringDoctor ? this.gatheringDoctorSpawn.position : this.combatDoctorSpawn.position));
        
        otherPlayer.GetComponent<Doctor>().enabled = true;
        otherPlayer.GetComponent<Doctor>().DoctorInit(otherDoctorType, Vector3.zero);
        
        RoomUIController.Instance.ChangeCanvas();
    }
}
