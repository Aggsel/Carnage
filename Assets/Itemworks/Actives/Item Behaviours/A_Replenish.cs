using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actives/Mechanical/Replenish")]
public class A_Replenish : Active
{
    [Tooltip("The amount of health to be replenished.")]
    [SerializeField] private float replenishAmount = 0.3f;

    private HealthController hc;

    public override void Initialize(GameObject obj)
    {
        hc = obj.GetComponent<HealthController>();
    }

    public override void TriggerActive()
    {
        hc.ModifyCurrentHealthProcent(replenishAmount);
    }

    public override void DetriggerActive()
    {

    }
}
