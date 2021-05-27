using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public class FiringController : MonoBehaviour
{
    [SerializeField] private Camera bulletCam = null;
    [SerializeField] private Transform muzzlePoint = null;
    [SerializeField] private ParticleSystem lineEffect = null;
    [SerializeField] private ParticleSystem cases = null;
    [SerializeField] private GameObject hitEffect = null;
    [SerializeField] private GameObject overheatObject = null;
    [SerializeField] private VisualEffect muzzleFlash = null;

    [SerializeField] private LayerMask shotLayerMask = 0;
    [SerializeField] private LayerMask hitEffectLayerMask = 0;
    //private ProjectileShotController psc;
    private Screenshake ss = null;
    private AttributeController attributeInstance = null;
    private AudioManager am = null;
    private float timeToFire = 0f;
    private bool overheated = false;

    void Start()
    {
        attributeInstance = this.gameObject.GetComponent<AttributeController>();
        ss = FindObjectOfType<Screenshake>();
        am = AudioManager.Instance;
    }

    void Update()
    {
        CheckFire();
    }

    void CheckFire()
    {
        if (Input.GetButton("Fire1") && Time.time >= timeToFire && overheated == false)
        {
            muzzleFlash.Play();
            timeToFire = Time.time + 1f / attributeInstance.weaponAttributesResultant.fireRate;
            FireWeapon();
        }
        else if (Input.GetButton("Fire1") && overheated == true)
        {
            am.PlaySound(am.playerShooting);
        }
    }

    void FireWeapon()
    {
        float accMultiplier = overheatObject.GetComponent<OverheatScript>().heatValue / attributeInstance.weaponAttributesResultant.heatMaximum;
        //accuracy applied
        Vector3 direction = bulletCam.transform.forward;
        direction.x += UnityEngine.Random.Range(-attributeInstance.weaponAttributesResultant.accuracy * accMultiplier, attributeInstance.weaponAttributesResultant.accuracy * accMultiplier);
        direction.y += UnityEngine.Random.Range(-attributeInstance.weaponAttributesResultant.accuracy * accMultiplier, attributeInstance.weaponAttributesResultant.accuracy * accMultiplier);
        direction.z += UnityEngine.Random.Range(-attributeInstance.weaponAttributesResultant.accuracy * accMultiplier, attributeInstance.weaponAttributesResultant.accuracy * accMultiplier);
        RaycastHit bulletHit;

        lineEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        muzzlePoint.transform.LookAt(direction + muzzlePoint.transform.position);

        lineEffect.Play();

        //bullet cases
        cases.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ParticleSystem.MainModule main = cases.main;

        main.startRotationX = (bulletCam.transform.eulerAngles.x + UnityEngine.Random.Range(-12, 12)) * Mathf.Deg2Rad;
        main.startRotationY = (bulletCam.transform.eulerAngles.y + UnityEngine.Random.Range(-12, 12)) * Mathf.Deg2Rad;
        main.startRotationZ = bulletCam.transform.eulerAngles.z * Mathf.Deg2Rad;

        cases.Play();

        //recoil
        ss.RecoilCall();

        //play sound
        am.PlaySound(ref am.playerShooting);

        if (Physics.Raycast(bulletCam.transform.position, direction, out bulletHit, Mathf.Infinity, shotLayerMask))
        {
            //draw line
            Debug.DrawLine(bulletCam.transform.position, bulletHit.point, Color.green, 1.5f);
            bulletHit.transform.GetComponentInParent<EnemyBehavior>()?.OnShot(new HitObject(transform.position, bulletHit.point, attributeInstance.weaponAttributesResultant.damage));
            //bulletHit.transform.GetComponentInParent<EnemyBehavior>()?.OnShot(new HitObject((bulletHit.point - bulletCam.transform.position).normalized, bulletHit.point, attributeInstance.weaponAttributesResultant.damage));
            if (((1 << bulletHit.transform.gameObject.layer) & hitEffectLayerMask) != 0)
            {
                GameObject impact = Instantiate(hitEffect, bulletHit.point + bulletHit.normal * 0.2f, Quaternion.LookRotation(bulletHit.normal));
                Destroy(impact, 2f);
            }
        }
        overheatObject.GetComponent<OverheatScript>().Heat(attributeInstance.weaponAttributesResultant.heatGeneration);
    }

    public void Overheated()
    {
        overheated = true;
        am.SetParameterByName(ref am.playerShooting, "Gun Upgrades", 1.0f);
    }

    public void Cooled()
    {
        overheated = false;
        am.SetParameterByName(ref am.playerShooting, "Gun Upgrades", 0.0f);
    }

    public void InitializeProjectile()
    {

    }

}