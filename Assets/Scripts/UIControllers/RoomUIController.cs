using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIController : MonoBehaviour
{
    public static RoomUIController Instance { get; private set; }

    [Header("Chat")]
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private TextMeshProUGUI messageContainer;

    [Header("Doctor Selector")]
    [SerializeField] private Image gatheringDocSprite;
    [SerializeField] private Image gatheringDocPlayerOverlay;
    [SerializeField] private Image combatDocSprite;
    [SerializeField] private Image combatDocPlayerOverlay;

    private Sprite[] playerOverlay;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        messageContainer.text = string.Empty;
        playerOverlay = Resources.LoadAll<Sprite>("PlayerOverlay");
    }

    /* Doctor Selection */
    public void OnDoctorSelectButtonPress(int type)
    {
        DoctorType selectedType = (DoctorType)type;

        RoomManager.Instance.myPlayerRPC.RPCSelectDoctor(selectedType);
        RoomManager.Instance.UpdateDoctorType(selectedType, false);
    }

    public void OnOtherDoctorSelectedCallback(DoctorType selectedType)
    {
        RoomManager.Instance.UpdateDoctorType(selectedType, true);
    }

    public void UpdateSelectedDoctorsPanel()
    {
        gatheringDocPlayerOverlay.sprite = playerOverlay[(RoomManager.Instance.myDoctor == DoctorType.GatheringDoctor && RoomManager.Instance.otherDoctor == DoctorType.GatheringDoctor ? 3 : (RoomManager.Instance.myDoctor != DoctorType.GatheringDoctor && RoomManager.Instance.otherDoctor == DoctorType.GatheringDoctor ? 2 : (RoomManager.Instance.myDoctor == DoctorType.GatheringDoctor && RoomManager.Instance.otherDoctor != DoctorType.GatheringDoctor ? 1 : 0)))];
        combatDocPlayerOverlay.sprite = playerOverlay[(RoomManager.Instance.myDoctor == DoctorType.CombatDoctor && RoomManager.Instance.otherDoctor == DoctorType.CombatDoctor ? 3 : (RoomManager.Instance.myDoctor != DoctorType.CombatDoctor && RoomManager.Instance.otherDoctor == DoctorType.CombatDoctor ? 2 : (RoomManager.Instance.myDoctor == DoctorType.CombatDoctor && RoomManager.Instance.otherDoctor != DoctorType.CombatDoctor ? 1 : 0)))];

        gatheringDocSprite.color = new Color(gatheringDocSprite.color.r, gatheringDocSprite.color.g, gatheringDocSprite.color.b, (RoomManager.Instance.myDoctor != DoctorType.GatheringDoctor && RoomManager.Instance.otherDoctor != DoctorType.GatheringDoctor ? .125f : 1f));
        combatDocSprite.color = new Color(combatDocSprite.color.r, combatDocSprite.color.g, combatDocSprite.color.b, (RoomManager.Instance.myDoctor != DoctorType.CombatDoctor && RoomManager.Instance.otherDoctor != DoctorType.CombatDoctor ? .125f : 1f));
    }

    /* Chat */
    public void WriteToChat(string newLine)
    {
        messageContainer.text += $"{newLine}\n";
    }

    public void OnSendMessageButtonPress()
    {
        RoomManager.Instance.myPlayerRPC.RPCSendMessage(messageInputField.text);
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
        RoomManager.Instance.StartGame();
    }
}
