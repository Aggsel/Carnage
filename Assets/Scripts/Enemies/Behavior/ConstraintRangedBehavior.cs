using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates.ConstraintRanged;

public class ConstraintRangedBehavior : EnemyBehavior
{
    [SerializeField] public ConstraintRangedChase chaseState = new ConstraintRangedChase();
    [SerializeField] public ConstraintRangedPatrol patrolState = new ConstraintRangedPatrol();
    [SerializeField] public ConstraintRangedAttack attackState = new ConstraintRangedAttack();

    protected override void Start(){
        base.Start();
        chaseState.SetBehaviour(this);
        patrolState.SetBehaviour(this);
        attackState.SetBehaviour(this);
        am.PlaySound(am.patientSpawn, this.gameObject);
        SetState(chaseState);
    }

    public void AnimHookAttack(){
        attackState.Attack();
    }

    public void AnimHookStopAttack(){
        attackState.StopAttack();
    }
}
