using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusMessage;
    [Header("Menu Screens")]
    [SerializeField] private GameObject TitleScreenCanvas;
    [SerializeField] private GameObject GameLobbyCanvas;
    [Header("Game Title Animation")]
    [SerializeField] private Transform gameTitleTransform;
    [SerializeField] private Transform gameTitleLowLimit;
    [SerializeField] private Transform gameTitleHighLimit;
    [Header("Room List")]
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomContent;
    private List<GameObject> roomObjectList = new List<GameObject>();
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

    private void Start()
    {
        gameTitleMaxPosition = gameTitleHighLimit.position;
        gameTitleMinPosition = gameTitleLowLimit.position;

        statusMessage.text = "...";
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

    public void SetStatusMassege(string newMessage)
    {
        statusMessage.text = newMessage;
    }

    /* * * * * * * * * * * * * * */
    /*  Menu Interaction Methods */
    /* * * * * * * * * * * * * * */
    
    /* TitleScreen */
    // PlayButton
    public void OnPlayButtonPressed()
    {
#if UNITY_EDITOR
        Debug.Log("[MainMenuController] Connecting to Server...");
#endif
        this.SetStatusMassege("Conectando ao Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void PlayButtonCallback()
    {
        TitleScreenCanvas.SetActive(false);
        GameLobbyCanvas.SetActive(true);
    }

    // QuitButton
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    /* Lobby */
    // Back
    public void OnBackButtonPressed()
    {
#if UNITY_EDITOR
        Debug.Log("[MainMenuController] Disconnecting from Lobby...");
#endif
        this.SetStatusMassege("Desconectando do Lobby...");
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
#if UNITY_EDITOR
        Debug.Log($"[MainMenuController] Creating \"{PhotonNetwork.NickName}\" Room...");
#endif
        this.SetStatusMassege($"Criando Sala \"{PhotonNetwork.NickName}\"...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, roomOptions);
    }

    public void CreateRoomButtonCallback()
    {
        // vai ter
    }

    // Join Room
    public void OnJoinRoomButtonPressed(TextMeshProUGUI roomName)
    {
#if UNITY_EDITOR
        Debug.Log($"[MainMenuController] Joining \"{roomName.text}\" Room...");
#endif
        this.SetStatusMassege($"Entrando na Sala \"{roomName.text}\"...");
        PhotonNetwork.JoinRoom(roomName.text);
    }

    public void JoinRoomButtonCallback()
    {
        // vai ter tbm
    }

    // Room List
    public void RoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject roomObject in roomObjectList) { Destroy(roomObject); }
        roomObjectList.Clear();
        foreach (RoomInfo currentRoom in roomList)
        {
            GameObject newRoomObject = GameObject.Instantiate(this.roomPrefab, this.roomContent);
            newRoomObject.GetComponentInChildren<TextMeshProUGUI>().text = currentRoom.Name;
            newRoomObject.GetComponentInChildren<Button>().interactable = currentRoom.PlayerCount < currentRoom.MaxPlayers;
            roomObjectList.Add(newRoomObject);
            this.roomContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (160f * this.roomContent.childCount) + 10f);
        }
    }
}
