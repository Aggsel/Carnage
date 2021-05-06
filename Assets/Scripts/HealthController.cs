using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10.0f;
    [SerializeField] private MonoBehaviour[] deathDisableScripts = null;
    [SerializeField] private GameObject bloodImageGO = null;

    private float currentHealth = 0.0f; //dont remove
    private RawImage damageIndicator = null;
    private AudioManager am = null;
    private AttributeController attributeInstance = null;
    private UIController uiController = null;

    public void SetMaxHealth(float newMaxHealth){
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        uiController.SetMaxHealth(maxHealth);
        uiController.UpdateHealthbar();
    }

    public float Health
    {
        get
        {
            return currentHealth;
        }
    }

    public void ModifyCurrentHealth(float healthIncrease){
        currentHealth += healthIncrease;
        currentHealth = Mathf.Clamp(currentHealth, -1.0f, maxHealth);
        uiController.UpdateHealthbar();
        CheckDeathCriteria();
    }

    public void ModifyCurrentHealthProcent(float percentageIncrease){
        ModifyCurrentHealth(maxHealth * percentageIncrease);
    }

    public void OnShot(HitObject hit){
        ModifyCurrentHealth(-hit.damage);
        HideDamageIndicator();
        am.PlaySound(am.playerHurt);
    }

    private void HideDamageIndicator()
    {
        uiController.StartCoroutine(uiController.FadeImage(damageIndicator, 1.3f, true));
    }

    private void Start() {
        am = AudioManager.Instance;
        attributeInstance = this.gameObject.GetComponent<AttributeController>();
        bloodImageGO.SetActive(true);
        damageIndicator = bloodImageGO.GetComponent<RawImage>();
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();

        if (bloodImageGO == null)
        {
            Debug.LogWarning("Missing damage indicator reference!");
        }
        else
        {
            damageIndicator.color = new Color(damageIndicator.color.r, damageIndicator.color.g, damageIndicator.color.b, 0.0f);
        }

        SetMaxHealth(attributeInstance.weaponAttributesResultant.health);
    }

    private void CheckDeathCriteria(){
        if(currentHealth <= 0.0f){
            Die();
        }
    }

    private void Die(){

        for (int i = 0; i < deathDisableScripts.Length; i++)
        {
            deathDisableScripts[i].enabled = false;
        }

        //am.PlaySound(am.playerDeath); //detta ljudet är balle
        StartCoroutine("DeathEffects");
        am.StopSound(ref am.ambManager);
    }

    private IEnumerator DeathEffects(){
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }
}
