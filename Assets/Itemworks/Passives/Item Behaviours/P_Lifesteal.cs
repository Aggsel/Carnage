using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Lifesteal")]
public class P_Lifesteal : PassiveEvent
{
    enum Type{
        Absolute,
        PercentOfDamage,
        PercentOfMaxHealth
    }
    [Header("Custom Parameters")]
    [Tooltip(@"* Absolute - Player will always receive a fixed amount of health.
* Percent Of Damage - The amount of health received is a percentage of the current players damage.
* Percent Of MaxHealth - The amount of health received is a percentage of the current players max health.")]
    [SerializeField] private Type type = Type.Absolute;

    [Tooltip(@"How much health the player should receive, dependant on what type of lifesteal.
* Absolute - How many HP the player should receive.
* Percent Of Damage - What percentage of the players current damage should come back as health.
* Percent Of MaxHealth - What percentage of the players max health should come back as health.")]
    [SerializeField] private float lifestealAmount = 0.0f;
    [Range(0.0f, 1.0f)]
    [Tooltip("What chance to trigger the lifesteal upon triggering the event.")]
    [SerializeField] private float lifestealChance = 0.0f;
    
    private HealthController healthController;
    private AttributeController attributeController;

    public override void Initialize(GameObject obj){
        base.Initialize(obj);

        healthController = obj.GetComponent<HealthController>();
        attributeController = obj.GetComponent<AttributeController>();
    }

    protected override void OnEvent(){
        if(Random.Range(0.0f, 1.0f) >= lifestealChance)
            return;

        switch(type){
            case Type.Absolute:
                healthController.ModifyCurrentHealth(lifestealAmount);
                break;
            case Type.PercentOfDamage:
                healthController.ModifyCurrentHealth(attributeController.weaponAttributesResultant.damage * lifestealAmount);
                break;
            case Type.PercentOfMaxHealth:
                healthController.ModifyCurrentHealth(attributeController.weaponAttributesResultant.health * lifestealAmount);
                break;
        }
    }
}