using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;
    [SerializeField] protected EnemyState currentState = null;
    [Header("Blood Decals")]
    [SerializeField] GameObject bloodDecalProjector = null;
    [Tooltip("A value of 0.0f will set the decals rotation to continue from the shot direction. A value of 1.0f will rotate the decals to face straight down.")]
    [Range(0.0f,1.0f)]
    [SerializeField] private float decalRotation = 0.35f;

    [SerializeField] public EnemyStateChase chaseState = new EnemyStateChase();
    [SerializeField] public EnemyStatePatrol patrolState = new EnemyStatePatrol();
    [SerializeField] public EnemyStateAttack attackState = new EnemyStateAttack();
    [SerializeField] public EnemyStateRangedAttack rangedAttackState = new EnemyStateRangedAttack();

    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] private GameObject player;
    public AudioManager am = null;

    [HideInInspector] public Animator anim = null;
    private BloodController bc = null;

    protected virtual void Start(){

        bc = FindObjectOfType<BloodController>();
        anim = GetComponentInChildren<Animator>();
        am = AudioManager.Instance;
        am.PlaySound(ref am.patientSpawn, this.gameObject);
        if(this.agent == null)
            this.agent = GetComponent<NavMeshAgent>();

        this.player = GameObject.Find("Player"); //Don't do this.

        chaseState.SetBehaviour(this);
        patrolState.SetBehaviour(this);
        attackState.SetBehaviour(this);
        rangedAttackState.SetBehaviour(this);
    }

    protected virtual void Update(){
        currentState?.Update();
    }

    public Vector3 GetTargetPosition(){
        return player.transform.position;
    }

    public NavMeshAgent GetAgent(){
        return this.agent;
    }

    public static bool CheckLineOfSight(Vector3 originPos, Vector3 targetPosition){
        RaycastHit hit;
        if (Physics.Raycast(originPos, (targetPosition - originPos).normalized, out hit, Mathf.Infinity)){
            //Should this mask passed as an function argument instead?
            if(((1<<hit.collider.gameObject.layer) & LayerMask.GetMask("Player")) != 0)
                return true;
            return false;
        }
        else{
            return false;
        }
    }

    public void SetState(EnemyState newState){
        currentState?.OnStateExit();
        this.currentState = newState;
        currentState.OnStateEnter();
    }

    public void FireProjectile(GameObject projectile, Transform spawnTransform = null){
        if(projectile == null)
            return;
        if(spawnTransform == null)
            spawnTransform = transform;
        GameObject instantiatedProjectile = Instantiate(projectile, spawnTransform.position, spawnTransform.rotation, transform.parent);
        instantiatedProjectile.GetComponent<EnemyProjectile>().parent = anim.gameObject;
    }

    public virtual void OnShot(HitObject hit){
        
        if(bc != null)
        {
            bc.InstantiateBlood(hit.hitPosition, hit.shotDirection);
        }
        am.PlaySound(ref am.patientHurt, transform.position);
        
        //if(bloodDecalProjector != null)
        //    Instantiate(bloodDecalProjector, transform.position, Quaternion.Lerp(Quaternion.LookRotation(hit.shotDirection, Vector3.up), Quaternion.Euler(new Vector3(90, 0, 0)), decalRotation));
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }
    
    private void OnDestroy(){
        bc.InstantiateDeathBlood(transform.position + new Vector3(0, 2.0f, 0));
        am.PlaySound(ref am.patientDeath, transform.position);
        if(this != null)    //In order to prevent unwanted behaviour while destroying enemies when exiting the game.
            parentSpawn?.ReportDeath(this);
    }

}