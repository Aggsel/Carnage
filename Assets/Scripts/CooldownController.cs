using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    public Active active;
    [SerializeField] private GameObject player = null;
    private float cooldownDuration;
    private float readyTime;
    private float cooldownTimeLeft;
    private float activeActuationTime;
    private float activeActuationTimeLeft;
    private bool activeActuated;
    private KeyCode activate;

    private void ReadKeybinds(KeyBindAsignments keys)
    {
        activate = keys.action;
    }

    private void Awake()
    {
        PauseController.updateKeysFunction += ReadKeybinds;
    }

    private void OnDestroy()
    {
        PauseController.updateKeysFunction -= ReadKeybinds;
    }

    void Start()
    {
        Initialize(active, player);
    }

    public void Initialize(Active selectedActive, GameObject player)
    {
        DeTrigger();
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
                if (Input.GetKeyDown(activate) && active != null)
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
        if(active != null)
        {
            activeActuated = false;
            activeActuationTimeLeft = activeActuationTime;
            active.DetriggerActive();
        }
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

    private void OnGUI ()
    {
        if (active != null)
        {
            if (activeActuated)
            {
                GUI.Label(new Rect(Screen.width - 125, Screen.height - 50, 125, 50), "Active Actuated: " + activeActuationTimeLeft.ToString("F0"));
            }
            else
            {
                GUI.Label(new Rect(Screen.width - 125, Screen.height - 50, 125, 50), "Active Cooldown: " + cooldownTimeLeft.ToString("F0"));
            }
        } 
    }
}