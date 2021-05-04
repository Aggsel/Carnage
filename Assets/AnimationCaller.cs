using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCaller : MonoBehaviour
{
    private AudioManager am = null;

    private void Start()
    {
        am = AudioManager.Instance;
    }

    #region Enemies
    //main fiend
    public void FiendFootSteps ()
    {
        //am.SetParameterByName(ref am.patientFootsteps, "surface", 1.0f);
        am.PlaySound(ref am.patientFootsteps, transform.gameObject);
    }

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
    #endregion

    //player
    public void PlayerMeleeStart ()
    {
        MeleeController mc = GetComponentInParent<MeleeController>();
        mc.StartMelee();
    }

    public void PlayerMeleeReset ()
    {
        MeleeController mc = GetComponentInParent<MeleeController>();
        mc.ResetMelee();
    }
}
