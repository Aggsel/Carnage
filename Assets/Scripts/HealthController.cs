using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10.0f;
    private float currentHealth = 0.0f;

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
    }

    private void CheckDeathCriteria(){
        if(currentHealth <= 0.0f){
            //kys
        }
    }

    void OnGUI(){
        GUI.Label(new Rect(10, 80, 100, 50), currentHealth.ToString("F2"));
    }
}
