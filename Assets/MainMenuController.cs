using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject TitleScreenCanvas;
    [SerializeField] private GameObject GameLobbyCanvas;
    [Header("Game Title Animation")]
    [SerializeField] private Transform gameTitleTransform;
    [SerializeField] private Transform gameTitleLowLimit;
    [SerializeField] private Transform gameTitleHighLimit;
    // position
    private Vector3 gameTitleMinPosition;
    private Vector3 gameTitleMaxPosition;
    private bool gameTitlePositionAnimationInvertDirection = false;
    private float gameTitlePositionAnimationTimer = .5f;
    private float gameTitlePositionAnimationLength = 2f;
    //rotation
    private float gameTitleRotationLimit = 7f;
    private bool gameTitleRotationAnimationInvertDirection = false;
    private float gameTitleRotationAnimationTimer = .5f;
    private float gameTitleRotationAnimationLength = 4.5f;

    private void Start()
    {
        gameTitleMaxPosition = gameTitleHighLimit.position;
        gameTitleMinPosition = gameTitleLowLimit.position;
    }

    private void Update()
    {
        // position
        gameTitleTransform.position = new Vector3(gameTitleTransform.position.x, Mathf.Lerp(gameTitleMinPosition.y, gameTitleMaxPosition.y, gameTitlePositionAnimationTimer), 0f);
        gameTitlePositionAnimationTimer += (Time.deltaTime / gameTitlePositionAnimationLength) * (gameTitlePositionAnimationInvertDirection ? -1 : 1);
        if (gameTitlePositionAnimationTimer <= 0f || gameTitlePositionAnimationTimer >= 1f) { gameTitlePositionAnimationInvertDirection = !gameTitlePositionAnimationInvertDirection; }
        // rotation
        gameTitleTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Lerp(-gameTitleRotationLimit, gameTitleRotationLimit, gameTitleRotationAnimationTimer)));
        gameTitleRotationAnimationTimer += (Time.deltaTime / gameTitleRotationAnimationLength) * (gameTitleRotationAnimationInvertDirection ? -1 : 1);
        if (gameTitleRotationAnimationTimer <= 0f || gameTitleRotationAnimationTimer >= 1f) { gameTitleRotationAnimationInvertDirection = !gameTitleRotationAnimationInvertDirection; }
    }

    /* Menu Interaction Methods */
    // TitleScreen
    public void PlayButton()
    {
        TitleScreenCanvas.SetActive(false);
        GameLobbyCanvas.SetActive(true);
    }
    // GameLobby
    public void BackButton()
    {
        TitleScreenCanvas.SetActive(true);
        GameLobbyCanvas.SetActive(false);
    }
}
