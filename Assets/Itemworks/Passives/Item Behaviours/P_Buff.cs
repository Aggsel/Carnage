using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Buffs")]
public class P_Buff : Passive
{
    [Tooltip("The first attribute which to buff or debuff.")]
    [SerializeField] public string attribute = "";
    [Tooltip("The second attribute which to buff or debuff.")]
    [SerializeField] public string secondAttribute = "";
    [Tooltip("The value of which to change the first attribute. A value of 1.25 means a 25% percent increase.")]
    [SerializeField] public float attributePercentage;
    [Tooltip("The value of which to change the second attribute. A value of 1.25 means a 25% percent increase.")]
    [SerializeField] public float secondAttributePercentage;

    private AttributeController ac;
    private Buff buffReference = null;
    private bool validity;

    public override void Initialize(GameObject obj)
    {
        Debug.Log("Initialized passive");
        ac = obj.GetComponent<AttributeController>();
        validity = true;

    }

    public override void TriggerPassive()
    {
        Debug.Log("Triggered Passive");
        if (secondAttribute != "")
        {
            buffReference = ac.AddBuff(attribute, secondAttribute, attributePercentage, secondAttributePercentage);
        }
        else
        {
            buffReference = ac.AddBuff(attribute, attributePercentage);
        }

        validity = false;

    }

    public override void DeTriggerPassive()
    {
        if (buffReference != null)
        {
            ac.RemoveBuff(buffReference);
        }
    }

    public override bool CheckValidity()
    {
        return validity;
    }
}