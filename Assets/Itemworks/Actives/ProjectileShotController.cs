using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShotController : MonoBehaviour
{
    [HideInInspector] public Rigidbody projectile;
    [HideInInspector] public float projectileForce = 250f;
    private Transform bulletSpawn;
    private Screenshake ss = null;

    void Start()
    {
        ss = FindObjectOfType<Screenshake>();
        bulletSpawn = GameObject.Find("muzzleFlash").transform; //skitdåligt
    }

    public void Launch()
    {
        //Instantiate a copy of our projectile and store it in a new rigidbody
        Rigidbody clonedBullet = Instantiate(projectile, bulletSpawn.position, transform.rotation) as Rigidbody;

        //Add force to the instantiated bullet
        clonedBullet.AddForce(bulletSpawn.transform.forward * -projectileForce);
        clonedBullet.GetComponent<MissileBehaviour>().player = this.gameObject;

        ss.RecoilCall();
        ss.StartCoroutine(ss.Shake(1.2f, 0.1f));
    }

}
