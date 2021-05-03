using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyBaseState
{
    [SerializeField] private float rotationSpeed = 15.0f;
    protected NavMeshAgent agent;
    protected Animator anim;
    protected float timer = 0.0f;

    public EnemyBaseState(){}

    public virtual void Update(){
        timer += Time.deltaTime;
        anim.SetFloat("speed", agent.velocity.magnitude);
    }

    public virtual void OnShot(HitObject hit){}

    public virtual void OnStateEnter(){}

    public virtual void OnStateExit(){
        timer = 0.0f;
    }

    protected void RotateTowardsTarget(Vector3 target){
        Vector3 direction = (target - agent.transform.position).normalized;
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);
    }
}
