using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Overheat Buff")]
public class P_OverheatBuff : Passive {
    [Header("Custom Parameters")]
    [Tooltip("Which attribute to affect.")]
    [SerializeField] private string attribute;
    [Tooltip("How much to affect the specified attribute (percent).")]
    [SerializeField] private float amount = 0.0f;
    [Tooltip("Min and max values for when this buff should be applied. x = 0.3, y = 0.7 means this buff will be active while overheat is between 30% and 70%.")]
    [SerializeField] private Vector2 overheatRange = new Vector2();

    private AttributeController attributeController;
    private OverheatScript overheatScript;
    private Buff currentBuff;
    private bool isActive = false;

    public override void Initialize(GameObject obj) {
        attributeController = obj.GetComponent<AttributeController>();
        overheatScript = obj.GetComponentInChildren<OverheatScript>();
    }

    public override void TriggerPassive() {
        float normalizedOverheat = overheatScript.HeatValue / attributeController.weaponAttributesResultant.heatMaximum;
        if(normalizedOverheat > overheatRange.x && normalizedOverheat < overheatRange.y)
            ActivateBuff();
        else
            DeactivateBuff();
    }

    private void ActivateBuff(){
        if(isActive)
            return;
        currentBuff = attributeController.AddBuff(attribute, amount);
        isActive = true;
    }

    private void DeactivateBuff(){
        if(!isActive)
            return;
        attributeController.RemoveBuff(currentBuff);
        isActive = false;
    }

    public override bool CheckValidity() {
        return true;
    }

    public override void DeTriggerPassive() {}
}