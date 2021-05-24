using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("The speed of the projectile....")]
    [SerializeField] private float speedMin, speedMax = 30.0f;
    [Tooltip("For fast the projectile accelerates per second")]
    [SerializeField] private float accelerationSpeed = 5.0f;
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private GameObject hitDecal = null;
    [SerializeField] private GameObject hitEffect = null;
    [SerializeField] private LayerMask hitEffectLm = 0;
    [SerializeField] Rigidbody rb = null;
    [HideInInspector] public GameObject sourceEnemy;

    Collider col = null;
    float speed = 0.0f;

    void OnEnable(){
        speed = speedMin;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if(col == null)
            col = GetComponent<Collider>();
        if (hitDecal == null)
            Debug.LogWarning("No hitdecal is set for fireball!");
    }

    void Update(){
        speed += Time.deltaTime * accelerationSpeed;
        speed = Mathf.Clamp(speed, speedMin, speedMax);
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter (Collision other)
    {
        if (other.transform.parent == null)
            return;
        if (other.transform.parent.gameObject == sourceEnemy)
            return;
        if (other.gameObject.layer == 12)
        {
            Vector3 shotdir = rb.velocity.normalized;
            other.gameObject.GetComponent<HealthController>()?.OnShot(new HitObject(shotdir, transform.position, damage));
        }

        if ((hitEffectLm.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            //decal
            GameObject newDecal = Instantiate(hitDecal) as GameObject;
            newDecal.transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(-other.contacts[0].normal));
            newDecal.transform.SetParent(other.transform, true);

            float ranRot = Random.Range(-180, 180);
            newDecal.transform.RotateAround(newDecal.transform.position, newDecal.transform.forward, ranRot);

            //HitEffect
            GameObject newEffect = Instantiate(hitEffect) as GameObject;
            newEffect.transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(-other.contacts[0].normal));
            newEffect.transform.SetParent(other.transform, true);

            Destroy(newEffect, 2f);
        }

        Destroy(this.gameObject);
    }

    /*void OnTriggerEnter(Collider other){
        if(other.transform.parent == null)
            return;
        if(other.transform.parent.gameObject == sourceEnemy)
            return;
        if(other.gameObject.layer == 12){
            Vector3 shotdir = rb.velocity.normalized;
            other.gameObject.GetComponent<HealthController>()?.OnShot(new HitObject(shotdir, transform.position, damage));
        }

        if((hitEffectLm.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                GameObject newDecal = Instantiate(hitDecal) as GameObject;
                newDecal.transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(hit.normal));
                newDecal.transform.SetParent(other.transform, true);

                float ranRot = Random.Range(-180, 180);
                newDecal.transform.RotateAround(newDecal.transform.position, newDecal.transform.forward, ranRot);
            }
        }

        Destroy(this.gameObject);
    }*/
}
