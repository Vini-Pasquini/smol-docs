using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIController : MonoBehaviour
{
    private RoomManager roomManager;

    [Header("Canvas")]
    [SerializeField] private GameObject roomLobbyCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject gatheringDocCanvas;
    [SerializeField] private GameObject combatDocCanvas;

    [Header("Chat")]
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private TextMeshProUGUI messageContainer;

    [Header("Doctor Selector")]
    [SerializeField] private GameObject waitingForPlayersOverlay;
    [SerializeField] private Image gatheringDocSprite;
    [SerializeField] private Image gatheringDocPlayerOverlay;
    [SerializeField] private Image combatDocSprite;
    [SerializeField] private Image combatDocPlayerOverlay;

    [Header("Ready System")]
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI readyButtonText;
    private Sprite[] playerOverlay;

    [Header("Doctor Resources")]
    [SerializeField] private TextMeshProUGUI resourcesDisplay;

    [Header("Gameover")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private RectTransform backToLobbyButtonRectTransform;

    [SerializeField] private Image lossOverlayImage;
    [SerializeField] private TextMeshProUGUI lossOverlayText;

    public GameObject GameOverCanvas { get { return gameOverCanvas; } }

    private GameObject loseOverlay;
    private GameObject winOverlay;

    private Color bufferColor;
    private Vector3 bufferPosition;

    private float animationTime = 0f;
    private bool animateGameover = false;

    private void Awake()
    {
        this.roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();

        this.loseOverlay = this.gameOverCanvas.transform.GetChild(0).gameObject;
        this.winOverlay = this.gameOverCanvas.transform.GetChild(1).gameObject;

        this.playerOverlay = Resources.LoadAll<Sprite>("PlayerOverlay");
    }

    private void Start()
    {
        this.messageContainer.text = string.Empty;
    }

    private void Update()
    {
        if (animateGameover)
        {
            // defeat
            this.bufferColor = this.lossOverlayImage.color;
            this.bufferColor.a =  Mathf.Lerp(0f, .75f, this.animationTime);
            this.lossOverlayImage.color = this.bufferColor;

            this.bufferColor = this.lossOverlayText.color;
            this.bufferColor.a =  Mathf.Lerp(0f, 1f, this.animationTime);
            this.lossOverlayText.color = this.bufferColor;

            // victory

            // common
            this.bufferPosition = this.backToLobbyButtonRectTransform.anchoredPosition;
            this.bufferPosition.y = Mathf.Lerp(-150f, 150f, this.animationTime);
            this.backToLobbyButtonRectTransform.anchoredPosition = this.bufferPosition;

            this.animationTime += Time.deltaTime;
            if (this.animationTime > 1f) { this.animationTime = 1f; }
        }
    }

    /* Doctor Selection */
    public void UpdateWaitingForPlayersOverlay(bool forceReset = false)
    {
        this.waitingForPlayersOverlay.SetActive(PhotonNetwork.CurrentRoom.PlayerCount <= 1 || forceReset);
        this.roomManager.ResetDoctorType();
    }

    public void OnDoctorSelectButtonPress(int type)
    {
        DoctorType selectedType = (DoctorType)type;

        this.roomManager.MyPlayerRPC.RPCSelectDoctor(selectedType);
        this.roomManager.UpdateDoctorType(selectedType, false);
    }

    public void UpdateSelectedDoctorsPanel()
    {
        DoctorType myDoctorType = this.roomManager.MyDoctorType;
        DoctorType otherDoctorType = this.roomManager.OtherDoctorType;

        this.gatheringDocPlayerOverlay.sprite = this.playerOverlay[(myDoctorType == DoctorType.GatheringDoctor && otherDoctorType == DoctorType.GatheringDoctor ? 3 : (myDoctorType != DoctorType.GatheringDoctor && otherDoctorType == DoctorType.GatheringDoctor ? 2 : (myDoctorType == DoctorType.GatheringDoctor && otherDoctorType != DoctorType.GatheringDoctor ? 1 : 0)))];
        this.combatDocPlayerOverlay.sprite = this.playerOverlay[(myDoctorType == DoctorType.CombatDoctor && otherDoctorType == DoctorType.CombatDoctor ? 3 : (myDoctorType != DoctorType.CombatDoctor && otherDoctorType == DoctorType.CombatDoctor ? 2 : (myDoctorType == DoctorType.CombatDoctor && otherDoctorType != DoctorType.CombatDoctor ? 1 : 0)))];

        this.gatheringDocSprite.color = new Color(this.gatheringDocSprite.color.r, this.gatheringDocSprite.color.g, this.gatheringDocSprite.color.b, (myDoctorType != DoctorType.GatheringDoctor && otherDoctorType != DoctorType.GatheringDoctor ? .125f : 1f));
        this.combatDocSprite.color = new Color(this.combatDocSprite.color.r, this.combatDocSprite.color.g, this.combatDocSprite.color.b, (myDoctorType != DoctorType.CombatDoctor && otherDoctorType != DoctorType.CombatDoctor ? .125f : 1f));

        this.readyButton.interactable = myDoctorType != DoctorType.None && otherDoctorType != DoctorType.None && myDoctorType != otherDoctorType;
        this.roomManager.ResetPlayerReady();
    }

    /* Chat */
    public void WriteToChat(string newLine)
    {
        this.messageContainer.text += $"{newLine}\n";
    }

    public void OnSendMessageButtonPress()
    {
        this.roomManager.MyPlayerRPC.RPCSendMessage(messageInputField.text);
        this.WriteToChat($"<color=#FF0080>eu</color>: {messageInputField.text}");
        this.messageInputField.text = string.Empty;
    }

    public void OnRecieveMessageCallback(string newMessage)
    {
        this.WriteToChat($"<color=#FF8000>outro</color>: {newMessage}");
    }

    /* Ready System */
    public void OnReadyButtonPress()
    {
        this.roomManager.MyPlayerRPC.RPCSetReady(!roomManager.MyPlayerReady);
        this.roomManager.UpdatePlayerReady(!roomManager.MyPlayerReady, false);
    }

    public void UpdateReadyButton()
    {
        bool myPlayerReady = this.roomManager.MyPlayerReady;
        bool otherPlayerReady = this.roomManager.OtherPlayerReady;

        this.readyButtonText.text = (myPlayerReady && otherPlayerReady ? "INICIANDO..." : (myPlayerReady ? "[X] PRONTO" : "[ ] PRONTO"));
        this.readyButton.image.color = (myPlayerReady && otherPlayerReady ? Color.green : (otherPlayerReady ? Color.yellow : Color.white));
    }

    public void ToggleLobbyCanvas(bool toggleOn)
    {
        // lobby
        this.roomLobbyCanvas.SetActive(toggleOn);
        // gameplay
        this.gameplayCanvas.SetActive(!toggleOn);
        this.gatheringDocCanvas.SetActive(!toggleOn && this.roomManager.MyDoctorType == DoctorType.GatheringDoctor);
        this.combatDocCanvas.SetActive(!toggleOn && this.roomManager.MyDoctorType == DoctorType.CombatDoctor);
    }

    /* Resources */
    public void UpdateResourcesDisplay() // mudar dps
    {
        Doctor doc = this.roomManager.MyDoctor;
        if (this.roomManager.MyDoctorType == DoctorType.GatheringDoctor)
        {
            this.resourcesDisplay.text = $"[RESTOS]\n Leucocitos: {doc.LeukocyteAmount} / ---\n Patogenos: {doc.PathogenAmount} / ---\n Soro de Encolhimento: {doc.ShrinkSerumAmount}";
            return;
        }
        this.resourcesDisplay.text = $"[MUNICOES]\n Morfina: {doc.MorphineAmount} / ---\n Vacina: {doc.VaccineAmount} / ---\n C.A.V.A.L.O.: {(doc.CavaloDeployed ? "Deployed" : "Stored")}";
        return;
    }

    /* Gathering Doctor */
    public void OnAmpouleButtonPress()
    {
        if (roomManager.MyDoctor.ShrinkSerumAmount >= 15 && (roomManager.MyPlayer.transform.position - roomManager.OtherPlayer.transform.position).magnitude <= roomManager.AmpouleInteractionReach)
        {
            int doses = roomManager.MyDoctor.ShrinkSerumAmount / 15;
            //roomManager.MyPlayerRPC.RPCDecreaseDoctorSize(doses);
            //roomManager.MyDoctor.UpdateShrinkSerumAmount(-15 * doses);
            roomManager.MyPlayerRPC.RPCDecreaseDoctorSize(1);
            roomManager.MyDoctor.UpdateShrinkSerumAmount(-15);
        }
    }

    /* Combat Doctor */
    public void OnCavaloButtonPress()
    {
        roomManager.MyDoctor.DeployCavaloInteraction();
    }

    /* Gameover */
    public void SetGameoverOverlay(bool toggleVictoryOverlay)
    {
        this.gameplayCanvas.SetActive(false);
        this.gatheringDocCanvas.SetActive(false);
        this.combatDocCanvas.SetActive(false);

        if (toggleVictoryOverlay) { this.winOverlay.SetActive(true); }
        else { this.loseOverlay.SetActive(true); }

        this.gameOverCanvas.SetActive(true);

        this.animateGameover = true;
        this.animationTime = 0f;
    }

    public void OnBackToLobbyButtonPress()
    {
        this.winOverlay.SetActive(false);
        this.loseOverlay.SetActive(false);
        this.gameOverCanvas.SetActive(false);

        this.UpdateWaitingForPlayersOverlay(true);
        roomManager.MyPhotonView.RPC("OtherPlayerBackToLobby", RpcTarget.Others);

        roomManager.ResetGame();

        this.animateGameover = false;
        this.animationTime = 0f;
    }

    // Stuff
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
