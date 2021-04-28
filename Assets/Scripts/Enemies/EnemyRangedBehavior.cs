using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedBehavior : EnemyBehavior
{
    [SerializeField] private float health = 10.0f;

    protected override void Start(){
        base.Start();

        SetState(rangedAttackState);
    }

    protected override void Update(){
        base.Update();

        //This is really dumb, ideally the current state should be responsible 
        //for when to change state, NOT the behaviour.
        if(Vector3.Distance(transform.position, GetTargetPosition()) <= chaseState.stoppingDistance && currentState != rangedAttackState){
            RotateTowardsTarget();
            if(EnemyBehavior.CheckLineOfSight(agent.transform.position, GetTargetPosition()))
                SetState(rangedAttackState);
        }
    }

    private void RotateTowardsTarget(){
        Vector3 direction = (GetTargetPosition() - agent.transform.position).normalized;
        direction = new Vector3(direction.x, 0, direction.z);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * 12.0f);
    }

    public override void OnShot(HitObject hit){
        base.OnShot(hit);
        currentState.OnShot(hit);

        this.health -= hit.damage;
        if(CheckDeathCriteria())
            Destroy(this.gameObject);
    }

    private bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }
}
