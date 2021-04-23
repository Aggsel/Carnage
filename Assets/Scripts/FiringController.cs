using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public class FiringController : MonoBehaviour
{
    [SerializeField] private Camera bulletCam = null;
    [SerializeField] private GameObject hitEffect = null;
    [SerializeField] private GameObject overheatObject = null;
    [SerializeField] private VisualEffect muzzleFlash = null;

    private ProjectileShotController psc;
    private Screenshake ss;

    private int bitmask;
    private AttributeController attributeInstance;
    private float timeToFire = 0f;
    private bool overheated = false;

    void Start()
    {
        GameObject player = this.gameObject;
        attributeInstance = player.GetComponent<AttributeController>();
        ss = FindObjectOfType<Screenshake>();
        int playerLayer = 12;
        bitmask = ~(1 << playerLayer);
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

        //recoil
        ss.RecoilCall();

        if (Physics.Raycast(bulletCam.transform.position, direction, out bulletHit, Mathf.Infinity, bitmask))
        {
            //draw line
            Debug.DrawLine(bulletCam.transform.position, bulletHit.point, Color.green, 1.5f);
            TargetScript target = bulletHit.transform.GetComponent<TargetScript>();

            if(target != null)
            {
                target.TakeDamage(attributeInstance.weaponAttributesResultant.damage);
            }
            bulletHit.transform.GetComponent<EnemyBehavior>()?.OnShot(new HitObject((bulletHit.point - bulletCam.transform.position).normalized, bulletHit.point));
            GameObject impact = Instantiate(hitEffect, bulletHit.point + bulletHit.normal * 0.2f, Quaternion.LookRotation(bulletHit.normal));
            Destroy(impact, 2f);
        }
        overheatObject.GetComponent<OverheatScript>().Heat(attributeInstance.weaponAttributesResultant.heatGeneration);
    }

    public void Overheated()
    {
        overheated = true;
    }

    public void Cooled()
    {
        overheated = false;
    }

    public void InitializeProjectile()
    {

    }

}
