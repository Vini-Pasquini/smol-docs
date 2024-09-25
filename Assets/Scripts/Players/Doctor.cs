using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    private PhotonView photonView;
    private Rigidbody doctorRigidbody;
    private SpriteRenderer spriteRenderer;

    private float movementSpeed = 2f;
    private Vector3 newVelocity = Vector3.zero;

    DoctorType doctorType;

    RaycastHit hitInfo;

    // gathering doc
    public float leukocyteAmount { get; private set; } // generates vaccine
    public float pathogenAmount { get; private set; } // generates morphine
    public float shrinkSerumAmount { get; private set; }
    // combat doc
    public float morphineAmount { get; private set; } // kills leukocyte
    public float vaccineAmount { get; private set; } // kills pathogen
    public bool horseDeployed { get; private set; }
    public float horseCooldown { get; private set; }

    private void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.doctorRigidbody = this.GetComponent<Rigidbody>();
        this.spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();


        // gathering doc
        this.leukocyteAmount = 0f;
        this.pathogenAmount = 0f;
        this.shrinkSerumAmount = 0f;
        // combat doc
        this.morphineAmount = 10f;
        this.vaccineAmount = 10f;
        this.horseDeployed = false;
        this.horseCooldown = 0f;

        this.enabled = false; // until game starts
    }

    private void Update()
    {
        this.spriteRenderer.flipX = this.doctorRigidbody.velocity.x < 0f ? true : (this.doctorRigidbody.velocity.x > 0f ? false : this.spriteRenderer.flipX);

        if (!this.photonView.IsMine) return;

        newVelocity = Vector3.zero;

        newVelocity.x = movementSpeed * (Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0));
        newVelocity.y = movementSpeed * (Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0));

        this.doctorRigidbody.velocity = newVelocity;

        switch (doctorType)
        {
            case DoctorType.GatheringDoctor:
                GatheringDoctorUpdate();
                break;
            case DoctorType.CombatDoctor:
                CombatDoctorUpdate();
                break;
            default:
                break;
        }
    }

    public void DoctorInit(DoctorType inType, Vector3 spawnPosition)
    {
        this.doctorType = inType;
        this.spriteRenderer.sprite = (this.doctorType == DoctorType.GatheringDoctor ? Resources.Load<Sprite>("phGatheringDoctorSprite") : Resources.Load<Sprite>("phCombatDoctorSprite"));
        if (spawnPosition == Vector3.zero) return;
        this.transform.position = spawnPosition;
        RoomUIController.Instance.UpdateResourcesDisplay();
    }

    /* Gathering Doctor Stuff */

    public void GatheringDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.CompareTag("EnemyPile"))
            {
                Destroy(hitInfo.transform.gameObject);

                this.pathogenAmount++;

                RoomUIController.Instance.UpdateResourcesDisplay();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) // ph horse
        {
            RoomManager.Instance.myPhotonView.RPC("ReloadAmmo", RpcTarget.Others, this.pathogenAmount);
            this.pathogenAmount = 0;
        }
    }

    /* Combat Doctor Stuff */

    public void CombatDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.CompareTag("Enemy"))
            {
                RoomManager.Instance.myPlayerRPC.RPCKillEnemy(hitInfo.transform.position);
                Destroy(hitInfo.transform.gameObject);
                RoomManager.Instance.enemyAmount--; // ph

                this.vaccineAmount--;

                RoomUIController.Instance.UpdateResourcesDisplay();
            }
        }
    }

    public void AddAmmo(float ammoAmount)
    {
        this.vaccineAmount += ammoAmount;
    }
}
