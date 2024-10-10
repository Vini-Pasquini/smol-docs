using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavaloController : Interactable
{
    private int vaccineAmountStored;
    private int morphineAmountStored;

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
        this.InteractionUpdate();
    }

    protected override void GatheringDoctorInteraction()
    {
        Debug.Log("DEPOSITOU RESTOS");
        int morphineAmount = (int)(roomManager.MyDoctor.LeukocyteAmount / 4f);
        int vaccineAmount = (int)(roomManager.MyDoctor.PathogenAmount / 4f);
        roomManager.MyDoctor.RemoveGatheringResources(morphineAmount * 4, vaccineAmount * 4);
        roomManager.MyPlayerRPC.RPCFillCavaloResources(morphineAmount, vaccineAmount);
        base.GatheringDoctorInteraction();
    }

    protected override void CombatDoctorInteraction()
    {
        Debug.Log("RESGATOU VACINAS");
        // adicionar cap dps
        roomManager.MyDoctor.AddCombatResources(this.morphineAmountStored, this.vaccineAmountStored);
        roomManager.MyPlayerRPC.RPCEmptyCavaloResources();
        base.CombatDoctorInteraction();
    }

    public void FillResources(int morphineAmount, int vaccineAmount)
    {
        this.morphineAmountStored += morphineAmount;
        this.vaccineAmountStored += vaccineAmount;
    }

    public void EmptyResources()
    {
        this.morphineAmountStored = 0;
        this.vaccineAmountStored = 0;
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
