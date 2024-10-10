using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : Interactable
{
    private void Awake()
    {
        base.InteractableInit();
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
