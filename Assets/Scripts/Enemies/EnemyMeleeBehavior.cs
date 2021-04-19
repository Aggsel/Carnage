using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMeleeBehavior : EnemyBehavior
{
    public NavMeshAgent agent;
    private GameObject player;
    private float health = 10.0f;

    [SerializeField] public EnemyStateChase chaseState;
    [SerializeField] public EnemyStatePatrol patrolState;
    [SerializeField] public EnemyStateAttack attackState;

    private void Start(){
        if(this.agent == null)
            this.agent = GetComponent<NavMeshAgent>();

        this.player = GameObject.Find("Player");
        
        chaseState = new EnemyStateChase(this);
        patrolState = new EnemyStatePatrol(this);
        attackState = new EnemyStateAttack(this);

        SetState(chaseState);
    }

    private void Update(){
        currentState.Update();
    }

    public Transform GetTargetTransform(){
        return player.transform;
    }

    public NavMeshAgent GetAgent(){
        return this.agent;
    }

    public override void OnShot(){
        currentState.OnShot();
        this.health -= 5.0f;

        if(CheckDeathCriteria())
            Destroy(this.gameObject);
    }

    private bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }
}
