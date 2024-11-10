using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
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

    [SerializeField] private TextMeshProUGUI scoreDisplay;

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
        if (Input.GetKeyDown(KeyCode.Return)) { this.OnSendMessageButtonPress(); }

        if (animateGameover)
        {
            // defeat
            this.bufferColor = this.lossOverlayImage.color;
            this.bufferColor.a = Mathf.Lerp(0f, .75f, this.animationTime);
            this.lossOverlayImage.color = this.bufferColor;

            this.bufferColor = this.lossOverlayText.color;
            this.bufferColor.a = Mathf.Lerp(0f, 1f, this.animationTime);
            this.lossOverlayText.color = this.bufferColor;

            // victory

            // common
            this.bufferPosition = this.backToLobbyButtonRectTransform.anchoredPosition;
            this.bufferPosition.y = Mathf.Lerp(-150f, 150f, this.animationTime);
            this.backToLobbyButtonRectTransform.anchoredPosition = this.bufferPosition;

            this.animationTime += Time.deltaTime;
            if (this.animationTime > 1f) { this.animationTime = 1f; }
        }

        this.UpdateTriggerAnimation();
        this.UpdateCavaloAnimation();
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

        this.gatheringDocSprite.color = new Color(this.gatheringDocSprite.color.r, this.gatheringDocSprite.color.g, this.gatheringDocSprite.color.b, (myDoctorType != DoctorType.GatheringDoctor && otherDoctorType != DoctorType.GatheringDoctor ? .4f : 1f));
        this.combatDocSprite.color = new Color(this.combatDocSprite.color.r, this.combatDocSprite.color.g, this.combatDocSprite.color.b, (myDoctorType != DoctorType.CombatDoctor && otherDoctorType != DoctorType.CombatDoctor ? .4f : 1f));

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
        if (messageInputField.text != string.Empty)
        {
            this.roomManager.MyPlayerRPC.RPCSendMessage(messageInputField.text);
            this.WriteToChat($"<color=#FF0080>eu</color>: {messageInputField.text}");
            this.messageInputField.text = string.Empty;
        }

        this.messageInputField.ActivateInputField();
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
            this.resourcesDisplay.text = $"[RESTOS]\n Leucocitos: {doc.LeukocyteAmount} / {doc.LeukocyteCap}\n Patogenos: {doc.PathogenAmount} / {doc.PathogenCap}\n Soro de Encolhimento: {doc.ShrinkSerumAmount}";
            return;
        }
        this.resourcesDisplay.text = $"[MUNICOES]\n Morfina: {doc.MorphineAmount} / {doc.MorphineCap}\n Vacina: {doc.VaccineAmount} / {doc.VaccineCap}\n C.A.V.A.L.O.: {(doc.CavaloDeployed ? "No Chao" : "No Inventario")}";
        return;
    }

    /* Gathering Doctor */
    public void OnAmpouleButtonPress()
    {
        this.PlayTriggerAnim();

        if (roomManager.MyDoctor.ShrinkSerumAmount >= 15 && (roomManager.MyPlayer.transform.position - roomManager.OtherPlayer.transform.position).magnitude <= roomManager.AmpouleInteractionReach)
        {
            int doses = roomManager.MyDoctor.ShrinkSerumAmount / 15;
            //roomManager.MyPlayerRPC.RPCDecreaseDoctorSize(doses);
            //roomManager.MyDoctor.UpdateShrinkSerumAmount(-15 * doses);
            roomManager.MyPlayerRPC.RPCDecreaseDoctorSize(1);
            roomManager.MyDoctor.UpdateShrinkSerumAmount(-15);
        }
    }

    // Ampoule Animation Stuff

    [SerializeField] private Image[] serumBars;
    [SerializeField] private RectTransform ampouleLever;

    private Color darkGreen = new Color(0, .5f, .25f);

    public void UpdateAmpoule()
    {
        int amount = this.roomManager.MyDoctor.ShrinkSerumAmount;

        for (int index = 0; index < 5; index++)
        {
            serumBars[index].fillAmount = index < amount / 15 ? 1f : (index > amount / 15 ? 0f : (amount % 15 == 0 ? 0f : amount / ((index + 1) * 15f) ));
            serumBars[index].color = (index < (int)(amount / 15) ? Color.green : this.darkGreen);
        }

        this.ampouleLever.localPosition = new Vector3(Mathf.Lerp(-900, -1200f, (amount / 75f)), 264f, 0f);
    }

    [SerializeField] private RectTransform ampouleTrigger;

    private bool isPlayingTrigger = false;
    private bool multipleTriggerInput = false;
    private bool isResettingTrigger = false;

    private float triggerTime = 0f;

    public void PlayTriggerAnim()
    {
        this.multipleTriggerInput = this.isPlayingTrigger;
        this.isPlayingTrigger = true;
    }

    private void UpdateTriggerAnimation()
    {
        if (!this.isPlayingTrigger) return;

        if (this.multipleTriggerInput)
        {
            this.triggerTime = 0f;
            this.isResettingTrigger = false;
            this.multipleTriggerInput = false;
        }

        this.triggerTime += Time.deltaTime * (this.isResettingTrigger ? -7 : 10);

        if (this.triggerTime <= 0f && this.isResettingTrigger)
        {
            this.triggerTime = 0f;
            this.isPlayingTrigger = false;
            this.isResettingTrigger = false;
            return;
        }

        if (this.triggerTime >= 1f)
        {
            this.triggerTime = 1f;
            this.isResettingTrigger = true;
            return;
        }

        this.ampouleTrigger.localPosition = new Vector3(Mathf.Lerp(-990f, -995f, this.triggerTime), Mathf.Lerp(370f, 390f, this.triggerTime), 0f);
        this.ampouleTrigger.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -15f, this.triggerTime));
    }

    // Backpack Animation Stuff

    [SerializeField] private Image pathogenBar;
    [SerializeField] private Image leukocyteBar;

    public void UpdateResourcesBackpack()
    {
        Doctor doc = this.roomManager.MyDoctor;

        pathogenBar.fillAmount = doc.PathogenAmount / doc.PathogenCap;
        leukocyteBar.fillAmount = doc.LeukocyteAmount / doc.LeukocyteCap;
    }

    /* Combat Doctor */
    public void OnCavaloButtonPress()
    {
        if (this.roomManager.MyDoctor.CavaloDeployed && (this.roomManager.MyPlayer.transform.position - this.roomManager.DeployedCavalo.transform.position).magnitude > roomManager.AmpouleInteractionReach) { return; }
        this.roomManager.MyDoctor.DeployCavaloInteraction();
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

    public void UpdateScoreDisplay()
    {
        this.scoreDisplay.text = $"SCORE: {this.roomManager.Score}";
    }


    // TESTE AAAAAA

    [SerializeField] private RectTransform morphineSpring;
    [SerializeField] private RectTransform vaccineSpring;

    private Vector3 morphineSpringPositionBuffer;
    private Vector3 vaccineSpringPositionBuffer;

    public void UpdateAmmoBackpack()
    {
        Doctor doc = this.roomManager.MyDoctor;

        morphineSpringPositionBuffer = morphineSpring.localPosition;
        morphineSpringPositionBuffer.y = Mathf.Lerp(-75f, 75f, (doc.MorphineAmount / doc.MorphineCap));
        morphineSpring.localPosition = morphineSpringPositionBuffer;

        vaccineSpringPositionBuffer = vaccineSpring.localPosition;
        vaccineSpringPositionBuffer.y = Mathf.Lerp(-75f, 75f, (doc.VaccineAmount / doc.VaccineCap));
        vaccineSpring.localPosition = vaccineSpringPositionBuffer;
    }

    [SerializeField] private Image cavaloSprite;

    [SerializeField] private Sprite cavaloOffSprite;
    [SerializeField] private Sprite[] cavaloOnSprite;

    private bool playCavaloAnimation = false;
    private int cavaloAnimationFrame = 0;
    private float cavaloAnimationFrameChangeRate = .25f; // in seconds
    private float cavaloAnimationTime = 1f;

    public void PlayCavaloAnimation()
    {
        this.playCavaloAnimation = this.roomManager.MyDoctor.CavaloDeployed;
        if (!this.playCavaloAnimation)
        {
            cavaloSprite.sprite = cavaloOffSprite;
            this.cavaloAnimationFrame = 0;
            this.cavaloAnimationTime = 1f;
        }
    }

    private void UpdateCavaloAnimation()
    {
        if (!this.playCavaloAnimation) return;

        this.cavaloAnimationTime += Time.deltaTime / this.cavaloAnimationFrameChangeRate;

        if (this.cavaloAnimationTime >= 1f)
        {
            this.cavaloAnimationTime = 0f;
            this.cavaloSprite.sprite = this.cavaloOnSprite[this.cavaloAnimationFrame];

            this.cavaloAnimationFrame++;
            if (this.cavaloAnimationFrame >= this.cavaloOnSprite.Length) { this.cavaloAnimationFrame = 0; }
        }
    }
}
