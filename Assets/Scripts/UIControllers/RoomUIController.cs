using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIController : MonoBehaviour
{
    private RoomManager roomManager;

    [Header("Canvas")]
    [SerializeField] private GameObject roomLobbyCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject gatheringDocCanvas; // might change later
    [SerializeField] private GameObject combatDocCanvas; // might change later

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

    [Header("Doctor Resources")]
    [SerializeField] private TextMeshProUGUI resourcesDisplay;

    private Sprite[] playerOverlay;

    private void Awake()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    private void Start()
    {
        messageContainer.text = string.Empty;
        playerOverlay = Resources.LoadAll<Sprite>("PlayerOverlay");
    }

    /* Doctor Selection */
    public void UpdateWaitingForPlayersOverlay()
    {
        waitingForPlayersOverlay.SetActive(PhotonNetwork.CurrentRoom.PlayerCount <= 1);
        roomManager.ResetDoctorType();
    }

    public void OnDoctorSelectButtonPress(int type)
    {
        DoctorType selectedType = (DoctorType)type;

        roomManager.MyPlayerRPC.RPCSelectDoctor(selectedType);
        roomManager.UpdateDoctorType(selectedType, false);
    }

    public void UpdateSelectedDoctorsPanel()
    {
        DoctorType myDoctorType = roomManager.MyDoctorType;
        DoctorType otherDoctorType = roomManager.OtherDoctorType;

        gatheringDocPlayerOverlay.sprite = playerOverlay[(myDoctorType == DoctorType.GatheringDoctor && otherDoctorType == DoctorType.GatheringDoctor ? 3 : (myDoctorType != DoctorType.GatheringDoctor && otherDoctorType == DoctorType.GatheringDoctor ? 2 : (myDoctorType == DoctorType.GatheringDoctor && otherDoctorType != DoctorType.GatheringDoctor ? 1 : 0)))];
        combatDocPlayerOverlay.sprite = playerOverlay[(myDoctorType == DoctorType.CombatDoctor && otherDoctorType == DoctorType.CombatDoctor ? 3 : (myDoctorType != DoctorType.CombatDoctor && otherDoctorType == DoctorType.CombatDoctor ? 2 : (myDoctorType == DoctorType.CombatDoctor && otherDoctorType != DoctorType.CombatDoctor ? 1 : 0)))];

        gatheringDocSprite.color = new Color(gatheringDocSprite.color.r, gatheringDocSprite.color.g, gatheringDocSprite.color.b, (myDoctorType != DoctorType.GatheringDoctor && otherDoctorType != DoctorType.GatheringDoctor ? .125f : 1f));
        combatDocSprite.color = new Color(combatDocSprite.color.r, combatDocSprite.color.g, combatDocSprite.color.b, (myDoctorType != DoctorType.CombatDoctor && otherDoctorType != DoctorType.CombatDoctor ? .125f : 1f));

        // TODO: mudar interactable do botao pra apenas quando ambos os jogadores tiverem doutores diferentes selecionados
        readyButton.interactable = myDoctorType != DoctorType.None && otherDoctorType != DoctorType.None && myDoctorType != otherDoctorType;
        roomManager.ResetPlayerReady();
    }

    /* Chat */
    public void WriteToChat(string newLine)
    {
        messageContainer.text += $"{newLine}\n";
    }

    public void OnSendMessageButtonPress()
    {
        roomManager.MyPlayerRPC.RPCSendMessage(messageInputField.text);
        this.WriteToChat($"me: {messageInputField.text}");
        messageInputField.text = string.Empty;
    }

    public void OnRecieveMessageCallback(string newMessage)
    {
        this.WriteToChat($"they: {newMessage}");
    }

    /* Ready System */
    public void OnReadyButtonPress()
    {
        roomManager.MyPlayerRPC.RPCSetReady(!roomManager.MyPlayerReady);
        roomManager.UpdatePlayerReady(!roomManager.MyPlayerReady, false);
    }

    public void UpdateReadyButton()
    {
        bool myPlayerReady = roomManager.MyPlayerReady;
        bool otherPlayerReady = roomManager.OtherPlayerReady;

        readyButtonText.text = (myPlayerReady && otherPlayerReady ? "STARTING..." : (myPlayerReady ? "[X] READY" : "[ ] READY"));
        readyButton.image.color = (myPlayerReady && otherPlayerReady ? Color.green : (otherPlayerReady ? Color.yellow : Color.white));
    }

    public void ToggleLobbyCanvas(bool toggleOn)
    {
        roomLobbyCanvas.SetActive(toggleOn);
        gameplayCanvas.SetActive(!toggleOn);
    }

    /* Resources */
    public void UpdateResourcesDisplay() // mudar dps
    {
        Doctor doc = roomManager.MyDoctor;
        if (roomManager.MyDoctorType == DoctorType.GatheringDoctor)
        {
            resourcesDisplay.text = $" Leucocitos: {doc.LeukocyteAmount} / ---\n Patogenos: {doc.PathogenAmount} / ---\n Soro de Encolhimento: {doc.ShrinkSerumAmount}";
            return;
        }
        resourcesDisplay.text = $" Morfina: {doc.MorphineAmount} / ---\n Vacina: {doc.VaccineAmount} / ---\n C.A.V.A.L.O.: {doc.HorseCooldown}";
        return;
    }
}
