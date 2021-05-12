using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates.ConstraintMelee
{
    public class ConstraintMeleeBaseState : EnemyBaseState
    {
        protected ConstraintMeleeBehavior behavior;

        public void SetBehaviour(ConstraintMeleeBehavior behaviorReference){
            this.behavior = behaviorReference;
            this.agent = behaviorReference.GetAgent();
            this.anim = behaviorReference.anim;
        }

        public virtual void SetState(ConstraintMeleeBaseState newState){
            behavior.SetState(newState);
        }
    }

    [System.Serializable]
    public class ConstraintMeleeAttack : ConstraintMeleeBaseState
    {
        [Tooltip("How far the enemy can reach when attacking.")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float damage = 1.0f;

        public ConstraintMeleeAttack() : base(){}

        public override void OnStateEnter(){
            base.OnStateEnter();

            agent.ResetPath();
            agent.isStopped = true;
            anim.SetTrigger("attack");
        }

        public override void OnStateExit(){
            base.OnStateExit();
            if(agent.isOnNavMesh)
                agent.isStopped = false;
        }

        public override void Update(){
            base.Update();
            RotateTowardsTarget(behavior.GetTargetPosition());
        }

        public void Attack(){
            RaycastHit hit;
            if (Physics.Raycast(agent.transform.position, (behavior.GetTargetPosition() - agent.transform.position).normalized, out hit, attackRange)){
                Vector3 dir = agent.transform.position - hit.point;
                hit.collider.GetComponent<HealthController>()?.OnShot(new HitObject(dir, hit.point, damage: damage));
                behavior.am.PlaySound(ref behavior.am.patientMelee, behavior.transform.position);
            }
        }

        public void StopAttack (){
            anim.ResetTrigger("attack");
            SetState(behavior.chaseState);
        }
    }

    [System.Serializable]
    public class ConstraintMeleeChase : ConstraintMeleeBaseState
    {
        [Tooltip("Enemy movement speed while chasing the player.")]
        [SerializeField] private float chaseSpeed = 12.0f;
        [Tooltip("What distance from the player that this enemy should stop.")]
        [SerializeField] public float stoppingDistance = 1.0f;
        private float previousStoppingDistance = 0.0f;
        private float previousSpeed = 0.0f;
        
        public ConstraintMeleeChase() : base(){}

        public override void OnStateEnter(){
            base.OnStateEnter();

            previousSpeed = base.agent.speed;
            base.agent.speed = chaseSpeed;

            previousStoppingDistance = base.agent.stoppingDistance;
            base.agent.stoppingDistance = stoppingDistance;
        }

        public override void OnStateExit(){
            base.OnStateExit();
            agent.speed = previousSpeed;
            agent.stoppingDistance = previousStoppingDistance;
        }

        public override void Update(){
            base.Update();

            agent.SetDestination(behavior.GetTargetPosition());

            if (Vector3.Distance(behavior.transform.position, behavior.GetTargetPosition()) <= stoppingDistance)
                SetState(behavior.attackState);
        }
    }

    [System.Serializable]
    public class ConstraintMeleePatrol : ConstraintMeleeBaseState
    {
        [Tooltip("Will start to chase the player when player is closer than distance from the enemy.")]
        [SerializeField] private float enemyVisionDistance = 20.0f;
        [Tooltip("Movement speed when enemy is patroling.")]
        [SerializeField] private float patrolSpeed = 5.0f;
        private float previousSpeed = 0.0f;

        public ConstraintMeleePatrol() : base(){}

        public override void OnStateEnter(){
            base.OnStateEnter();

            previousSpeed = agent.speed;
            agent.speed = patrolSpeed;
            agent.ResetPath();
        }

        public override void OnStateExit(){
            base.OnStateExit();
            agent.speed = previousSpeed;
        }

        public override void Update(){
            base.Update();

            Vector3 newPos = base.behavior.transform.position + new Vector3(Random.Range(0.0f, 5.0f), base.behavior.transform.position.y, Random.Range(0.0f, 5.0f));
            agent.SetDestination(newPos);

            if(Vector3.Distance(behavior.transform.position, behavior.GetTargetPosition()) <= enemyVisionDistance)
                SetState(behavior.chaseState);
        }

        public override void OnShot(HitObject hit){
            behavior.SetState(behavior.chaseState);
        }
    }
}

