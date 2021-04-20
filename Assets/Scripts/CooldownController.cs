﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    public string abilityButton = "Fire1";
    public Active active;

    [SerializeField] private GameObject player;
    private float cooldownDuration;
    private float readyTime;
    private float cooldownTimeLeft;
    private float activeActuationTime;
    private float activeActuationTimeLeft;
    private bool activeActuated;

    void Start()
    {
        Initialize(active, player);
    }

    public void Initialize(Active selectedActive, GameObject player)
    {
        activeActuated = false;
        active = selectedActive;
        if(active != null)
        {
            cooldownDuration = active.cooldown;
            activeActuationTime = active.buffTime;
            activeActuationTimeLeft = activeActuationTime;
            cooldownTimeLeft = cooldownDuration;
            active.Initialize(player);
        }
        ActiveReady();
    }

    void Update()
    {
        if (!activeActuated)
        {
            bool cdFinished = (Time.time > readyTime);
            if (cdFinished)
            {
                ActiveReady();
                //Change input to controller-specific action button
                if (Input.GetKeyDown(KeyCode.E) && active != null)
                {
                    Triggered();
                }
            }
            else
            {
                Cooldown();
            }
        }
        else
        {
            ActuationCooldown();
        }
    }

    private void Triggered()
    {
        activeActuated = true;
        activeActuationTimeLeft = activeActuationTime;
        
        cooldownTimeLeft = cooldownDuration;
        active.TriggerActive();
    }

    private void DeTrigger()
    {
        activeActuated = false;
        activeActuationTimeLeft = activeActuationTime;
        active.DetriggerActive();
    }


    private void ActiveReady()
    {
        //UI stuff probably...
    }

    private void Cooldown()
    {
        cooldownTimeLeft -= Time.deltaTime;
        float roundedFloat = Mathf.Round(cooldownTimeLeft);
        //write to UI... probably...
    }

    private void ActuationCooldown()
    { 
        activeActuationTimeLeft -= Time.deltaTime;
        float roundedFloat = Mathf.Round(activeActuationTimeLeft);
        if (activeActuationTimeLeft <= 0.0)
        {
            DeTrigger();
            readyTime = cooldownDuration + Time.time;
        }
    }



}
