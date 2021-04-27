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
    
    public EnemyStateChase() : base(){}

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

        agent.SetDestination(behavior.GetTargetPosition());

        if (Vector3.Distance(behavior.transform.position, behavior.GetTargetPosition()) <= attackInitiationRange)
            SetState(behavior.attackState);
    }
}
