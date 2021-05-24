using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates.Rage
{
    public class RageBaseState : EnemyBaseState
    {
        protected RageBehavior behavior;

        public void SetBehaviour(RageBehavior behaviorReference){
            this.behavior = behaviorReference;
            this.agent = behaviorReference.GetAgent();
            this.anim = behaviorReference.anim;
        }

        public virtual void SetState(RageBaseState newState){
            behavior.SetState(newState);
        }
    }

    [System.Serializable]
    public class RageAttack : RageBaseState
    {
        [Tooltip("How many raycasts (per side) should be used to determine if player is hit. Should ideally be a small, odd number. 3 count = 3 raycasts on each side of center.")]
        [SerializeField] private int dashRaycastCount = 9;
        [Tooltip("Unintuitively, smaller value = more spread")]
        [SerializeField] private float raySpread = 4.6f;
        [SerializeField] private float attackRange = 1.5f;

        [SerializeField] private LayerMask enemyLayerMask = 0;

        [SerializeField] private float damage = 30.0f;
        [SerializeField] private float knockback = 0.0f;

        [SerializeField] private float timeoutDuration = 2.0f;

        public RageAttack() : base(){}

        public override void OnStateEnter(){
            base.OnStateEnter();
            anim.SetTrigger("attack");
            // behavior.transform.rotation = Quaternion.LookRotation((behavior.GetTargetPosition() - behavior.transform.position).normalized, Vector3.up);
            agent.isStopped = true;
        }

        public override void Update(){
            base.Update();

            if(timer >= timeoutDuration)
                behavior.SetState(behavior.chaseState);
        }

        public override void OnStateExit(){
            anim.ResetTrigger("attack");
            base.OnStateExit();
        }

        public void Attack(){
            for (int i = -dashRaycastCount; i <= dashRaycastCount; i++){
                float x = Mathf.Sin(i / raySpread + behavior.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                float z = Mathf.Cos(i / raySpread + behavior.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                Vector3 point = new Vector3(x,0,z) * attackRange;
                Debug.DrawRay(agent.transform.position + new Vector3(0, 0.5f, 0), point, Color.red);
                RaycastHit hit;
                if (Physics.Raycast(agent.transform.position + new Vector3(0, 0.5f, 0), point, out hit, attackRange, ~enemyLayerMask)){
                    if(((1<<hit.collider.gameObject.layer) & LayerMask.GetMask("Player")) != 0){
                        HitObject hitObject = new HitObject((hit.point - agent.transform.position).normalized, hit.point, damage, knockback, HitType.Melee);
                        hit.collider.gameObject.GetComponent<HealthController>().OnShot(hitObject);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class RageChase : RageBaseState
    {
        [Tooltip("Enemy movement speed while chasing the player.")]
        [SerializeField] private float chaseSpeed = 12.0f;
        [Tooltip("What distance from the player that this enemy should stop.")]
        [SerializeField] public float stoppingDistance = 1.0f;
        [Tooltip("Will charge player when the enemy can see the player. This is how far the enemy can see.")]
        [SerializeField] private float visionRange = 15.0f;
        [Tooltip("How often the enemy should check if the player is in line of sight.")]
        [SerializeField] private float lineOfSightFrequency = 0.8f;
        [Tooltip("How close the enemy should get to the player before attacking.")]
        [SerializeField] private float attackRange = 2.0f;
        private float lineOfSightTimer = 0.0f;
        [Tooltip("How often the enemy should recalculate the path to the player.")]
        [SerializeField] private float pathRecalculationFrequency = 0.5f;
        private float pathRecalculationTimer = 0.0f;
        private float previousStoppingDistance = 0.0f;
        private float previousSpeed = 0.0f;
        private float attackRangeSqrd = 0.0f;
        
        public RageChase() : base(){
            attackRangeSqrd = attackRange * attackRange;
        }

        public override void OnStateEnter(){
            base.OnStateEnter();

            previousSpeed = base.agent.speed;
            base.agent.speed = chaseSpeed;

            previousStoppingDistance = base.agent.stoppingDistance;
            base.agent.stoppingDistance = stoppingDistance;

            agent.isStopped = false;
        }

        public override void OnStateExit(){
            base.OnStateExit();
            agent.speed = previousSpeed;
            agent.stoppingDistance = previousStoppingDistance;
        }

        public override void Update(){
            base.Update();

            pathRecalculationTimer += Time.deltaTime;
            if(timer >= pathRecalculationFrequency){
                agent.SetDestination(behavior.GetTargetPosition());
                pathRecalculationTimer = 0.0f;
            }

            lineOfSightTimer += Time.deltaTime;
            if(lineOfSightTimer >= lineOfSightFrequency){
                if(EnemyBehavior.CheckLineOfSight(behavior.transform.position, behavior.GetTargetPosition(), visionRange))
                    behavior.SetState(behavior.preChargeAttack);
                lineOfSightTimer = 0.0f;
            }

            if(Vector3.SqrMagnitude(behavior.GetTargetPosition() - behavior.transform.position) < attackRangeSqrd)
                SetState(behavior.attackState);
        }
    }

    [System.Serializable]
    public class RageIdle : RageBaseState
    {
        [Tooltip("How long the enemy should be stunned before charging again.")]
        [SerializeField] private float timeoutDuration = 5.0f;
        [SerializeField] private float attackRange = 2.0f;
        private float attackRangeSqrd = 0.0f;

        public RageIdle() : base(){
            attackRangeSqrd = attackRange * attackRange;
        }

        public override void OnStateEnter(){
            base.OnStateEnter();
            agent.isStopped = true;
        }

        public override void Update(){
            base.Update();
            RotateTowardsTarget(behavior.GetTargetPosition());

            if(timer >= timeoutDuration)
                if(EnemyBehavior.CheckLineOfSight(behavior.transform.position, behavior.GetTargetPosition()))
                    behavior.SetState(behavior.preChargeAttack);
                else
                    behavior.SetState(behavior.chaseState);

            if(Vector3.SqrMagnitude(behavior.GetTargetPosition() - behavior.transform.position) < attackRangeSqrd)
                SetState(behavior.attackState);
        }

        public override void OnShot(HitObject hit){
            behavior.SetState(behavior.chaseState);
        }
    }

    [System.Serializable]
    public class RagePreChargeAttack : RageBaseState{
        public RagePreChargeAttack() : base(){}

        public override void OnStateEnter(){
            base.OnStateEnter();
            agent.isStopped = true;
            anim.SetBool("charge", true);
        }

        public override void Update(){
            base.Update();
            RotateTowardsTarget(behavior.GetTargetPosition());
        }
    }

    [System.Serializable]
    public class RageChargeAttack : RageBaseState{
        [Tooltip("How fast the enemy should move while charging towards the player.")]
        [SerializeField] private float movementSpeed = 15.0f;
        [Tooltip("Maximum length of a given charge attack.")]
        [SerializeField] private float chargeLength = 20.0f;
        [Tooltip("How far the range of the charge attack is.")]
        [SerializeField] private float dashAttackRange = 2.0f;
        [SerializeField] private float timeoutDuration = 4.0f;
        [SerializeField] private float stoppingDistance = 3.0f;
        [SerializeField] private float damage = 30.0f;
        [SerializeField] private float knockback = 5.0f;

        [Tooltip("How many raycasts (per side) should be used to determine if player is hit. Should ideally be a small, odd number. 3 count = 3 raycasts on each side of center.")]
        [SerializeField] private int dashRaycastCount = 3;
        [Tooltip("Unintuitively, smaller value = more spread")]
        [SerializeField] private float raySpread = 5.0f;

        [SerializeField] private LayerMask playerLayer = 0;
        [SerializeField] private LayerMask enemyLayerMask = 0;

        private float previousSpeed = 0.0f;
        Vector3 targetPosition = new Vector3(0,0,0);
        private float stoppingDistanceSqrd = 0.0f;

        public RageChargeAttack() : base(){
            stoppingDistanceSqrd = stoppingDistance * stoppingDistance;
        }

        public override void OnStateEnter(){
            base.OnStateEnter();

            previousSpeed = agent.speed;
            agent.speed = movementSpeed;
            agent.isStopped = false;

            RotateTowardsTarget(behavior.GetTargetPosition());
            RaycastHit hit;
            if (Physics.Raycast(behavior.transform.position, (behavior.GetTargetPosition() - behavior.transform.position).normalized, out hit, chargeLength, ~playerLayer)){
                targetPosition = hit.point;
                //Remove the agent radius from the target position to prevent target from being inside walls.
                targetPosition = targetPosition + (behavior.transform.position - targetPosition).normalized * agent.radius * 2.0f;
            }
            else{
                targetPosition = behavior.transform.position + (behavior.GetTargetPosition() - behavior.transform.position).normalized * chargeLength;
            }

            agent.SetDestination(targetPosition);
            Debug.DrawLine(behavior.transform.position, targetPosition, Color.yellow, 2.0f);
        }

        public override void OnStateExit(){
            base.OnStateExit();
            agent.speed = previousSpeed;
            anim.SetBool("charge", false);
        }

        public override void Update(){
            base.Update();

            //Check if player is in the way. If hit, damage the player and become stunned.
            if(Attack()){
                behavior.SetState(behavior.postChargeAttack);
                return;
            }

            if(Vector3.SqrMagnitude(behavior.transform.position - targetPosition) < stoppingDistance){
                behavior.SetState(behavior.idleState);
            }

            if(this.timer > timeoutDuration){
                behavior.SetState(behavior.chaseState);
            }
        }

        //Returns true if player was hit.
        private bool Attack(){
            for (int i = -dashRaycastCount; i <= dashRaycastCount; i++){
                float x = Mathf.Sin(i / raySpread + behavior.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                float z = Mathf.Cos(i / raySpread + behavior.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                Vector3 point = new Vector3(x,0,z) * dashAttackRange;
                Debug.DrawRay(agent.transform.position + new Vector3(0, 0.5f, 0), point, Color.red);
                RaycastHit hit;
                if (Physics.Raycast(agent.transform.position + new Vector3(0, 0.5f, 0), point, out hit, dashAttackRange, ~enemyLayerMask)){
                    if(((1<<hit.collider.gameObject.layer) & LayerMask.GetMask("Player")) != 0){
                        HitObject hitObject = new HitObject((hit.point - agent.transform.position).normalized, hit.point, damage, knockback, HitType.Melee);
                        hit.collider.gameObject.GetComponent<HealthController>().OnShot(hitObject);
                        behavior.SetState(behavior.idleState);
                        return true;
                    }
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class RagePostChargeAttack : RageBaseState{
        public RagePostChargeAttack() : base(){}

        [Tooltip("How many raycasts (per side) should be used to determine if player is hit. Should ideally be a small, odd number. 3 count = 3 raycasts on each side of center.")]
        [SerializeField] private int dashRaycastCount = 7;
        [Tooltip("Unintuitively, smaller value = more spread")]
        [SerializeField] private float raySpread = 4.6f;
        [SerializeField] private float dashAttackRange = 1.5f;

        [SerializeField] private LayerMask enemyLayerMask = 0;

        [SerializeField] private float damage = 30.0f;
        [SerializeField] private float knockback = 0.0f;

        public override void OnStateEnter(){
            base.OnStateEnter();
            agent.isStopped = true;
            anim.SetBool("charge", false);
        }

        public override void OnStateExit(){
            base.OnStateExit();
            RotateTowardsTarget(behavior.GetTargetPosition());
            Attack();
        }

        public override void Update(){
            base.Update();
            RotateTowardsTarget(behavior.GetTargetPosition());
        }

        private void Attack(){
            for (int i = -dashRaycastCount; i <= dashRaycastCount; i++){
                float x = Mathf.Sin(i / raySpread + behavior.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                float z = Mathf.Cos(i / raySpread + behavior.transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                Vector3 point = new Vector3(x,0,z) * dashAttackRange;
                Debug.DrawRay(agent.transform.position + new Vector3(0, 0.5f, 0), point, Color.red);
                RaycastHit hit;
                if (Physics.Raycast(agent.transform.position + new Vector3(0, 0.5f, 0), point, out hit, dashAttackRange, ~enemyLayerMask)){
                    if(((1<<hit.collider.gameObject.layer) & LayerMask.GetMask("Player")) != 0){
                        HitObject hitObject = new HitObject((hit.point - agent.transform.position).normalized, hit.point, damage, knockback, HitType.Melee);
                        hit.collider.gameObject.GetComponent<HealthController>().OnShot(hitObject);
                    }
                }
            }
        }
    }
}