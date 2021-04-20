using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateChase : EnemyState
{
    [Tooltip("Enemy movement speed while chasing the player.")]
    [SerializeField] private float chaseSpeed = 12.0f;
    [Tooltip("What distance from the player that this enemy should stop.")]
    [SerializeField] private float stoppingDistance = 1.0f;
    private float previousStoppingDistance = 0.0f;
    private float previousSpeed = 0.0f;
    
    public EnemyStateChase(EnemyBehavior behaviourReference) : base(behaviourReference){}

    public override void OnStateEnter(){
        base.OnStateEnter();

        previousSpeed = base.agent.speed;
        base.agent.speed = chaseSpeed;

        previousStoppingDistance = base.agent.stoppingDistance;
        base.agent.stoppingDistance = stoppingDistance;
        
        SetDebugColor(Color.red);
    }

    public override void OnStateExit(){
        base.OnStateExit();
        agent.speed = previousSpeed;
        agent.stoppingDistance = previousStoppingDistance;
    }

    public override void Update(){
        base.Update();

        Debug.DrawLine(behaviour.transform.position, agent.destination, Color.white);
        agent.SetDestination(behaviour.GetTargetTransform().position);

        if(this.timer >= 10.0f)
            SetState(behaviour.patrolState);

        if(Vector3.Distance(behaviour.transform.position, behaviour.GetTargetTransform().position) <= attackInitiationRange)
            SetState(behaviour.attackState);
    }
}
