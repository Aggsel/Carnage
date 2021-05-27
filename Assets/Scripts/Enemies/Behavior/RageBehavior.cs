using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates.Rage;

public class RageBehavior : EnemyBehavior
{
    [Header("States")]
    [Tooltip("Attributes when the enemy is chasing the player.")]
    [SerializeField] public RageChase chaseState = new RageChase();
    [Tooltip("Attributes when enemy is in idle.")]
    [SerializeField] public RageIdle idleState = new RageIdle();
    [Tooltip("Attributes when enemy is melee attacking the player HULK SMASH.")]
    [SerializeField] public RageAttack attackState = new RageAttack();

    [Header("Charge States")]
    [Tooltip("Attributes while the enemy is transitioning to the charge attack")]
    [SerializeField] public RagePreChargeAttack preChargeAttack = new RagePreChargeAttack();
    [Tooltip("Attributes while the enemy is actively charging.")]
    [SerializeField] public RageChargeAttack chargeAttackState = new RageChargeAttack();
    [Tooltip("Attributes when the enemy has hit something and is attacking.")]
    [SerializeField] public RagePostChargeAttack postChargeAttack = new RagePostChargeAttack();

    protected override void Start(){
        base.Start();

        chaseState.SetBehaviour(this);
        idleState.SetBehaviour(this);
        attackState.SetBehaviour(this);

        preChargeAttack.SetBehaviour(this);
        chargeAttackState.SetBehaviour(this);
        postChargeAttack.SetBehaviour(this);

        SetState(chaseState);
    }

    protected override void Update() {
        base.Update();
        // Debug.Log(currentState);
    }

    public void AnimHookChargeStart(){
        SetState(chargeAttackState);
    }

    public void AnimHookChargeStop(){
        SetState(chaseState);
    }

    public void AnimHookAttack(){
        attackState.Attack();
    }

    public void AnimHookStopAttack(){
        SetState(chaseState);
    }
}
