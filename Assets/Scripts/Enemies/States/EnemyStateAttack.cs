﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateAttack : EnemyState
{
    [Tooltip("How fast the enemy should rotate toward the player when winding up the attack.")]
    [SerializeField] private float rotationSpeed = 13.0f;
    [Tooltip("How far the enemy can reach when attacking.")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damage = 1.0f;

    public EnemyStateAttack() : base(){}

    public override void OnStateEnter(){
        base.OnStateEnter();
        
        agent.ResetPath();
        agent.isStopped = true;

        anim.SetTrigger("attack");
        SetDebugColor(Color.green);
    }

    public override void OnStateExit(){
        base.OnStateExit();

        agent.isStopped = false;
    }

    public override void Update(){
        base.Update();
        RotateTowardsTarget();
    }

    private void RotateTowardsTarget(){
        Vector3 direction = (behavior.GetTargetPosition() - agent.transform.position).normalized;
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);
    }

    public void StopAttack ()
    {
        anim.ResetTrigger("attack");
        SetState(behavior.chaseState);
    }

    public void Attack(){
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, agent.transform.TransformDirection(Vector3.forward), out hit, attackRange)){
            Vector3 dir = agent.transform.position - hit.point;
            hit.collider.GetComponent<HealthController>()?.OnShot(new HitObject(dir, hit.point, damage: 1.0f));
            behavior.am.PlaySound(ref behavior.am.patientMelee, behavior.transform.position);
        }
    }
}