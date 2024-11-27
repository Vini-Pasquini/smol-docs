using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CapsuleController : Interactable
{
    private EntityManager _entityManager;
    private SpriteRenderer _spriteRenderer;
    private TextMeshPro phAmountDisplay;

    private int serumAmount = 200;

    private void Awake()
    {
        base.InteractableInit();
        phAmountDisplay = this.transform.GetChild(2).GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        this._spriteRenderer = this.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        this._entityManager = GameObject.Find("EntityManager").GetComponent<EntityManager>();

        this.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-15f, 15f)));
        this._spriteRenderer.sortingOrder = (int)(this.transform.position.y * -100f);
    }

    private void Update()
    {
        base.InteractionUpdate();
    }

    protected override void GatheringDoctorInteraction()
    {
        int difference = 75 - roomManager.MyDoctor.ShrinkSerumAmount;
        if (difference > this.serumAmount) difference = this.serumAmount;
        roomManager.MyDoctor.UpdateShrinkSerumAmount(difference);
        this.serumAmount -= difference;

        if (this.serumAmount <= 0) { this._spriteRenderer.sprite = this._entityManager.CapsuleEmpty; }
        else if (this.serumAmount <= 100) { this._spriteRenderer.sprite = this._entityManager.CapsuleMid; }

        phAmountDisplay.text = this.serumAmount.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnterCallback(other);
    }

    private void OnTriggerExit(Collider other)
    {
        base.OnTriggerExitCallback(other);
    }
}
