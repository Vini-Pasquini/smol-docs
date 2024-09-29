using Photon.Pun;
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
    private SpriteRenderer doctorSpriteRenderer;
    private Animator doctorAnimator;

    private float movementSpeed = 3f;
    private Vector3 newVelocity = Vector3.zero;

    private const float scaleCooldown = 2f;
    private float scaleTimer = 1f;
    private float scaleFactor = .1f;

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
        this.roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        this.roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
    }

    private void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.doctorRigidbody = this.GetComponent<Rigidbody>();
        this.doctorSpriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
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
        this.doctorSpriteRenderer.sortingOrder = (int)(this.transform.position.y * -100f);
        this.doctorSpriteRenderer.flipX = this.doctorRigidbody.velocity.x < 0f ? true : (this.doctorRigidbody.velocity.x > 0f ? false : this.doctorSpriteRenderer.flipX);

        // ph
        if (this.doctorRigidbody.velocity.magnitude > .1f) { this.doctorAnimator.Play("RUN"); }
        else { this.doctorAnimator.Play("IDLE"); }

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

        if (this.scaleTimer <= 0f)
        {
            this.transform.localScale += Vector3.one * this.scaleFactor;
            this.scaleTimer = scaleCooldown;
        }

        this.scaleTimer -= Time.deltaTime;
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
        this.transform.localScale = Vector3.one;
        
        this.enabled = false; // until game starts again
    }

    /* Gathering Doctor Stuff */

    public void GatheringDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.CompareTag("EnemyPile") && (roomManager.MyPlayer.transform.position - hitInfo.transform.position).magnitude <= roomManager.InteractionReach)
            {
                Destroy(hitInfo.transform.gameObject);

                switch (hitInfo.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name) // gambeta, mudar dps pelo EnemyType do scriptable object
                {
                    case "phLeukocyteRemains":
                        this._leukocyteAmount++;
                        break;
                    case "phPathogenVirusRemains": case "phPathogenBacteriaRemains":
                        this._pathogenAmount++;
                        break;
                }

                roomUIController.UpdateResourcesDisplay();
            }
        }
    }

    /* Combat Doctor Stuff */

    public void CombatDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (hitInfo.transform.CompareTag("Enemy") && (roomManager.MyPlayer.transform.position - hitInfo.transform.position).magnitude <= roomManager.InteractionReach)
            {
                EnemyType enemyType = hitInfo.transform.GetComponent<EnemyController>().EnemyType;
                roomManager.MyPlayerRPC.RPCKillEnemy(hitInfo.transform.position, enemyType);
                Destroy(hitInfo.transform.gameObject);
                roomManager.UpdateEnemyAmount(-1);

                switch (enemyType)
                {
                    case EnemyType.Leukocyte:
                        this._morphineAmount--;
                        break;
                    case EnemyType.PathogenVirus: case EnemyType.PathogenBacteria:
                        this._vaccineAmount--;
                        break;
                }

                roomUIController.UpdateResourcesDisplay();
            }
        }
    }

    public void AddAmmo(float morphine = 0f, float vaccine = 0f)
    {
        this._morphineAmount += morphine;
        this._vaccineAmount += vaccine;

        roomUIController.UpdateResourcesDisplay();
    }

    public void ResetResources()
    {
        this._leukocyteAmount = this._pathogenAmount = 0f;

        roomUIController.UpdateResourcesDisplay();
    }
}
