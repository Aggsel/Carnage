using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeBehavior : EnemyBehavior
{
    [SerializeField] private float health = 10.0f;

    protected override void Start(){
        base.Start();
        SetState(chaseState);
    }

    public override void OnShot(HitObject hit){
        base.OnShot(hit);
        currentState.OnShot(hit);

        //Apply knockback, this should not be applied here.
        this.agent.transform.position += new Vector3(hit.shotDirection.x, 0.0f, hit.shotDirection.z).normalized * hit.knockback;

        this.health -= hit.damage;
        if(CheckDeathCriteria())
            Destroy(this.gameObject);
    }

    private bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }
}
