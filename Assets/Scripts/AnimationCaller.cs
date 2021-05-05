﻿using System.Collections;
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
    public void FiendMeleeAttack()
    {
        ConstraintMeleeBehavior eb = GetComponentInParent<ConstraintMeleeBehavior>();
        eb.AnimHookAttack();
    }

    public void FiendMeleeAttackStop()
    {
        ConstraintMeleeBehavior eb = GetComponentInParent<ConstraintMeleeBehavior>();
        eb.AnimHookStopAttack();
    }

    //Ranged fiend
    public void FiendRangedAttack()
    {
        ConstraintRangedBehavior eb = GetComponentInParent<ConstraintRangedBehavior>();
        eb.AnimHookAttack();
    }

    public void FiendRangedAttackStop()
    {
        ConstraintRangedBehavior eb = GetComponentInParent<ConstraintRangedBehavior>();
        eb.AnimHookStopAttack();
    }
    #endregion

    //player
    public void PlayerMeleeStart()
    {
        MeleeController mc = GetComponentInParent<MeleeController>();
        mc.StartMelee();
    }

    public void PlayerMeleeReset()
    {
        MeleeController mc = GetComponentInParent<MeleeController>();
        mc.ResetMelee();
    }
}