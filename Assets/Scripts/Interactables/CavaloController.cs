using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavaloController : Interactable
{
    private int _vaccineAmountStored;
    private int _morphineAmountStored;

    public int VaccineAmountStored { get { return _vaccineAmountStored; } }
    public int MorphineAmountStored { get { return _morphineAmountStored; } }

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
        int morphineAmount = (int)(roomManager.MyDoctor.LeukocyteAmount / 4f);
        int vaccineAmount = (int)(roomManager.MyDoctor.PathogenAmount / 4f);
        roomManager.MyDoctor.RemoveGatheringResources(morphineAmount * 4, vaccineAmount * 4);
        roomManager.MyPlayerRPC.RPCFillCavaloResources(morphineAmount, vaccineAmount);
    }

    protected override void CombatDoctorInteraction()
    {
        // adicionar cap dps, talvez
        roomManager.MyDoctor.AddCombatResources(this._morphineAmountStored, this._vaccineAmountStored);
        roomManager.MyPlayerRPC.RPCEmptyCavaloResources();
    }

    public void FillResources(int morphineAmount, int vaccineAmount)
    {
        this._morphineAmountStored += morphineAmount;
        this._vaccineAmountStored += vaccineAmount;
    }

    public void EmptyResources()
    {
        this._morphineAmountStored = 0;
        this._vaccineAmountStored = 0;
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
