using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("The speed of the projectile....")]
    [SerializeField] private float projectileSpeed = 20.0f;
    [SerializeField] private float damage = 2.0f;
    [SerializeField] Rigidbody rb = null;
    Collider col = null;

    void OnEnable(){
        if(rb == null)
            rb = GetComponent<Rigidbody>();
        if(col == null)
            col = GetComponent<Collider>();
    }

    void Update(){
        rb.MovePosition(transform.position + transform.forward * projectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == 12){
            other.gameObject.GetComponent<HealthController>()?.ModifyCurrentHealth(-2.0f);
        }
        Destroy(this.gameObject);
    }

    // private void OnCollisionExit(Collision other) {
    //     col.enabled = true;
    // }

    // void OnCollisionEnter(Collision collision){
    //     if(collision.collider.gameObject.layer == LayerMask.GetMask("Player"))
    //         GetComponent<HealthController>()?.ModifyCurrentHealth(-2.0f);
    //     Destroy(this.gameObject);
    // }
}
