using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEvent : Passive
{
    [Tooltip("Which event should trigger this lifesteal.")]
    [SerializeField] private GameEvent triggerEvent;

    public override void Initialize(GameObject obj){
        triggerEvent.Subscribe(OnEvent);
    }

    protected virtual void OnEvent(){}
    public override void TriggerPassive(){}
    public override void DeTriggerPassive(){}
    public override bool CheckValidity(){
        return false;
    }
}
