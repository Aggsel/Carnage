using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates.ConstraintMelee;

public class ConstraintMeleeBehavior : EnemyBehavior
{
    [SerializeField] public ConstraintMeleeChase chaseState = new ConstraintMeleeChase();
    [SerializeField] public ConstraintMeleePatrol patrolState = new ConstraintMeleePatrol();
    [SerializeField] public ConstraintMeleeAttack attackState = new ConstraintMeleeAttack();

    protected override void Start(){
        base.Start();
        chaseState.SetBehaviour(this);
        patrolState.SetBehaviour(this);
        attackState.SetBehaviour(this);
        SetState(chaseState);
    }

    public void AnimHookAttack(){
        attackState.Attack();
    }

    public void AnimHookStopAttack(){
        attackState.StopAttack();
    }
}
