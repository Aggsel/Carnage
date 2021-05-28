using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatScript : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private AttributeController attributeInstance;
    private UIController uiController;
    public float heatValue = 0f;
    private float heatMax = 0f;
    public bool recentlyHeated = false;
    public bool overheated = false;
    private float coolingInitializeRemaining = 0f;
    private Buff buffReferenceOne = null;
    private AudioManager am = null;

    void Start()
    {
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        attributeInstance = player.GetComponent<AttributeController>();
        heatMax = attributeInstance.weaponAttributesResultant.heatMaximum;
        am = AudioManager.Instance;
        am.PlaySound(ref am.playerOverheat);
    }

    void Update()
    {
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
            heatValue -= attributeInstance.weaponAttributesResultant.coolingRate * Time.deltaTime;
            heatValue = Mathf.Clamp(heatValue, 0f, attributeInstance.weaponAttributesResultant.heatMaximum);
            am.SetParameterByName(ref am.playerOverheat, "Overheating", Mathf.Clamp((heatValue / 100.0f), 0.0f, 0.99f));

            if (heatValue == 0 && overheated == true)
            {
                player.GetComponent<FiringController>().Cooled();
                overheated = false;
                attributeInstance.RemoveBuff(buffReferenceOne);
            }
        }
    }

    public void Heat(float heatGeneration)
    {
        uiController.SetMaxHeat(attributeInstance.weaponAttributesResultant.heatMaximum);
        heatMax = attributeInstance.weaponAttributesResultant.heatMaximum;
        heatValue += heatGeneration;
        am.SetParameterByName(ref am.playerOverheat, "Overheating", Mathf.Clamp((heatValue / 100.0f), 0.0f, 0.99f));
        if (heatValue >= attributeInstance.weaponAttributesResultant.heatMaximum)
        {
            buffReferenceOne = attributeInstance.AddBuff("coolinginitialize", "coolingrate", 1.75f, 1.5f);
            player.GetComponent<FiringController>().Overheated();
            overheated = true;
        }
        recentlyHeated = true;
        coolingInitializeRemaining = attributeInstance.weaponAttributesResultant.coolingInitialize;
    }

    public float HeatValue
    {
        get
        {
            return heatValue;
        }
    }

    public float HeatPercentage
    {
        get
        {
            return heatValue / heatMax;
        }
    }
}
