using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShotController : MonoBehaviour
{
    [HideInInspector] public Rigidbody projectile;
    [HideInInspector] public float projectileForce = 250f;
    private Transform bulletSpawn;

    void Start()
    {
        bulletSpawn = GameObject.Find("muzzleFlash").transform; //skitdåligt
    }

    public void Launch()
    {
        //Instantiate a copy of our projectile and store it in a new rigidbody variable called clonedBullet
        Rigidbody clonedBullet = Instantiate(projectile, bulletSpawn.position, transform.rotation) as Rigidbody;

        //Add force to the instantiated bullet, pushing it forward away from the bulletSpawn location, using projectile force for how hard to push it away
        clonedBullet.AddForce(bulletSpawn.transform.forward * -projectileForce);
    }

}
