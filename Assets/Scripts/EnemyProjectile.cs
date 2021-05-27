using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("The min speed of the projectile....")]
    [SerializeField] private float speedMin = 30.0f;
    [Tooltip("The max speed of the projectile....")]
    [SerializeField] private float speedMax = 60.0f;
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

    [FMODUnity.EventRef]
    public string selectsound;
    FMOD.Studio.EventInstance sound;

    void OnEnable(){
        speed = speedMin;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if(col == null)
            col = GetComponent<Collider>();
        if (hitDecal == null)
            Debug.LogWarning("No hitdecal is set for fireball!");

        sound = FMODUnity.RuntimeManager.CreateInstance(selectsound);
    }

    void Update(){
        speed += Time.deltaTime * accelerationSpeed;
        speed = Mathf.Clamp(speed, speedMin, speedMax);
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound, this.transform, rb);
        FMOD.Studio.PLAYBACK_STATE fmodPbState;
        sound.getPlaybackState(out fmodPbState);
        if(fmodPbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            sound.start();
        }
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

            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            Destroy(newEffect, 2f);
        }

        AudioManager.Instance.StopSound(AudioManager.Instance.patientProjectile);
        Destroy(this.gameObject);
    }
}
