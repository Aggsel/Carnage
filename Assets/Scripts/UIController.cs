using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private HealthController hc;
    [SerializeField]
    private MovementController mc;
    [SerializeField]
    private OverheatScript oc;

    private Slider healthbar;
    private Slider dashCharges;
    private Slider overheatbar;


    void Start()
    {
        healthbar = GameObject.FindGameObjectWithTag("Healthbar").GetComponent<Slider>();
        dashCharges = GameObject.FindGameObjectWithTag("Dashcharge").GetComponent<Slider>();
        overheatbar = GameObject.FindGameObjectWithTag("Overheatbar").GetComponent<Slider>();
    }

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
