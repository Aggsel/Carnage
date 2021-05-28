using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actives/Mechanical/Replenish")]
public class A_Replenish : Active
{
    [Tooltip("The amount of health to be replenished.")]
    [SerializeField] private float replenishAmount = 0.3f;

    private HealthController hc;
    private CooldownController cc;
    private GameObject player;
    private AudioManager am = null;

    public override void Initialize(GameObject obj)
    {
        player = obj;
        hc = player.GetComponent<HealthController>();
        cc = player.GetComponentInChildren<CooldownController>(); ;
        am = AudioManager.Instance;
    }

    public override void TriggerActive()
    {
        hc.ModifyCurrentHealthProcent(replenishAmount);
        cc.active = null;
        cc.Initialize(null, player);
        am.PlaySound(ref am.itemsHealing);
    }

    public override void DetriggerActive()
    {

    }
}
