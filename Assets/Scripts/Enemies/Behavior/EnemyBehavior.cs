using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] GameEvent onDeath = null;
    [SerializeField] GameEvent onDamage = null;
    [SerializeField] GameEvent onMeleeKill = null;

    private EnemySpawnPoint parentSpawn;
    [SerializeField] protected float health = 10.0f;
    [SerializeField] protected float difficulty = 1.0f;

    protected EnemyBaseState currentState = null;
    [HideInInspector] public AudioManager am;
    [HideInInspector] protected NavMeshAgent agent;
    [HideInInspector] protected GameObject player;

    [HideInInspector] public Animator anim = null;
    private BloodController bc = null;
    private Collider enemyCollider = null;

    protected virtual void Start(){
        am = AudioManager.Instance;
        bc = FindObjectOfType<BloodController>();
        anim = GetComponentInChildren<Animator>();
        if(this.agent == null)
            this.agent = GetComponent<NavMeshAgent>();

        this.player = GameObject.Find("Player"); //Don't do this.
        enemyCollider = GetComponentInChildren<Collider>();
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

    public static bool CheckLineOfSight(Vector3 originPos, Vector3 targetPosition, float range = Mathf.Infinity, bool ignoreEnemies = true){
        RaycastHit hit;
        if (Physics.Raycast(originPos, (targetPosition - originPos).normalized, out hit, range, ignoreEnemies ? ~LayerMask.GetMask("Enemy") : ~0)){
            Debug.DrawLine(originPos, hit.point, Color.blue, 0.5f);
            //Should this mask passed as an function argument instead?
            if(((1<<hit.collider.gameObject.layer) & LayerMask.GetMask("Player")) != 0)
                return true;
                
            Debug.DrawLine(originPos, targetPosition, Color.red, 0.5f);
            return false;
        }
        else{
            Debug.DrawLine(originPos, targetPosition, Color.red, 0.5f);
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
        onDamage?.Invoke();

        if(bc != null){
            bc.InstantiateBlood(hit.hitPosition, hit.shotDirection);
        }
        

        //Apply knockback, this should not be applied here.
        this.agent.transform.position += new Vector3(hit.shotDirection.x, 0.0f, hit.shotDirection.z).normalized * hit.knockback;
        this.health -= hit.damage;

        if(CheckDeathCriteria()){
            if(hit.type == HitType.Melee)
                onMeleeKill?.Invoke();
            OnDeath();
        }
    }

    protected virtual bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }

    public float GetDifficulty(){
        return this.difficulty;
    }

    protected virtual void OnDeath(){
        currentState.OnDeath();
        bc.InstantiateDeathBlood(transform.position + new Vector3(0, 1.0f, 0));
        parentSpawn?.ReportDeath(this);
        onDeath?.Invoke();
        Destroy(this.gameObject);
        enemyCollider.enabled = false;
    }
}