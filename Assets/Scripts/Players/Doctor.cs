using Photon.Pun;
using UnityEngine;

public class Doctor : MonoBehaviour // TODO: me livrar do mono
{
    private RoomManager roomManager;
    private RoomUIController roomUIController;
    private EnemyManager enemyManager;

    [SerializeField] private RuntimeAnimatorController combatDoctorAnimatorController;
    [SerializeField] private RuntimeAnimatorController gatheringDoctorAnimatorController;

    private PhotonView photonView;
    private Rigidbody doctorRigidbody;

    private SpriteRenderer doctorSpriteRenderer;
    private Animator doctorAnimator;

    private AudioSource audioSource;
    
    private float movementSpeed = 3f;
    private Vector3 newVelocity = Vector3.zero;

    private const float scaleCooldown = 2f;
    private float scaleTimer = 1f;
    private float scaleFactor = .1f;

    public int individualScore;

    DoctorType doctorType;

    RaycastHit hitInfo;

    private int layerMask = (1 << 30) | (1 << 31);

    // gathering doc
    private int _leukocyteAmount = 0;  // generates vaccine
    public int LeukocyteAmount { get { return this._leukocyteAmount; } }
    private int _pathogenAmount = 0; // generates morphine
    public int PathogenAmount { get { return this._pathogenAmount; } }
    private int _shrinkSerumAmount = 75; // comes with a full charge
    public int ShrinkSerumAmount { get { return this._shrinkSerumAmount; } }

    // combat doc
    private int _morphineAmount = 100; // kills leukocyte
    public int MorphineAmount { get { return this._morphineAmount; } }
    private int _vaccineAmount = 100; // kills pathogen
    public int VaccineAmount { get { return this._vaccineAmount; } }
    private bool _cavaloDeployed = false;
    public bool CavaloDeployed { get { return this._cavaloDeployed; } }
    private float _cavaloCooldownTimer = 0f;
    public float CavaloCooldownTimer { get { return this._cavaloCooldownTimer; } }
    private float _cavaloCooldown = 30f;

    private void Awake()
    {
        this.roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        this.roomUIController = GameObject.Find("RoomUIController").GetComponent<RoomUIController>();
        this.enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }

    private void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.doctorRigidbody = this.GetComponent<Rigidbody>();

        this.doctorSpriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.doctorAnimator = this.transform.GetChild(0).GetComponent<Animator>();

        this.audioSource = this.transform.GetChild(2).GetComponent<AudioSource>();

        this.individualScore = 0;

        this.enabled = false; // until game starts
    }

    private void Update()
    {
        this.doctorSpriteRenderer.sortingOrder = (int)(this.transform.position.y * -100f);
        this.doctorSpriteRenderer.flipX = this.doctorRigidbody.velocity.x < 0f ? true : (this.doctorRigidbody.velocity.x > 0f ? false : this.doctorSpriteRenderer.flipX);

        // ph
        if (this.doctorRigidbody.velocity.magnitude > .1f)
        {
            this.doctorAnimator.Play("RUN");
            this.audioSource.UnPause();
        }
        else
        {
            this.doctorAnimator.Play("IDLE");
            this.audioSource.Pause();
        }

        if (!this.photonView.IsMine) return;

        // ph
        Vector3 cameraPosition = this.transform.position;
        cameraPosition.y += this.transform.localScale.y; // sim, especfico pra caramba
        cameraPosition.z = -10f;
        Camera.main.transform.position = cameraPosition;

        this.newVelocity = Vector3.zero;

        this.newVelocity.x = this.movementSpeed * (Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f));
        this.newVelocity.y = this.movementSpeed * (Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f));

        this.doctorRigidbody.velocity = this.newVelocity;

        switch (doctorType)
        {
            case DoctorType.GatheringDoctor:
                this.GatheringDoctorUpdate();
                break;
            case DoctorType.CombatDoctor:
                this.CombatDoctorUpdate();
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

        this.time += Time.deltaTime;
        if (this.time >= 1f && this.individualScore > 0)
        {
            this.roomManager.MyPlayerRPC.RPCSendScoreUpdate(this.individualScore);
            this.individualScore = 0;
            this.time = 0f;
        }
    }

    float time = 0;

    public void DoctorInit(DoctorType inType, Vector3 spawnPosition)
    {
        this.doctorType = inType;
        this.doctorAnimator.runtimeAnimatorController = (this.doctorType == DoctorType.GatheringDoctor ? this.gatheringDoctorAnimatorController : this.combatDoctorAnimatorController);
        if (spawnPosition == Vector3.zero) return;
        this.transform.position = spawnPosition;

        // gathering doc
        this._leukocyteAmount = 0;
        this._pathogenAmount = 0;
        this._shrinkSerumAmount = 15;

        // combat doc
        this._morphineAmount = 100;
        this._vaccineAmount = 100;
        this._cavaloDeployed = false;
        this._cavaloCooldownTimer = 0f;

        this.roomUIController.UpdateResourcesDisplay(); // TODO: remover

        this.roomUIController.UpdateAmpoule();
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
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out this.hitInfo, float.MaxValue, layerMask))
        {
            if (this.hitInfo.transform.CompareTag("EnemyPile") && (this.roomManager.MyPlayer.transform.position - this.hitInfo.transform.position).magnitude <= this.roomManager.InteractionReach)
            {
                Destroy(this.hitInfo.transform.gameObject);

                switch (this.hitInfo.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name) // gambeta, mudar dps pelo EnemyType do scriptable object
                {
                    case "phLeukocyteRemains":
                        this._leukocyteAmount += Random.Range(1, 4);
                        break;
                    case "phPathogenVirusRemains": case "phPathogenBacteriaRemains":
                        this._pathogenAmount += Random.Range(1, 4);
                        break;
                }

                this.individualScore += 3;

                this.roomUIController.UpdateResourcesDisplay();
            }
        }
    }

    public void RemoveGatheringResources(int leukocyteAmount = 0, int pathogenAmount = 0)
    {
        // verificar cap dps
        this._leukocyteAmount -= leukocyteAmount;
        this._pathogenAmount -= pathogenAmount;

        this.roomUIController.UpdateResourcesDisplay();
    }

    public void UpdateShrinkSerumAmount(int increment)
    {
        this._shrinkSerumAmount += increment;

        if (this._shrinkSerumAmount > 75) this._shrinkSerumAmount = 75;
        if (this._shrinkSerumAmount < 0) this._shrinkSerumAmount = 0;

        this.roomUIController.UpdateResourcesDisplay(); // TODO: remover

        this.roomUIController.UpdateAmpoule();
    }

    /* Combat Doctor Stuff */

    public void CombatDoctorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out this.hitInfo, float.MaxValue, layerMask))
        {
            if (this.hitInfo.transform.CompareTag("Enemy") && (this.roomManager.MyPlayer.transform.position - this.hitInfo.transform.position).magnitude <= this.roomManager.InteractionReach)
            {
                Vector3 killPosition = this.hitInfo.transform.position;

                EnemyType enemyType = this.enemyManager.KillEnemy(this.hitInfo.transform.gameObject);

                this.roomManager.MyPlayerRPC.RPCKillEnemy(killPosition, enemyType);

                switch (enemyType)
                {
                    case EnemyType.Leukocyte:
                        this._morphineAmount--;
                        break;
                    case EnemyType.PathogenVirus: case EnemyType.PathogenBacteria:
                        this._vaccineAmount--;
                        break;
                }

                this.individualScore += 7;

                this.roomUIController.UpdateResourcesDisplay();
            }
        }
    }

    public void AddCombatResources(int morphineAmount, int vaccineAmount)
    {
        // verificar dps, talvez
        this._morphineAmount += morphineAmount;
        this._vaccineAmount += vaccineAmount;

        this.roomUIController.UpdateResourcesDisplay();
    }

    public void DeployCavaloInteraction()
    {
        // if (this._cavaloCooldown > 0f) { return; } // mensagem dps, ou sla, vou ver ainda como mostrar pro player isso

        if (!this._cavaloDeployed)
        {
            roomManager.MyPlayerRPC.RPCDeployCavalo(this.transform.position);
            this._cavaloDeployed = true;
            //this._cavaloCooldownTimer = this._cavaloCooldown;
            this.roomUIController.UpdateResourcesDisplay();
            return;
        }

        if (this._cavaloDeployed)
        {
            roomManager.MyPlayerRPC.RPCPickupCavalo();
            this._cavaloDeployed = false;
            //this._cavaloCooldownTimer = this._cavaloCooldown;
            this.roomUIController.UpdateResourcesDisplay();
            return;
        }

    }

    public void AddAmmo(int morphine = 0, int vaccine = 0)
    {
        this._morphineAmount += morphine;
        this._vaccineAmount += vaccine;

        roomUIController.UpdateResourcesDisplay();
    }

    public void ResetResources()
    {
        this._leukocyteAmount = this._pathogenAmount = 0;

        roomUIController.UpdateResourcesDisplay();
    }
}
