using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] falas;

    [SerializeField] private int index = 0;

    private void Start()
    {
        falas[index++].enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (index >=  falas.Length - 1) { SceneManager.LoadScene("MainMenu"); return; }
            falas[index++].enabled = true;
            if (index >= falas.Length - 1) { falas[falas.Length - 1].text = "Começar..."; }
        }
    }
}
