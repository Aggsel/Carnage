using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10.0f;
    private float currentHealth = 0.0f;

    private MovementController movementController;
    private FiringController firingController;
    [SerializeField] private Viewbob viewBob;
    [SerializeField] private WeaponSway weaponSway;

    public void SetMaxHealth(float newMaxHealth){
        maxHealth = newMaxHealth;
    }

    public void ModifyCurrentHealth(float healthIncrease){
        currentHealth += healthIncrease;
        currentHealth = Mathf.Clamp(currentHealth, -1.0f, maxHealth);
        CheckDeathCriteria();
    }

    public void ModifyCurrentHealthProcent(float percentageIncrease){
        ModifyCurrentHealth(maxHealth * percentageIncrease);
    }

    public void OnShot(HitObject hit){
        ModifyCurrentHealth(-hit.damage);
    }

    private void Start() {
        this.currentHealth = maxHealth;
        movementController = GetComponent<MovementController>();
        firingController = GetComponent<FiringController>();

        if(viewBob == null)
            viewBob = GetComponentInChildren<Viewbob>();
        if(weaponSway == null)
            weaponSway = GetComponentInChildren<WeaponSway>();
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
        StartCoroutine("DeathEffects");
    }

    private IEnumerator DeathEffects(){
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }

    void OnGUI(){
        GUI.Label(new Rect(10, 80, 100, 50), currentHealth.ToString("F2"));
    }
}
