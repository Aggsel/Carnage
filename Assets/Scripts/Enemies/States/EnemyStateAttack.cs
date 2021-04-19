using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateAttack : EnemyState
{
    public EnemyStateAttack(EnemyMeleeBehavior behaviourReference) : base(behaviourReference){
        base.behaviour = behaviourReference;
    }

    public override void OnStateExit(){
        base.OnStateExit();
        agent.isStopped = false;
    }

    public override void OnStateEnter(){
        base.OnStateEnter();
        agent.ResetPath();
        agent.isStopped = true;

        SetDebugColor(Color.green);
    }

    public override void Update(){
        base.Update();

        this.timer += Time.deltaTime;
        if(this.timer >= 2.0f)
            SetState(behaviour.chaseState);
    }
}
