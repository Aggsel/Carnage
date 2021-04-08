using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatScript : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private WeaponAttributes weaponInstance = null;
    [SerializeField] private Gradient gradient = null;
    [SerializeField] private Image fill = null;
    private bool recentlyHeated = false;
    private bool overheated = false;
    private float coolingInitializeRemaining = 0f;

    public float incrementedCoolingInitialize = 0f;
    public float incrementedCoolingRate = 0f;

    void Start()
    {
        weaponInstance = player.GetComponent<FiringController>().weaponAttributes;
        slider.maxValue = weaponInstance.heatMaximum;
        slider.value = 0f;
    }

    void Update()
    {
        slider.maxValue = weaponInstance.heatMaximum;
        if(recentlyHeated == true)
        {
            coolingInitializeRemaining -= Time.deltaTime;
            if(coolingInitializeRemaining <= 0f)
            {
                recentlyHeated = false;
            }
        }
        else
        {
            slider.value -= weaponInstance.coolingRate * Time.deltaTime;
            fill.color = gradient.Evaluate(slider.normalizedValue);
            if(slider.value == 0 && overheated == true)
            {
                player.GetComponent<FiringController>().Cooled();
                overheated = false;
                weaponInstance.coolingIntializeTime -= incrementedCoolingInitialize;
                weaponInstance.coolingRate -= incrementedCoolingRate;

            }
        }

    }

    public void Heat(float heatGeneration)
    {
        slider.value += heatGeneration;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(slider.value >= weaponInstance.heatMaximum)
        {
            incrementedCoolingInitialize = weaponInstance.coolingIntializeTime * 1.75f - weaponInstance.coolingIntializeTime;
            incrementedCoolingRate = weaponInstance.coolingRate * 1.5f - weaponInstance.coolingRate;
            weaponInstance.coolingIntializeTime += incrementedCoolingInitialize;
            weaponInstance.coolingRate += incrementedCoolingRate;
            player.GetComponent<FiringController>().Overheated();
            overheated = true;
        }
        recentlyHeated = true;
        coolingInitializeRemaining = weaponInstance.coolingIntializeTime;
    }

}
