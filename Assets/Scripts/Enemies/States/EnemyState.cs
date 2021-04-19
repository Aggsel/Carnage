using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyState
{
    [SerializeField] protected float attackRange = 3.0f;

    protected EnemyMeleeBehavior behaviour;
    [HideInInspector] protected NavMeshAgent agent;
    protected float timer = 0.0f;
    private float previousMovementSpeed = 0.0f;

    public EnemyState(EnemyMeleeBehavior behaviourReference){
        this.behaviour = behaviourReference;
        this.agent = behaviour.GetAgent();
    }

    public virtual void Update(){
        timer += Time.deltaTime;
    }

    public virtual void OnShot(){}

    public virtual void OnStateEnter(){
        previousMovementSpeed = agent.speed;
    }

    public virtual void OnStateExit(){
        agent.speed = previousMovementSpeed;
        timer = 0.0f;
    }

    public virtual void SetState(EnemyState newState){
        behaviour.SetState(newState);
    }

    protected void SetDebugColor(Color color){
        this.agent.GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", color);
    }

    public virtual EnemyState GetState(){
        return this;
    }
}
