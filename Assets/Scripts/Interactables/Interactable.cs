using ParrelSync.NonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private class InteractableDoctors
    {
        private Doctor[] memory;
        private int count;

        public int Count { get { return this.count; } }

        public InteractableDoctors()
        {
            this.memory = new Doctor[2];
            this.count = 0;
        }

        public bool Add(Doctor doctor)
        {
            if (this.count >= memory.Length) { return false; }
            this.memory[this.count++] = doctor;
            return true;
        }

        public bool Remove(Doctor doctor)
        {
            if (this.count <= 0) { return false; }
            if (this.memory[this.count - 1] == doctor)
            {
                this.count--;
                return true;
            }
            if (this.memory[0] == doctor)
            {
                this.memory[0] = this.memory[1];
                this.count--;
                return true;
            }
            return false;
        }

        public bool Has(Doctor doctor)
        {
            for (int index = 0; index < this.count; index++)
            {
                if (memory[index] == doctor) return true;
            }
            return false;
        }

        public bool isEmpty()
        {
            return this.count == 0;
        }
    }

    [SerializeField] protected GameObject interactionPopup;

    protected RoomManager roomManager;

    private InteractableDoctors doctorsInRange;

    protected void InteractableInit()
    {
        this.roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        this.doctorsInRange = new InteractableDoctors();
    }

    protected void InteractionUpdate()
    {
        if (!this.interactionPopup.activeSelf) { return; }

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (roomManager.MyDoctorType)
            {
                case DoctorType.GatheringDoctor:
                    this.GatheringDoctorInteraction();
                    break;
                case DoctorType.CombatDoctor:
                    this.CombatDoctorInteraction();
                    break;
            }
        }
    }

    protected virtual void GatheringDoctorInteraction()
    {
        Debug.Log("GATHERING DOC INTERACTION");
    }

    protected virtual void CombatDoctorInteraction()
    {
        Debug.Log("COMBAT DOC INTERACTION");
    }

    protected void OnTriggerEnterCallback(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.doctorsInRange.Add(other.GetComponent<Doctor>());
        }

        this.interactionPopup.SetActive(this.doctorsInRange.Has(this.roomManager.MyDoctor));
    }

    protected void OnTriggerExitCallback(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.doctorsInRange.Remove(other.GetComponent<Doctor>());
        }

        this.interactionPopup.SetActive(this.doctorsInRange.Has(this.roomManager.MyDoctor));
    }
}
