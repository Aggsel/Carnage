using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateChase : EnemyState
{
    [SerializeField] private float chaseSpeed = 15.0f;
    
    public EnemyStateChase(EnemyMeleeBehavior behaviourReference) : base(behaviourReference){
        base.behaviour = behaviourReference;
    }

    public override void OnStateExit(){
        base.OnStateExit();
    }

    public override void OnStateEnter(){
        base.OnStateEnter();
        SetDebugColor(Color.red);

        agent.speed = chaseSpeed;
    }

    public override void Update(){
        base.Update();

        Debug.DrawLine(behaviour.transform.position, agent.destination, Color.white);
        agent.SetDestination(behaviour.GetTargetTransform().position);

        this.timer += Time.deltaTime;
        if(this.timer >= 10.0f)
            SetState(behaviour.patrolState);

        if(Vector3.Distance(behaviour.transform.position, behaviour.GetTargetTransform().position) <= attackRange)
            SetState(behaviour.attackState);
    }
}
