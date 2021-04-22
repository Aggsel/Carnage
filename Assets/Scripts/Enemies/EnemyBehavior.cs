﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;
    protected EnemyState currentState = null;
    [Header("Blood Decals")]
    [SerializeField] GameObject bloodDecalProjector = null;
    [Tooltip("A value of 0.0f will set the decals rotation to continue from the shot direction. A value of 1.0f will rotate the decals to face straight down.")]
    [Range(0.0f,1.0f)]
    [SerializeField] private float decalRotation = 0.35f;

    [SerializeField] public EnemyStateChase chaseState;
    [SerializeField] public EnemyStatePatrol patrolState;
    [SerializeField] public EnemyStateAttack attackState;
    [SerializeField] public EnemyStateRangedAttack rangedAttackState;

    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] private GameObject player;

    protected virtual void Start(){
        if(this.agent == null)
            this.agent = GetComponent<NavMeshAgent>();

        this.player = GameObject.Find("Player"); //Don't do this.

        chaseState = new EnemyStateChase(this);
        patrolState = new EnemyStatePatrol(this);
        attackState = new EnemyStateAttack(this);
        rangedAttackState = new EnemyStateRangedAttack(this);
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

    public virtual void OnShot(HitObject hit){
        if(bloodDecalProjector != null)
            Instantiate(bloodDecalProjector, transform.position, Quaternion.Lerp(Quaternion.LookRotation(hit.shotDirection, Vector3.up), Quaternion.Euler(new Vector3(90, 0, 0)), decalRotation));
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }
    
    private void OnDestroy(){
        parentSpawn?.ReportDeath(this);
    }

}