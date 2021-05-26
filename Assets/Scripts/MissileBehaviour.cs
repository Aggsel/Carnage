using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : MonoBehaviour
{
    public GameObject player;
    [SerializeField] LayerMask ignoreMask = 0;
    [SerializeField] GameObject explosionDecal = null;
    [SerializeField] private float blastRadius = 0.0f;
    [SerializeField] private GameObject explosionVFX = null;
    [SerializeField] private float explosionDamage = 0.0f;
    [SerializeField] private GameObject trailParticle = null;

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.name != "Player")
        {
            player.GetComponent<Screenshake>().StartCoroutine(player.GetComponent<Screenshake>().Shake(1.6f, 0.35f));
            ContactPoint contact = other.contacts[0];
            Instantiate(explosionVFX, transform.position, Quaternion.FromToRotation(Vector3.up, contact.normal));

            Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
            foreach (Collider obj in colliders)
            {
                if (obj.name == "Player")
                {
                    HealthController hc = obj.GetComponent<HealthController>();
                    hc.OnShot(new HitObject(transform.position - obj.transform.position, transform.position - obj.transform.position, explosionDamage, 1.0f));
                }
                else
                {
                    if (obj.GetComponentInParent<EnemyBehavior>() != null)
                    {
                        obj.GetComponentInParent<EnemyBehavior>().OnShot(new HitObject(obj.transform.position - transform.position, transform.position - obj.transform.position, 125.0f, 1.0f));
                        //shrug
                    }
                }
            }

            if ((ignoreMask.value & (1 << other.transform.gameObject.layer)) > 0)
            {
                GameObject newDecal = Instantiate(explosionDecal) as GameObject;
                newDecal.transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(contact.normal));
                newDecal.transform.SetParent(other.transform, true);

                float ranRot = Random.Range(-180, 180);
                newDecal.transform.RotateAround(newDecal.transform.position, newDecal.transform.forward, ranRot);
            }
            AudioManager.Instance.PlaySound(ref AudioManager.Instance.playerExplosion, this.transform.position);
            trailParticle.transform.SetParent(null, true);
            Destroy(gameObject);
        }
    }
}