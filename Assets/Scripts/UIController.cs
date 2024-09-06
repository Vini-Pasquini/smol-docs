using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private TextMeshProUGUI userList;
    [SerializeField] public TextMeshProUGUI messagesContainer;
    [SerializeField] public TextMeshProUGUI inputMessage;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayerList()
    {
        string list = "Player List:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            list += $"{player.NickName}\n";
        }
        userList.text = list;
    }
}
