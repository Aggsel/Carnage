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
    private float currentHealth = 0.0f;
    private MovementController movementController;
    private FiringController firingController;
    private RawImage damageIndicator;
    private AudioManager am;
    private AttributeController attributeInstance;
    private UIController uiController;
    private Slider healthbarSlider;
    [SerializeField] private Viewbob viewBob;
    [SerializeField] private WeaponSway weaponSway;
    [SerializeField] private GameObject bloodImageGO;

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
        StartCoroutine(FadeImage(true));
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade the overlay
        if (fadeAway)
        {
            for (float i = 1.3f; i >= 0.0f; i -= Time.deltaTime)
            {
                damageIndicator.color = new Color(damageIndicator.color.r, damageIndicator.color.g, damageIndicator.color.b, i);
                yield return null;
            }
        }
    }

    private void Start() {
        am = AudioManager.Instance;
        attributeInstance = this.gameObject.GetComponent<AttributeController>();
        bloodImageGO.SetActive(true);
        damageIndicator = bloodImageGO.GetComponent<RawImage>();
        movementController = GetComponent<MovementController>();
        firingController = GetComponent<FiringController>();
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();

        if (bloodImageGO == null)
        {
            Debug.LogWarning("Missing damage indicator reference!");
        }
        else
        {
            damageIndicator.color = new Color(damageIndicator.color.r, damageIndicator.color.g, damageIndicator.color.b, 0.0f);
        }

        if(viewBob == null)
            viewBob = GetComponentInChildren<Viewbob>();
        if(weaponSway == null)
            weaponSway = GetComponentInChildren<WeaponSway>();
        SetMaxHealth(attributeInstance.weaponAttributesResultant.health);
    }

    private void CheckDeathCriteria(){
        if(currentHealth <= 0.0f){
            Die();
        }
    }

    private void Die(){
        movementController.enabled = false;
        firingController.enabled = false;
        viewBob.enabled = false;
        weaponSway.enabled = false;
        am.PlaySound(am.playerDeath);
        StartCoroutine("DeathEffects");
    }

    private IEnumerator DeathEffects(){
        yield return new WaitForSeconds(7.5f);
        SceneManager.LoadScene(1);
    }
}
