using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSample nowPlaying = AudioSample.None;

    [Header("Menu")]
    [SerializeField] private AudioClip menuOminousAudioClip;
    [SerializeField] private AudioClip menuPleasantAudioClip;
    
    [Header("Lobby")]
    [SerializeField] private AudioClip lobbyAudioClip;
    
    [Header("Level")]
    [SerializeField] private AudioClip levelAudioClip;
    [SerializeField] private AudioClip levelLoopAAudioClip;
    [SerializeField] private AudioClip levelLoopBAudioClip;

    [Header("Boss")]
    [SerializeField] private AudioClip bossAudioClip;
    [SerializeField] private AudioClip bossLoopAAudioClip;
    [SerializeField] private AudioClip bossLoopBAudioClip;

    [Header("Gameover")]
    [SerializeField] private AudioClip victoryAudioClip;
    [SerializeField] private AudioClip defeatAudioClip;

    private AudioSource _audioSource;

    private bool isIntense = false;

    private void Awake()
    {
        if (GameObject.FindObjectsByType<AudioManager>(FindObjectsSortMode.None).Length > 1) { GameObject.Destroy(this.gameObject); return; }
        GameObject.DontDestroyOnLoad(this);

        this._audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        this.PlayAudioClip(AudioSample.Menu);
    }

    private void Update()
    {
        this.transform.position = Camera.main.transform.position;

        if (!this._audioSource.isPlaying)
        {
            switch (this.nowPlaying)
            {
                case AudioSample.Defeat: case AudioSample.Victory: break;
                case AudioSample.Level:
                    this._audioSource.clip = (this.isIntense ? this.levelLoopBAudioClip : this.levelLoopAAudioClip);
                    break;
                case AudioSample.Boss:
                    this._audioSource.clip = (this.isIntense ? this.bossLoopBAudioClip : this.bossLoopAAudioClip);
                    break;
                default:
                    this._audioSource.loop = true;
                    this._audioSource.Play();
                    break;
            }
        }
    }

    public void PlayAudioClip(AudioSample audioSample)
    {
        switch (audioSample)
        {
            case AudioSample.None:
                break;
            case AudioSample.Menu:
                this._audioSource.clip = (Random.Range(0f, 1f) <= 0.5f ? this.menuPleasantAudioClip : this.menuOminousAudioClip);
                this._audioSource.loop = true;
                break;
            case AudioSample.Lobby:
                this._audioSource.clip = this.lobbyAudioClip;
                this._audioSource.loop = true;
                break;
            case AudioSample.Level:
                this._audioSource.clip = this.levelAudioClip;
                this._audioSource.loop = false;
                break;
            case AudioSample.Boss:
                this._audioSource.clip = this.bossAudioClip;
                this._audioSource.loop = false;
                break;
            case AudioSample.Victory:
                this._audioSource.clip = this.victoryAudioClip;
                this._audioSource.loop = false;
                break;
            case AudioSample.Defeat:
                this._audioSource.clip = this.defeatAudioClip;
                this._audioSource.loop = false;
                break;
        }
        this.nowPlaying = audioSample;
        this._audioSource.Play();
    }

    public void ToggleIntensity(bool isIntense)
    {
        this.isIntense = isIntense;
        this._audioSource.loop = false;
    }
}
