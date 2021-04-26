using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCaller : MonoBehaviour
{
    //melee fiend
    public void FiendMeleeAttack ()
    {
        //Debug.Log("START ATTACK");
        EnemyBehavior eb = GetComponentInParent<EnemyBehavior>();
        eb.attackState.Attack();
    }

    public void FiendMeleeAttackStop ()
    {
        EnemyBehavior eb = GetComponentInParent<EnemyBehavior>();
        eb.attackState.StopAttack();
    }

    //Ranged fiend
    public void FiendRangedAttack ()
    {
        EnemyBehavior eb = GetComponentInParent<EnemyBehavior>();
        eb.rangedAttackState.RangedAttack();
    }

    public void FiendRangedAttackStop ()
    {
        EnemyBehavior eb = GetComponentInParent<EnemyBehavior>();
        eb.rangedAttackState.StopRangedAttack();
    }
}
