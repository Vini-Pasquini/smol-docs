using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CapsuleController : Interactable
{
    private TextMeshPro phAmountDisplay;

    private int serumAmount = 200;

    private void Awake()
    {
        base.InteractableInit();
        phAmountDisplay = this.transform.GetChild(2).GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int)(this.transform.position.y * -100f);
    }

    private void Update()
    {
        base.InteractionUpdate();
    }

    protected override void GatheringDoctorInteraction()
    {
        Debug.Log("COLETOU SORO");

        int difference = 75 - roomManager.MyDoctor.ShrinkSerumAmount;
        if (difference > this.serumAmount) difference = this.serumAmount;
        roomManager.MyDoctor.UpdateShrinkSerumAmount(difference);
        this.serumAmount -= difference;

        phAmountDisplay.text = this.serumAmount.ToString();

        base.GatheringDoctorInteraction();
    }

    protected override void CombatDoctorInteraction()
    {
        Debug.Log("TU NAO PEGA SORO");
        base.CombatDoctorInteraction();
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
