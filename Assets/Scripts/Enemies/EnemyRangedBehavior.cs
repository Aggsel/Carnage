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

        if(Vector3.Distance(transform.position, GetTargetTransform().position) <= 20.0f){
            RotateTowardsTarget();
            if(CheckLineOfSight())
                SetState(rangedAttackState);
        }
    }

    private void RotateTowardsTarget(){
        Vector3 direction = (GetTargetTransform().position - agent.transform.position).normalized;
        direction = new Vector3(direction.x, 0, direction.z);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * 12.0f);
    }

    public override void OnShot(HitObject hit){
        base.OnShot(hit);
        currentState.OnShot(hit);

        //Apply knockback, this should not be applied here.
        this.agent.transform.position += new Vector3(hit.shotDirection.x, 0.0f, hit.shotDirection.z).normalized * hit.knockback;

        this.health -= 5.0f;
        if(CheckDeathCriteria())
            Destroy(this.gameObject);
    }

    private bool CheckLineOfSight(){
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, GetTargetTransform().position - agent.transform.position, out hit, Mathf.Infinity)){
            if(hit.collider.GetComponent<MovementController>() != null)
                return true;
            return false;
        }
        else{
            return false;
        }
    }

    private bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }
}
