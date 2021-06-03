using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates.ConstraintRanged;

public class ConstraintRangedBehavior : EnemyBehavior
{
    [SerializeField] public ConstraintRangedChase chaseState = new ConstraintRangedChase();
    [SerializeField] public ConstraintRangedPatrol patrolState = new ConstraintRangedPatrol();
    [SerializeField] public ConstraintRangedAttack attackState = new ConstraintRangedAttack();

    [FMODUnity.EventRef]
    public string selectsound;
    FMOD.Studio.EventInstance sound;

    private Rigidbody rb = null;

    protected override void Start(){
        base.Start();
        chaseState.SetBehaviour(this);
        patrolState.SetBehaviour(this);
        attackState.SetBehaviour(this);

        sound = FMODUnity.RuntimeManager.CreateInstance(selectsound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound, this.transform, rb);
        PlaySound();

        SetState(chaseState);
    }

    protected override void Update()
    {
        base.Update();
        
    }

    void PlaySound()
    {
        sound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));
        sound.start();
    }

    public void AnimHookAttack(){
        attackState.Attack();
    }

    public void AnimHookStopAttack(){
        attackState.StopAttack();
    }
}
