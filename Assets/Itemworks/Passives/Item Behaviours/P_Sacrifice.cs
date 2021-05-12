using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Sacrifice")]
public class P_Sacrifice : Passive {

    private AttributeController attributeController;
    private HealthController healthController;

    [Header("Custom Parameters")]
    [Tooltip("How much HP to sacrifice upon pickup for another buff.")]
    [SerializeField] private float hpSacrificeAmount = 0.2f;

    [SerializeField] private string attribute = "";
    [SerializeField] private float amount = 0.0f;

    public override void Initialize(GameObject obj){
        attributeController = obj.GetComponent<AttributeController>();
        healthController = obj.GetComponent<HealthController>();

        healthController.SetMaxHealth(attributeController.weaponAttributesResultant.health * (1.0f - hpSacrificeAmount));
        attributeController.AddBuff(attribute, amount);
    }

    public override void TriggerPassive(){}
    public override void DeTriggerPassive(){}

    public override bool CheckValidity(){
        return false;
    }
}
