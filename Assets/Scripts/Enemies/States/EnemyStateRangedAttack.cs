using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateRangedAttack : EnemyState
{
    [Tooltip("How fast the enemy should rotate toward the player when winding up the attack.")]
    [SerializeField] private float rotationSpeed = 13.0f;
    [Tooltip("Cooldown before enemy can fire another shot.")]
    [SerializeField] private float shotCooldown = 1.0f;

    private float attackRange = 30.0f; //TODO: Remove this.

    public EnemyStateRangedAttack(EnemyBehavior behaviourReference) : base(behaviourReference){}

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

        RotateTowardsTarget();
        if(base.timer >= shotCooldown){
            Attack();
            base.timer = 0.0f;
        }

        if(Vector3.Distance(behaviour.transform.position, behaviour.GetTargetTransform().position) >= attackRange || !CheckLineOfSight())
            SetState(behaviour.chaseState);
    }

    private void RotateTowardsTarget(){
        Vector3 direction = (behaviour.GetTargetTransform().position - agent.transform.position).normalized;
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);
    }

    private void Attack(){
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, agent.transform.TransformDirection(Vector3.forward), out hit, attackRange)){
            if(hit.collider.GetComponent<MovementController>() != null){
                Debug.DrawRay(agent.transform.position, agent.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 1.0f);
                Debug.Log("Hit player");
            }
        }
    }

    private bool CheckLineOfSight(){
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, base.behaviour.GetTargetTransform().position - agent.transform.position, out hit, Mathf.Infinity)){
            if(hit.collider.GetComponent<MovementController>() != null)
                return true;
            return false;
        }
        else{
            return false;
        }
    }

}
