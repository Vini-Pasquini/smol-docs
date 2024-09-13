using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public GameObject myPlayer { get; private set; }
    public PlayerRPC myPlayerRPC { get; private set; }
    public PhotonView myPhotonView { get; private set; }

    public DoctorType myDoctor { get; private set; }
    public DoctorType otherDoctor { get; private set; }

    public bool runningLevel { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        myPlayer = PhotonNetwork.Instantiate("Doctor", Vector3.zero, Quaternion.identity);
        myPlayerRPC = myPlayer.GetComponent<PlayerRPC>();
        myPhotonView = myPlayer.GetComponent<PhotonView>();
        
        myDoctor = DoctorType.None;
        otherDoctor = DoctorType.None;

        runningLevel = false;
    }

    public void UpdateDoctorType(DoctorType newDoctorType, bool otherDoctor = false)
    {
        if (otherDoctor) { this.otherDoctor = (this.otherDoctor == DoctorType.None || this.otherDoctor != newDoctorType ? newDoctorType : DoctorType.None); }
        else { this.myDoctor = (this.myDoctor == DoctorType.None || this.myDoctor != newDoctorType ? newDoctorType : DoctorType.None); }

        RoomUIController.Instance.UpdateSelectedDoctorsPanel();
    }

    public void StartGame()
    {
        this.runningLevel = true;
        myPlayer.GetComponent<Doctor>().enabled = true;
    }

    //public void DEV_TEST()
    //{
    //    myPlayer = PhotonNetwork.Instantiate("Doctor", Vector3.zero, Quaternion.identity);
    //    myPlayerRPC = myPlayer.GetComponent<PlayerRPC>();
    //    myPhotonView = myPlayer.GetComponent<PhotonView>();
    //
    //    myDoctor = DoctorType.None;
    //    otherDoctor = DoctorType.None;
    //}
}
