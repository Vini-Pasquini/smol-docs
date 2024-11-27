using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreloadUIController : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI[] falas;
    private int audioIndex = -1;
    private int spriteIndex = 0;

    private AudioSource _audioSource;

    [SerializeField] private TextMeshProUGUI message;

    [SerializeField] private Image characterA;
    [SerializeField] private Image characterB;
    [SerializeField] private Image hologram;

    [SerializeField] private Sprite[] hologramSprites;

    [SerializeField] private AudioClip[] dialogClips;

    private void Start()
    {
        // falas[index++].enabled = true;
        this._audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!this._audioSource.isPlaying)
        {
            characterA.transform.localScale = Vector3.one * .9f;
            characterA.color = new Color(1f, 1f, 1f, .75f);

            characterB.transform.localScale = Vector3.one * .9f;
            characterB.color = new Color(1f, 1f, 1f, .75f);
        }

        if (this.audioIndex == 1 && !this._audioSource.isPlaying)
        {
            DialogStep();
        }
        
        if (audioIndex >= dialogClips.Length - 1 && !this._audioSource.isPlaying)
        {
            SceneManager.LoadScene("MainMenu");
        }

        // 0 - 0
        // 1 - 4
        // 2 - 5
        // 3 - 7
        // 4 - 9
        // 5 - 11

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (index >=  falas.Length - 1) { SceneManager.LoadScene("MainMenu"); return; }
            //falas[index++].enabled = true;
            //if (index >= falas.Length - 1) { falas[falas.Length - 1].text = "Começar..."; }

            DialogStep();
            switch(audioIndex)
            {
                case 4: case 5: case 7: case 9: case 11:
                    spriteIndex++;
                    if (spriteIndex >= hologramSprites.Length) { spriteIndex = hologramSprites.Length - 1; }
                    break;
            }
            hologram.sprite = this.hologramSprites[spriteIndex];
        }
    }

    private void DialogStep()
    {
        if (audioIndex >= dialogClips.Length - 1) { return; }

        this._audioSource.clip = dialogClips[++audioIndex];
        this._audioSource.Play();

        if (audioIndex >= dialogClips.Length - 1) { message.text = "Começando..."; }

        characterA.transform.localScale = Vector3.one * (audioIndex % 2 == 0 ? 1f : .9f);
        characterA.color = new Color(1f, 1f, 1f, (audioIndex % 2 == 0 ? 1f : .75f));

        characterB.transform.localScale = Vector3.one * (audioIndex % 2 != 0 ? 1f : .9f);
        characterB.color = new Color(1f, 1f, 1f, (audioIndex % 2 != 0 ? 1f : .75f));
    }
}
