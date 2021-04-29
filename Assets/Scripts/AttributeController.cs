﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct WeaponAttributes
{
    [Tooltip("The flat damage of the weapon")]
    public float damage;
    [Tooltip("The flat firing rate of the weapon per second")]
    public float fireRate;
    [Tooltip("The amount of heat generated by each shot of the weapon")]
    public float heatGeneration;
    [Tooltip("The maximum heat-value before the weapon needs to cool")]
    public float heatMaximum;
    [Tooltip("The time needed for the weapon to START cooling")]
    public float coolingIntialize;
    [Tooltip("The rate at which the weapon cools after intitializing a cooling")]
    public float coolingRate;
    [Tooltip("The general disarray of accuracy whilst shooting, ramps up as the player shoots consecutively, higher value means a more innacuracte standard for the weapon")]
    public float accuracy;
    [Tooltip("The flat amount of health the player has")]
    public float health;
}

[Serializable]
public struct WeaponAttributesResultant
{
    [Tooltip("Base damage with additive buffs applied")]
    public float damage;
    [Tooltip("The base firing rate with all additive buffs applied")]
    public float fireRate;
    [Tooltip("Base heat generation with additive buffs applied")]
    public float heatGeneration;
    [Tooltip("Base heat-max values with all additive buffs applied")]
    public float heatMaximum;
    [Tooltip("Base coolingInitialize values with all additive buffs applied")]
    public float coolingInitialize;
    [Tooltip("Base coolingrate with all additive buffs applied")]
    public float coolingRate;
    [Tooltip("Base accuracy with all additive buffs applied")]
    public float accuracy;
    [Tooltip("Base health with buffs applied")]
    public float health;
}

public class Buff
{
    public string stat;
    public float increment;
    public string statTwo;
    public float incrementTwo;

    public Buff(string theStat, float value)
    {
        stat = theStat;
        increment = value;
    }
    public Buff(string firstStat, string secondStat, float firstValue, float secondValue)
    {
        stat = firstStat;
        statTwo = secondStat;
        increment = firstValue;
        incrementTwo = secondValue;
    }
}

public class AttributeController : MonoBehaviour
{
    [SerializeField] private WeaponAttributes weaponAttributesBase = new WeaponAttributes();
    [SerializeField] public WeaponAttributesResultant weaponAttributesResultant = new WeaponAttributesResultant();
    private List<Buff> buffList = new List<Buff>();

    void Awake()
    {
        Recalculate();
    }

    public Buff AddBuff(string stat, float increment)
    {
        Buff newBuff = new Buff(stat, increment);
        buffList.Add(newBuff);
        int number = buffList.Count;
        Buff item = buffList[number - 1];
        Recalculate();
        return item;
    }

    public Buff AddBuff(string stat, string secondStat, float increment, float secondIncrement)
    {
        Buff newBuff = new Buff(stat, secondStat, increment, secondIncrement);
        buffList.Add(newBuff);
        int number = buffList.Count;
        Buff item = buffList[number - 1];
        Recalculate();
        return item;
    }

    public void RemoveBuff(Buff objectRef)
    {
        if(buffList.Contains(objectRef))
        {
            buffList.Remove(objectRef);
            Recalculate();
        }
    }

    void Recalculate()
    {
        Rebase();
        foreach (Buff item in buffList)
        {
            switch (item.stat.Trim().ToLower())
            {
                case "damage":
                    float damageDiff = weaponAttributesBase.damage * item.increment - weaponAttributesBase.damage;
                    weaponAttributesResultant.damage += damageDiff;
                    break;
                case "firerate":
                    float firerateDiff = weaponAttributesBase.fireRate * item.increment - weaponAttributesBase.fireRate;
                    weaponAttributesResultant.fireRate += firerateDiff;
                    break;
                case "heatgeneration":
                    float heatgenerationDiff = weaponAttributesBase.heatGeneration * item.increment - weaponAttributesBase.heatGeneration;
                    weaponAttributesResultant.heatGeneration += heatgenerationDiff;
                    break;
                case "heatmaximum":
                    float heatmaximumDiff = weaponAttributesBase.heatMaximum * item.increment - weaponAttributesBase.heatMaximum;
                    weaponAttributesResultant.heatMaximum += heatmaximumDiff;
                    break;
                case "coolinginitialize":
                    float coolinginitializeDiff = weaponAttributesBase.coolingIntialize * item.increment - weaponAttributesBase.coolingIntialize;
                    weaponAttributesResultant.coolingInitialize += coolinginitializeDiff;
                    break;
                case "coolingrate":
                    float coolingrateDiff = weaponAttributesBase.coolingRate * item.increment - weaponAttributesBase.coolingRate;
                    weaponAttributesResultant.coolingRate += coolingrateDiff;
                    break;
                case "accuracy":
                    float accuracyDiff = weaponAttributesBase.accuracy * item.increment - weaponAttributesBase.accuracy;
                    weaponAttributesResultant.accuracy += accuracyDiff;
                    break;
                case "health":
                    float healthDiff = weaponAttributesBase.health * item.increment - weaponAttributesBase.health;
                    weaponAttributesResultant.health += healthDiff;
                    break;

            }

            if(item.statTwo != null)
            {
                switch (item.statTwo.Trim().ToLower())
                {
                    case "damage":
                        float damageDiff = weaponAttributesBase.damage * item.incrementTwo - weaponAttributesBase.damage;
                        weaponAttributesResultant.damage += damageDiff;
                        break;
                    case "firerate":
                        float firerateDiff = weaponAttributesBase.fireRate * item.incrementTwo - weaponAttributesBase.fireRate;
                        weaponAttributesResultant.fireRate += firerateDiff;
                        break;
                    case "heatgeneration":
                        float heatgenerationDiff = weaponAttributesBase.heatGeneration * item.incrementTwo - weaponAttributesBase.heatGeneration;
                        weaponAttributesResultant.heatGeneration += heatgenerationDiff;
                        break;
                    case "heatmaximum":
                        float heatmaximumDiff = weaponAttributesBase.heatMaximum * item.incrementTwo - weaponAttributesBase.heatMaximum;
                        weaponAttributesResultant.heatMaximum += heatmaximumDiff;
                        break;
                    case "coolinginitialize":
                        float coolinginitializeDiff = weaponAttributesBase.coolingIntialize * item.incrementTwo - weaponAttributesBase.coolingIntialize;
                        weaponAttributesResultant.coolingInitialize += coolinginitializeDiff;
                        break;
                    case "coolingrate":
                        float coolingrateDiff = weaponAttributesBase.coolingRate * item.incrementTwo - weaponAttributesBase.coolingRate;
                        weaponAttributesResultant.coolingRate += coolingrateDiff;
                        break;
                    case "accuracy":
                        float accuracyDiff = weaponAttributesBase.accuracy * item.incrementTwo - weaponAttributesBase.accuracy;
                        weaponAttributesResultant.accuracy += accuracyDiff;
                        break;
                    case "health":
                        float healthDiff = weaponAttributesBase.health * item.incrementTwo - weaponAttributesBase.health;
                        weaponAttributesResultant.health += healthDiff;
                        break;

                }
            }
        }
    }
    
    void Rebase()
    {
        weaponAttributesResultant.damage = weaponAttributesBase.damage;
        weaponAttributesResultant.fireRate = weaponAttributesBase.fireRate;
        weaponAttributesResultant.heatGeneration = weaponAttributesBase.heatGeneration;
        weaponAttributesResultant.heatMaximum = weaponAttributesBase.heatMaximum;
        weaponAttributesResultant.coolingInitialize = weaponAttributesBase.coolingIntialize;
        weaponAttributesResultant.coolingRate = weaponAttributesBase.coolingRate;
        weaponAttributesResultant.accuracy = weaponAttributesBase.accuracy;
        weaponAttributesResultant.health = weaponAttributesBase.health;
    }
}