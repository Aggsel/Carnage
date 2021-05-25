using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image activeImage = null;
    //[SerializeField] private Image frameImage = null;

    [Header("Other stuff?")]
    public Active active;
    [SerializeField] private GameObject player = null;
    private UIController uic = null;
    private GameObject flashImageGO = null;
    private RawImage flashImage = null;
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
        uic = GameObject.Find("Game Controller Controller/Canvas").GetComponent<UIController>();
        flashImageGO = GameObject.Find("Game Controller Controller/Canvas/FlashImage");
        flashImage = flashImageGO.GetComponent<RawImage>();
        Initialize(active, player);
    }

    public void Initialize(Active selectedActive, GameObject player)
    {
        DeTrigger();
        active = selectedActive;
        if(active != null)
        {
            activeImage.enabled = true;
            activeImage.sprite = active.sprite;
            cooldownDuration = active.cooldown;
            activeActuationTime = active.buffTime;
            activeActuationTimeLeft = activeActuationTime;
            cooldownTimeLeft = 0.0f;
            activeActuated = false;
            readyTime = 0.0f;
            activeImage.fillAmount = 1.0f - (cooldownTimeLeft / cooldownDuration);
            active.Initialize(player);
        }
        else
        {
            activeImage.enabled = false;
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
            activeImage.fillAmount = (activeActuationTimeLeft / activeActuationTime);
        }
    }

    private void Triggered()
    {
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0.0f);
        uic.StartCoroutine(uic.FadeImage(flashImage, 0.85f, true));
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
        activeImage.color = new Color32(255, 255, 255, 255);
    }

    private void Cooldown()
    {
        cooldownTimeLeft -= Time.deltaTime;
        float roundedFloat = Mathf.Round(cooldownTimeLeft);
        activeImage.fillAmount = 1.0f - (cooldownTimeLeft / cooldownDuration);
        activeImage.color = new Color32(100, 100, 100, 255);
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