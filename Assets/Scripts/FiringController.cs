using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public class FiringController : MonoBehaviour
{
    
    [SerializeField] private Camera bulletCam = null;
    [SerializeField] private GameObject hitEffect = null;
    [SerializeField] private GameObject overheatBar = null;
    [SerializeField] private VisualEffect muzzleFlash = null;
    private int bitmask;
    private AttributeController attributeInstance;
    private float timeToFire = 0f;
    private bool overheated = false;


    void Start()
    {
        GameObject player = this.gameObject;
        attributeInstance = player.GetComponent<AttributeController>();
        int playerLayer = 9;
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
        //accuracy applied
        Vector3 direction = bulletCam.transform.forward;
        direction.x += UnityEngine.Random.Range(-attributeInstance.weaponAttributesResultant.accuracy, attributeInstance.weaponAttributesResultant.accuracy);
        direction.y += UnityEngine.Random.Range(-attributeInstance.weaponAttributesResultant.accuracy, attributeInstance.weaponAttributesResultant.accuracy);
        direction.z += UnityEngine.Random.Range(-attributeInstance.weaponAttributesResultant.accuracy, attributeInstance.weaponAttributesResultant.accuracy);
        RaycastHit bulletHit;
        if (Physics.Raycast(bulletCam.transform.position, direction, out bulletHit, Mathf.Infinity, bitmask))
        {
            //draw line      
            Debug.DrawLine(bulletCam.transform.position, bulletHit.point, Color.green, 1.5f);
            TargetScript target = bulletHit.transform.GetComponent<TargetScript>();
            if(target != null)
            {
                target.TakeDamage(attributeInstance.weaponAttributesResultant.damage);
            }
            GameObject impact = Instantiate(hitEffect, bulletHit.point + bulletHit.normal * 0.2f, Quaternion.LookRotation(bulletHit.normal));
            Destroy(impact, 2f);
        }

        overheatBar.GetComponent<OverheatScript>().Heat(attributeInstance.weaponAttributesResultant.heatGeneration);

    }

    public void Overheated()
    {
        overheated = true;
    }

    public void Cooled()
    {
        overheated = false;
    }
}
