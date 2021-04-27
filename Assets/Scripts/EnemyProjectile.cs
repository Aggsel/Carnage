using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("The speed of the projectile....")]
    [SerializeField] private float projectileSpeed = 20.0f;
    [SerializeField] private float damage = 2.0f;
    [SerializeField] Rigidbody rb = null;
    [HideInInspector] public GameObject parent;
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
        if(other.gameObject == parent)
            return;
        if(other.gameObject.layer == 12){
            Vector3 shotdir = rb.velocity.normalized;
            other.gameObject.GetComponent<HealthController>()?.OnShot(new HitObject(shotdir, transform.position, damage));
        }
        Destroy(this.gameObject);
    }
}
