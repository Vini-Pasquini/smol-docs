using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private RoomUIController roomUIController;
    private PhotonManager photonManager;
    private AudioManager audioManager;

    [SerializeField] private Transform gatheringDoctorSpawn;
    [SerializeField] private Transform combatDoctorSpawn;

    [SerializeField] private GameObject enemyPilePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnList;

    [SerializeField] private Transform interactionArea;

    RaycastHit hitInfo;

    private GameObject _otherPlayer;
    public GameObject OtherPlayer { get { return this._otherPlayer; } }
    private GameObject _myPlayer;
    public GameObject MyPlayer { get { return this._myPlayer; } }
    private PlayerRPC _myPlayerRPC;
    public PlayerRPC MyPlayerRPC { get { return this._myPlayerRPC; } }
    private PhotonView _myPhotonView;
    public PhotonView MyPhotonView { get { return this._myPhotonView; } }

    private Doctor _myDoctor;
    public Doctor MyDoctor { get { return this._myDoctor; } }
    private Doctor _otherDoctor;
    public Doctor OtherDoctor { get { return this._otherDoctor; } }
    private DoctorType _myDoctorType;
    public DoctorType MyDoctorType { get { return this._myDoctorType; } }
    private DoctorType _otherDoctorType;
    public DoctorType OtherDoctorType { get { return this._otherDoctorType; } }

    private bool _myPlayerReady;
    public bool MyPlayerReady { get { return this._myPlayerReady; } }
    private bool _otherPlayerReady;
    public bool OtherPlayerReady { get { return this._otherPlayerReady; } }

    private bool _runningLevel;
    public bool RunningLevel { get { return this._runningLevel; } }

    private int _enemyAmount;
    public int EnemyAmount { get { return this._enemyAmount; } }

    private float _maxPlayerScale = 5f;

    // nha
    private float _interactionReach = 4f;
    public float InteractionReach { get { return this._interactionReach; } }
    private float _ampouleInteractionReach = 2.5f;
    public float AmpouleInteractionReach { get { return this._ampouleInteractionReach; } }

    private void Awake()
    {
        roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        audioManager.PlayAudioClip(AudioSample.Lobby);

        photonManager.InitRoomStuff();
    }

    private void Start()
    {
        this._runningLevel = false;

        this._myPlayer = PhotonNetwork.Instantiate("Doctor", Vector3.forward * -20f, Quaternion.identity);
        this._myPlayerRPC = this._myPlayer.GetComponent<PlayerRPC>();
        this._myPhotonView = this._myPlayer.GetComponent<PhotonView>();

        this._myDoctor = this._myPlayer.GetComponent<Doctor>();

        this._myDoctorType = DoctorType.None;
        this._otherDoctorType = DoctorType.None;

        this._enemyAmount = 0;

        roomUIController.UpdateWaitingForPlayersOverlay();
    }

    private void Update()
    {
        if (!this._runningLevel) return;
        switch (this._myDoctorType)
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

        if (this._myPlayer.transform.localScale.x >= this._maxPlayerScale || this._otherPlayer.transform.localScale.x >= this._maxPlayerScale)
        {
            this.EndGame(false);
        }

        this.interactionArea.position = this._myPlayer.transform.position;
    }

    /* Gathering Doctor Stuff */

    public void LocalGatheringDoctorUpdate()
    {
        // vai ter coisa aki, eu juro
        // na real, acho que nao, nao lembro o uqe eu tava planejando por aki, vamo ver iuashdiusahdiusaz
    }

    public void SpawnEnemyPile(Vector3 spawPosition, EnemyType enemyType)
    {
        // TODO: guardar o tipo no scriptable dps que eu mudar
        GameObject.Instantiate(enemyPilePrefab, spawPosition, Quaternion.identity).transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"ph{enemyType}Remains");
    }

    /* Combat Doctor Stuff */

    private float enemySpawnInterval = .5f;
    private float enemySpawnTimer = 5;

    public void LocalCombatDoctorUpdate()
    {
        enemySpawnTimer -= Time.deltaTime;
        if (enemySpawnTimer <= 0 && this._enemyAmount < 25)
        {
            int spawnIndex = Random.Range(0, enemySpawnList.Length);
            GameObject newEnemy = GameObject.Instantiate(enemyPrefab, enemySpawnList[spawnIndex].position, Quaternion.identity);
            newEnemy.GetComponent<EnemyController>().EnemyInit(this.enemySpawnList, spawnIndex);
            enemySpawnTimer = enemySpawnInterval;
            this._enemyAmount++;
        }
    }

    /* Other Stuff */

    public void UpdateDoctorType(DoctorType newDoctorType, bool otherDoctor = false)
    {
        if (otherDoctor) { this._otherDoctorType = (this._otherDoctorType == DoctorType.None || this._otherDoctorType != newDoctorType ? newDoctorType : DoctorType.None); }
        else { this._myDoctorType = (this._myDoctorType == DoctorType.None || this._myDoctorType != newDoctorType ? newDoctorType : DoctorType.None); }

        roomUIController.UpdateSelectedDoctorsPanel();
    }

    public void ResetDoctorType()
    {
        this._myDoctorType = DoctorType.None;
        this._otherDoctorType = DoctorType.None;

        roomUIController.UpdateSelectedDoctorsPanel();
    }

    public void UpdatePlayerReady(bool readyState, bool otherPlayer = false)
    {
        if (otherPlayer) { this._otherPlayerReady = readyState; }
        else { this._myPlayerReady = readyState; }

        roomUIController.UpdateReadyButton();

        if (this._myPlayerReady && this._otherPlayerReady)
        {
            this.StartGame();
        }
    }

    public void ResetPlayerReady()
    {
        this._myPlayerReady = false;
        this._otherPlayerReady = false;

        roomUIController.UpdateReadyButton();
    }

    public void StartGame()
    {
        this._runningLevel = true;
        
        foreach (GameObject currentPlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (currentPlayer == this._myPlayer) continue;
            this._otherPlayer = currentPlayer;
            break;
        }

        this._myDoctor.enabled = true;
        this._myDoctor.DoctorInit(this._myDoctorType, (this._myDoctorType == DoctorType.GatheringDoctor ? this.gatheringDoctorSpawn.position : this.combatDoctorSpawn.position));

        this._otherPlayer.GetComponent<Doctor>().enabled = true;
        this._otherPlayer.GetComponent<Doctor>().DoctorInit(this._otherDoctorType, Vector3.zero);

        roomUIController.ToggleLobbyCanvas(false);
        audioManager.PlayAudioClip(AudioSample.Level);
    }

    public void EndGame(bool hasWon)
    {
        roomUIController.SetGameoverOverlay(hasWon);

        foreach (Doctor currentPlayer in GameObject.FindObjectsByType<Doctor>(FindObjectsInactive.Include, FindObjectsSortMode.None)) // TODO: adicionar spectator
        {
            currentPlayer.enabled = false;
        }

        this._runningLevel = false;
        this.audioManager.PlayAudioClip((hasWon ? AudioSample.Victory : AudioSample.Defeat));
    }

    public void ResetGame() // TODO: resetar tudo (preciso de um array de entidades)
    {
        this._runningLevel = false;

        foreach (Doctor currentPlayer in GameObject.FindObjectsByType<Doctor>(FindObjectsInactive.Include, FindObjectsSortMode.None)) // TODO: adicionar spectator
        {
            currentPlayer.DoctorReset();
        }

        roomUIController.ToggleLobbyCanvas(true);
        audioManager.PlayAudioClip(AudioSample.Lobby);
    }

    /* Running Game Stuff */

    public void UpdateEnemyAmount(int increment)
    {
        this._enemyAmount += increment;
    }

    public void ResetPlayerSize() // sla pq, mas só funciona na roomManager, gg
    {

        if ((this.MyPlayer.transform.position - this.OtherPlayer.transform.position).magnitude <= this._ampouleInteractionReach)
        {
            this._myPlayer.transform.localScale = Vector3.one;
        }
    }
}
