using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIController : MonoBehaviour
{
    public static RoomUIController Instance { get; private set; }

    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private TextMeshProUGUI messageContainer;

    [SerializeField] private Image gatheringDocSprite;
    [SerializeField] private Image combatDocSprite;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        messageContainer.text = string.Empty;
    }

    public void FlipSpriteAlpha(Image doctorSprite)
    {
        doctorSprite.color = new Color(doctorSprite.color.r, doctorSprite.color.g, doctorSprite.color.b, (doctorSprite.color.a >= 1f ? .5f : 1f));
    }

    /*  */
    public void OnDoctorSelectButtonPress(int type)
    {
        DoctorType selectedType = (DoctorType)type;

        RoomManager.Instance.myPlayerRPC.RPCSelectDoctor(selectedType);
        RoomManager.Instance.myDoctor = (RoomManager.Instance.myDoctor == DoctorType.None || RoomManager.Instance.myDoctor != selectedType ? selectedType : DoctorType.None);
        switch (selectedType)
        {
            case DoctorType.GatheringDoctor:
                this.FlipSpriteAlpha(gatheringDocSprite);
                break;
            case DoctorType.CombatDoctor:
                this.FlipSpriteAlpha(combatDocSprite);
                break;
        }
    }

    public void OnDoctorSelectedCallback(DoctorType selectedType)
    {
        Debug.Log($"o amiguinho selecionou o boniquinho: {selectedType}");
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
}
