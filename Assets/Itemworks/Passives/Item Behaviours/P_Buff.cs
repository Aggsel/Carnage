using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Buffs")]
public class P_Buff : Passive
{
    [Tooltip("The array of all buffs you want this item to apply.")]
    [SerializeField] public Buff[] buffs;

    private AttributeController ac;
    private bool validity;

    public override void Initialize(GameObject obj)
    {
        ac = obj.GetComponent<AttributeController>();
        validity = true;
    }

    public override void TriggerPassive()
    {
        foreach(Buff i in buffs)
        {
            if(i.statTwo != "")
            {
                ac.AddBuff(i.stat, i.statTwo, i.increment, i.incrementTwo);
            }
            else
            {
                ac.AddBuff(i.stat, i.increment);
            }  
        }
        validity = false;
    }

    public override void DeTriggerPassive()
    {
        foreach (Buff i in buffs)
        {
            ac.RemoveBuff(i);
        }
    }

    public override bool CheckValidity()
    {
        return validity;
    }
}