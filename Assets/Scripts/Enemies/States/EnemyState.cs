﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyState
{
    [Tooltip("How far from the player the enemy has to be in order to initiate an attack. Keep in mind that this is not the range of the actual attack.")]
    [SerializeField] protected float attackInitiationRange = 4.0f;

    protected EnemyBehavior behavior;
    protected NavMeshAgent agent;
    protected float timer = 0.0f;

    public EnemyState(EnemyBehavior behaviourReference){
        this.behavior = behaviourReference;
        this.agent = behaviourReference.GetAgent();
    }

    public virtual void Update(){
        timer += Time.deltaTime;
    }

    public virtual void OnShot(HitObject hit){}

    public virtual void OnStateEnter(){}

    public virtual void OnStateExit(){
        timer = 0.0f;
    }

    public virtual void SetState(EnemyState newState){
        behavior.SetState(newState);
    }

    protected void SetDebugColor(Color color){
        this.agent.GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", color);
    }

    public virtual EnemyState GetState(){
        return this;
    }
}
