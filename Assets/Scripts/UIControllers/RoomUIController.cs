using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomUIController : MonoBehaviour
{
    public static RoomUIController Instance { get; private set; }

    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private TextMeshProUGUI messageContainer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        messageContainer.text = string.Empty;
    }

    /* Chat */
    public void WriteToChat(string newLine)
    {
        messageContainer.text += $"{newLine}\n";
    }

    public void OnSendMessageButtonPress()
    {
        PhotonManager.Instance.RPCSendMessage(messageInputField.text);
        this.WriteToChat($"me: {messageInputField.text}");
        messageInputField.text = string.Empty;
    }

    public void OnRecieveMessageCallback(string newMessage)
    {
        this.WriteToChat($"they: {newMessage}");
    }
}
