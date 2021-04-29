using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Assign: ")]
    [SerializeField] private Slider healthbar;
    [SerializeField] private Slider dashCharges;
    [SerializeField] private Slider overheatbar;

    [Header("Assign scripts")]
    [SerializeField] private HealthController hc;
    [SerializeField] private MovementController mc;
    [SerializeField] private OverheatScript oc;

    public void SetMaxHealth(float maxHealth)
    {
        healthbar.maxValue = maxHealth;
    }

    public void UpdateHealthbar()
    {
        healthbar.value = hc.Health;
    }

    public void SetMaxDashcharge(int maxCharges)
    {
        dashCharges.maxValue = maxCharges;
    }

    public void SetMaxHeat(float maxHeat)
    {
        overheatbar.maxValue = maxHeat;
    }

    void Update()
    {
        dashCharges.value = mc.Charge;
        overheatbar.value = oc.HeatValue;
    }
}
