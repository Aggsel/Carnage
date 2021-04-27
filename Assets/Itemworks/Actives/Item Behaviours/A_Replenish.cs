using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Replenish : Active
{
    [Tooltip("Choose between Heat or Health values to replenish.")]
    [SerializeField] private string toReplenish = "health";
    [Tooltip("The amount of health to be replenished.")]
    [SerializeField] private float replenishAmount = 0f;

    private AttributeController ac;

    public override void Initialize(GameObject obj)
    {
        ac = obj.GetComponent<AttributeController>();
    }

    public override void TriggerActive()
    {
        
    }

    public override void DetriggerActive()
    {

    }
}
