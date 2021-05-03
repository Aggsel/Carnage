using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;
    [SerializeField] protected float health = 10.0f;

    [HideInInspector] protected EnemyBaseState currentState = null;
    [HideInInspector] public AudioManager am;
    [HideInInspector] protected NavMeshAgent agent;
    [HideInInspector] protected GameObject player;

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
        am = AudioManager.Instance;
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

    public void SetState(EnemyBaseState newState){
        currentState?.OnStateExit();
        this.currentState = newState;
        currentState.OnStateEnter();
    }

    public virtual void OnShot(HitObject hit){
        currentState.OnShot(hit);

        if(bc != null){
            bc.InstantiateBlood(hit.hitPosition, hit.shotDirection);
        }
        am.PlaySound(ref am.patientHurt, transform.position);

        //Apply knockback, this should not be applied here.
        this.agent.transform.position += new Vector3(hit.shotDirection.x, 0.0f, hit.shotDirection.z).normalized * hit.knockback;
        this.health -= hit.damage;

        if(CheckDeathCriteria())
            OnDeath();
    }

    protected virtual bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }

    protected virtual void OnDeath(){
        bc.InstantiateDeathBlood(transform.position + new Vector3(0, 1.0f, 0));
        am.PlaySound(ref am.patientDeath, transform.position);
        parentSpawn?.ReportDeath(this);
        Destroy(this.gameObject);
    }
}