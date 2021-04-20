﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatePatrol : EnemyState
{
    [Tooltip("Will start to chase the player when player is closer than distance from the enemy.")]
    [SerializeField] private float enemyVisionDistance = 20.0f;
    [Tooltip("Movement speed when enemy is patroling.")]
    [SerializeField] private float patrolSpeed = 5.0f;
    private float previousSpeed = 0.0f;

    public EnemyStatePatrol(EnemyBehavior behaviourReference) : base(behaviourReference){}

    public override void OnStateEnter(){
        base.OnStateEnter();

        previousSpeed = agent.speed;
        agent.speed = patrolSpeed;
        agent.ResetPath();

        SetDebugColor(Color.blue);
    }

    public override void OnStateExit(){
        base.OnStateExit();
        agent.speed = previousSpeed;
    }

    public override void Update(){
        base.Update();

        Vector3 newPos = base.behaviour.transform.position + new Vector3(Random.Range(0.0f, 5.0f), base.behaviour.transform.position.y, Random.Range(0.0f, 5.0f));
        agent.SetDestination(newPos);

        if(Vector3.Distance(behaviour.transform.position, behaviour.GetTargetTransform().position) <= enemyVisionDistance)
            SetState(behaviour.chaseState);
    }

    public override void OnShot(HitObject hit){
        behaviour.SetState(behaviour.chaseState);
    }
}
