using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Actives/Buffs")]
public class A_Buff : Active
{
    [Tooltip("The attribute which to buff when using this active.")]
    [SerializeField] private string attributeToBuff = "firerate";
    [Tooltip("Percentages to shift the attribute with, a value of 2.5 equals 250% modifier.")]
    [SerializeField] private float increment = 0f;


    private Buff reference = null;
    private AttributeController ac;

    public override void Initialize(GameObject obj)
    {
        ac = obj.GetComponent<AttributeController>();
    }
    
    public override void TriggerActive()
    {
        reference = ac.AddBuff(attributeToBuff, increment);
    }

    public override void DetriggerActive()
    {
        if(reference != null)
        {
            ac.RemoveBuff(reference);
        }
    }

}
