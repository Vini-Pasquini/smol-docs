using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavaloController : MonoBehaviour
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

    [SerializeField] private GameObject interactionPopup;

    private InteractableDoctors doctorsInRange;

    private RoomManager roomManager;

    private void Awake()
    {
        this.doctorsInRange = new InteractableDoctors();
        this.roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    private void Start()
    {
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int)(this.transform.position.y * -100f);
    }

    private void Update()
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

    private void GatheringDoctorInteraction()
    {
        Debug.Log("DEPOSITOU RESTOS");
    }

    private void CombatDoctorInteraction()
    {
        Debug.Log("RESGATOU VACINAS");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bool flag = this.doctorsInRange.Add(other.GetComponent<Doctor>());
            Debug.Log(flag ? "Doctor Added" : "Add Failed");
        }

        this.interactionPopup.SetActive(this.doctorsInRange.Has(this.roomManager.MyDoctor));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bool flag = this.doctorsInRange.Remove(other.GetComponent<Doctor>());
            Debug.Log(flag ? "Doctor Removed" : "Remove Failed");
        }

        this.interactionPopup.SetActive(this.doctorsInRange.Has(this.roomManager.MyDoctor));
    }
}
