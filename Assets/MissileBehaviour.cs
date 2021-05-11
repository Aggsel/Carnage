using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private float blastRadius;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float explosionDamage;

    void OnCollisionEnter(Collision other)
    {
        player.GetComponent<Screenshake>().StartCoroutine(player.GetComponent<Screenshake>().Shake(1.6f, 0.6f));
        ContactPoint contact = other.contacts[0];
        Instantiate(explosionVFX, transform.position, Quaternion.FromToRotation(Vector3.up, contact.normal));
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider obj in colliders){
            if(obj.name == "Player")
            {
                obj.GetComponent<HealthController>().ModifyCurrentHealth(-explosionDamage);
            }
            else
            {
                if(obj.GetComponentInParent<EnemyBehavior>() == null)
                {
                    //shrug
                }
                else
                {
                    obj.GetComponentInParent<EnemyBehavior>().OnShot(new HitObject(obj.transform.position - transform.position, transform.position - obj.transform.position, 125.0f, 1.0f));
                }
            }
        }
        Destroy(gameObject);
    }

}
