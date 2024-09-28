using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Doctor : MonoBehaviour // TODO: me livrar do mono
{
    private RoomManager roomManager;
    private RoomUIController roomUIController;

    [SerializeField] private AnimatorController combatDoctorAnimatorController;
    [SerializeField] private AnimatorController gatheringDoctorAnimatorController;

    private PhotonView photonView;
    private Rigidbody doctorRigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator doctorAnimator;

    private float movementSpeed = 3f;
    private Vector3 newVelocity = Vector3.zero;

    DoctorType doctorType;

    RaycastHit hitInfo;

    // gathering doc
    private float _leukocyteAmount;  // generates vaccine
    public float LeukocyteAmount { get { return this._leukocyteAmount; } }
    private float _pathogenAmount; // generates morphine
    public float PathogenAmount { get { return this._pathogenAmount; } }
    private float _shrinkSerumAmount;
    public float ShrinkSerumAmount { get { return this._shrinkSerumAmount; } }
    // combat doc
    private float _morphineAmount; // kills leukocyte
    public float MorphineAmount { get { return this._morphineAmount; } }
    private float _vaccineAmount; // kills pathogen
    public float VaccineAmount { get { return this._vaccineAmount; } }
    private bool _horseDeployed;
    public bool HorseDeployed { get { return this._horseDeployed; } }
    private float _horseCooldown;
    public float HorseCooldown { get { return this._horseCooldown; } }

    private void Awake()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
    }

    private void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.doctorRigidbody = this.GetComponent<Rigidbody>();
        this.spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.doctorAnimator = this.transform.GetChild(0).GetComponent<Animator>();

        // gathering doc
        this._leukocyteAmount = 0f;
        this._pathogenAmount = 0f;
        this._shrinkSerumAmount = 0f;
        // combat doc
        this._morphineAmount = 0f;
        this._vaccineAmount = 0f;
        this._horseDeployed = false;
        this._horseCooldown = 0f;

        this.enabled = false; // until game starts
    }

    private void Update()
    {
        this.spriteRenderer.sortingOrder = (int)(this.transform.position.y * -100f);
        this.spriteRenderer.flipX = this.doctorRigidbody.velocity.x < 0f ? true : (this.doctorRigidbody.velocity.x > 0f ? false : this.spriteRenderer.flipX);

        if (this.doctorRigidbody.velocity.magnitude > .1f) // ph
        {
            this.doctorAnimator.Play("RUN");
        }
        else
        {
            this.doctorAnimator.Play("IDLE");
        }

        if (!this.photonView.IsMine) return;

        newVelocity = Vector3.zero;

        newVelocity.x = movementSpeed * (Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f));
        newVelocity.y = movementSpeed * (Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f));

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
        this.doctorAnimator.runtimeAnimatorController = (this.doctorType == DoctorType.GatheringDoctor ? this.gatheringDoctorAnimatorController : this.combatDoctorAnimatorController);
        if (spawnPosition == Vector3.zero) return;
        this.transform.position = spawnPosition;

        // gathering doc
        this._leukocyteAmount = 0f;
        this._pathogenAmount = 0f;
        this._shrinkSerumAmount = 0f;
        // combat doc
        this._morphineAmount = 0f;
        this._vaccineAmount = 0f;
        this._horseDeployed = false;
        this._horseCooldown = 0f;

        roomUIController.UpdateResourcesDisplay();
    }

    public void DoctorReset()
    {
        this.doctorType = DoctorType.None;
        this.transform.position = Vector3.forward * -20f;
        
        this.enabled = false; // until game starts again
    }

    /* Gathering Doctor Stuff */

    public void GatheringDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.CompareTag("EnemyPile"))
            {
                Destroy(hitInfo.transform.gameObject);

                this._pathogenAmount++;

                roomUIController.UpdateResourcesDisplay();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) // ph horse
        {
            roomManager.MyPhotonView.RPC("ReloadAmmo", RpcTarget.Others, this._pathogenAmount);
            this._pathogenAmount = 0;
        }
    }

    /* Combat Doctor Stuff */

    public void CombatDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.CompareTag("Enemy"))
            {
                roomManager.MyPlayerRPC.RPCKillEnemy(hitInfo.transform.position);
                Destroy(hitInfo.transform.gameObject);
                roomManager.UpdateEnemyAmount(-1);

                this._vaccineAmount--;

                roomUIController.UpdateResourcesDisplay();
            }
        }
    }

    public void AddAmmo(float ammoAmount)
    {
        this._vaccineAmount += ammoAmount;
    }
}
