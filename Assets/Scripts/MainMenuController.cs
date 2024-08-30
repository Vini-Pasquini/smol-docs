using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    [Header("Menu Screens")]
    [SerializeField] private GameObject TitleScreenCanvas;
    [SerializeField] private GameObject GameLobbyCanvas;
    [Header("Game Title Animation")]
    [SerializeField] private Transform gameTitleTransform;
    [SerializeField] private Transform gameTitleLowLimit;
    [SerializeField] private Transform gameTitleHighLimit;
    // position
    private Vector3 gameTitleMinPosition;
    private Vector3 gameTitleMaxPosition;
    private bool gameTitlePositionAnimationInvertDirection = false;
    private float gameTitlePositionAnimationTimer = .5f;
    private float gameTitlePositionAnimationLength = 2f;
    //rotation
    private float gameTitleRotationLimit = 7f;
    private bool gameTitleRotationAnimationInvertDirection = false;
    private float gameTitleRotationAnimationTimer = .5f;
    private float gameTitleRotationAnimationLength = 4.5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameTitleMaxPosition = gameTitleHighLimit.position;
        gameTitleMinPosition = gameTitleLowLimit.position;
    }

    private void Update()
    {
        // position
        gameTitleTransform.position = new Vector3(gameTitleTransform.position.x, Mathf.Lerp(gameTitleMinPosition.y, gameTitleMaxPosition.y, gameTitlePositionAnimationTimer), 0f);
        gameTitlePositionAnimationTimer += (Time.deltaTime / gameTitlePositionAnimationLength) * (gameTitlePositionAnimationInvertDirection ? -1 : 1);
        if (gameTitlePositionAnimationTimer < 0f) { gameTitlePositionAnimationTimer = 0f; gameTitlePositionAnimationInvertDirection = false; }
        if (gameTitlePositionAnimationTimer > 1f) { gameTitlePositionAnimationTimer = 1f; gameTitlePositionAnimationInvertDirection = true; }
        // rotation
        gameTitleTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(-gameTitleRotationLimit, gameTitleRotationLimit, gameTitleRotationAnimationTimer)));
        gameTitleRotationAnimationTimer += (Time.deltaTime / gameTitleRotationAnimationLength) * (gameTitleRotationAnimationInvertDirection ? -1 : 1);
        if (gameTitleRotationAnimationTimer < 0f) { gameTitleRotationAnimationTimer = 0f; gameTitleRotationAnimationInvertDirection = false; }
        if (gameTitleRotationAnimationTimer > 1f) { gameTitleRotationAnimationTimer = 1f; gameTitleRotationAnimationInvertDirection = true; }
    }

    /* * * * * * * * * * * * * * */
    /*  Menu Interaction Methods */
    /* * * * * * * * * * * * * * */
    
    /* TitleScreen */
    // PlayButton
    public void OnPlayButtonPressed()
    {
        Debug.Log("[PhotonManager] Connecting to Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void PlayButtonCallback()
    {
        TitleScreenCanvas.SetActive(false);
        GameLobbyCanvas.SetActive(true);
    }

    /* Lobby */
    // Back
    public void OnBackButtonPressed()
    {
        Debug.Log("[PhotonManager] Disconnecting from Lobby...");
        PhotonNetwork.LeaveLobby();
    }

    public void BackButtonCallback()
    {
        TitleScreenCanvas.SetActive(true);
        GameLobbyCanvas.SetActive(false);
    }

    // Create Room
    public void OnCreateRoomButtonPressed()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName);
    }

    public void CreateRoomButtonCallback()
    {
        // SceneManager.LoadScene("Game");
    }

    // Join Room
    public void OnJoinRoomButtonPressed()
    {
        PhotonNetwork.JoinRoom(PhotonNetwork.NickName);
    }

    public void JoinRoomButtonCallback()
    {
        // SceneManager.LoadScene("Game");
    }

    // Room List
    public void RoomListUpdate(List<RoomInfo> roomList)
    {

    }
}
