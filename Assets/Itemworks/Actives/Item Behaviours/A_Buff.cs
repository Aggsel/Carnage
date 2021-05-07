using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Actives/Buffs")]
public class A_Buff : Active
{
    [Tooltip("The array of all buffs you want this item to apply.")]
    [SerializeField] private Buff[] buffs;
    private Buff[] references = null;
    private AttributeController ac;

    public override void Initialize(GameObject obj)
    {
        ac = obj.GetComponent<AttributeController>();
        references = new Buff[buffs.Length];
    }
    
    public override void TriggerActive()
    {
        int count = 0;
        foreach (Buff i in buffs)
        {
            if (i.statTwo != "")
            {
                references[count] = ac.AddBuff(i.stat, i.statTwo, i.increment, i.incrementTwo);
            }
            else
            {
                references[count] = ac.AddBuff(i.stat, i.increment);
            }
            count++;
        }
    }

    public override void DetriggerActive()
    {
        foreach (Buff i in references)
        {
            if(i != null)
            {
                ac.RemoveBuff(i);
            }
        }
    }
}
