using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates.ConstraintRanged
{
    public class ConstraintRangedBaseState : EnemyBaseState
    {
        protected ConstraintRangedBehavior behavior;
        
        public void SetBehaviour(ConstraintRangedBehavior behaviorReference){
            this.behavior = behaviorReference;
            this.agent = behaviorReference.GetAgent();
            this.anim = behaviorReference.anim;
        }

        public virtual void SetState(ConstraintRangedBaseState newState){
            behavior.SetState(newState);
        }
    }

    [System.Serializable]
    public class ConstraintRangedAttack : ConstraintRangedBaseState
    {
        private float timeOutOfSight = 0.0f;
        [SerializeField] private GameObject projectilePrefab = null;
        [SerializeField] private Transform projectileSpawnPosition = null;

        public ConstraintRangedAttack() : base(){}

        public override void OnStateEnter(){
            base.OnStateEnter();
            
            agent.ResetPath();
            agent.isStopped = true;

            anim.SetTrigger("attack");
        }

        public override void OnStateExit(){
            base.OnStateExit();
            agent.isStopped = false;
        }

        public override void Update(){
            base.Update();

            RotateTowardsTarget(behavior.GetTargetPosition());
            bool lineOfSight = EnemyBehavior.CheckLineOfSight(agent.transform.position, behavior.GetTargetPosition());
            timeOutOfSight = !lineOfSight ? timeOutOfSight + Time.deltaTime : 0.0f; //Update timeOutOfSight if player is not in sight, otherwise reset.

            if(timeOutOfSight >= 2.0f){
                SetState(behavior.chaseState);
            }
        }

        public void Attack(){
            FireProjectile(projectilePrefab, projectileSpawnPosition);
        }

        public void FireProjectile(GameObject projectile, Transform spawnTransform = null){
            if(projectile == null)
                return;
            if(spawnTransform == null)
                spawnTransform = behavior.transform;
            GameObject instantiatedProjectile = GameObject.Instantiate(projectile, spawnTransform.position, spawnTransform.rotation, behavior.transform.parent);
            instantiatedProjectile.transform.LookAt(behavior.GetTargetPosition());
        }

        public void StopAttack(){
            anim.ResetTrigger("attack");
            SetState(behavior.chaseState);
        }
    }

    [System.Serializable]
    public class ConstraintRangedChase : ConstraintRangedBaseState
    {
        [Tooltip("Enemy movement speed while chasing the player.")]
        [SerializeField] private float chaseSpeed = 12.0f;
        [Tooltip("What distance from the player that this enemy should stop.")]
        [SerializeField] public float stoppingDistance = 1.0f;
        private float previousStoppingDistance = 0.0f;
        private float previousSpeed = 0.0f;

        public ConstraintRangedChase() : base(){}

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

            if(Vector3.Distance(behavior.transform.position, behavior.GetTargetPosition()) <= behavior.chaseState.stoppingDistance){
                RotateTowardsTarget(behavior.GetTargetPosition());
                if(EnemyBehavior.CheckLineOfSight(agent.transform.position, behavior.GetTargetPosition()))
                    SetState(behavior.attackState);
            }

            if (Vector3.Distance(behavior.transform.position, behavior.GetTargetPosition()) <= stoppingDistance)
                SetState(behavior.attackState);
        }
    }

    [System.Serializable]
    public class ConstraintRangedPatrol : ConstraintRangedBaseState
    {
        [Tooltip("Will start to chase the player when player is closer than distance from the enemy.")]
        [SerializeField] private float enemyVisionDistance = 20.0f;
        [Tooltip("Movement speed when enemy is patroling.")]
        [SerializeField] private float patrolSpeed = 5.0f;
        private float previousSpeed = 0.0f;

        public ConstraintRangedPatrol() : base(){}

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
