using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatScript : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    private AttributeController attributeInstance;
    private UIController uiController;
    public float heatValue = 0f;
    private float heatMax = 0f;
    private bool recentlyHeated = false;
    private bool overheated = false;
    private float coolingInitializeRemaining = 0f;
    private Buff buffReferenceOne = null;


    void Start()
    {
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        attributeInstance = player.GetComponent<AttributeController>();
        
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
            heatValue = Mathf.Clamp(heatValue, 0f, heatMax);


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
        heatValue += heatGeneration;
        if(heatValue >= attributeInstance.weaponAttributesResultant.heatMaximum)
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

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 60, 100, 50), heatValue.ToString("F0"));
    }

}
