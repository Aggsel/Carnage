using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateRangedAttack : EnemyState
{
    [Tooltip("How fast the enemy should rotate toward the player when winding up the attack.")]
    [SerializeField] private float rotationSpeed = 13.0f;
    [Tooltip("Cooldown before enemy can fire another shot.")]
    [SerializeField] private float shotCooldown = 0.2f;
    private float timeOutOfSight = 0.0f;

    private float attackRange = 30.0f; //TODO: Remove this.

    public EnemyStateRangedAttack(EnemyBehavior behaviorReference) : base(behaviorReference){}

    public override void OnStateEnter(){
        base.OnStateEnter();
        
        agent.ResetPath();
        agent.isStopped = true;

        SetDebugColor(Color.white);
    }

    public override void OnStateExit(){
        base.OnStateExit();

        agent.isStopped = false;
    }

    public override void Update(){
        base.Update();

        if(base.timer >= shotCooldown){
            base.timer = 0.0f;
            Attack();
        }

        RotateTowardsTarget();
        bool lineOfSight = EnemyBehavior.CheckLineOfSight(agent.transform.position, behavior.GetTargetPosition());
        timeOutOfSight = !lineOfSight ? timeOutOfSight + Time.deltaTime : 0.0f; //Update timeOutOfSight if player is not in sight, otherwise reset.

        if(!lineOfSight && timeOutOfSight >= 2.0f)
            SetState(behavior.chaseState);

        if(Vector3.Distance(behavior.transform.position, behavior.GetTargetPosition()) >= attackRange)
            SetState(behavior.chaseState);
    }

    private void RotateTowardsTarget(){
        Vector3 direction = (behavior.GetTargetPosition() - agent.transform.position).normalized;
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);
    }

    private void Attack(){
        //Fire projectile towards behaviour.GetTargetPosition()
    }

    private bool CheckLineOfSight(){
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, base.behavior.GetTargetPosition() - agent.transform.position, out hit, Mathf.Infinity)){
            if(hit.collider.GetComponent<MovementController>() != null){
                return true;
            }
            return false;
        }
        else{
            return false;
        }
    }

}
