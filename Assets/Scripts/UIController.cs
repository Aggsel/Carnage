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
    private OverheatScript fc;

    private Slider healthbar;
    private Slider dashCharges;


    void Start()
    {
        healthbar = GameObject.FindGameObjectWithTag("Healthbar").GetComponent<Slider>();
        dashCharges = GameObject.FindGameObjectWithTag("Dashcharge").GetComponent<Slider>();
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

    void Update()
    {
        dashCharges.value = mc.Charge;
    }
}
