using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    private ParticleSystem particle = null;
    private List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

    private BloodController bc = null;
    private float spawnProcentage = 0;

    private void Start ()
    { 
        particle = GetComponent<ParticleSystem>();
        bc.SpawnBloodOptimized(gameObject);
    }

    public void SetBloodController (BloodController bc)
    {
        this.bc = bc;
        //spawnProcentage = bc.GetBloodSpawnProcentage();
    }

    //removed dynamic blood placement to improve performance
    /*private void OnParticleCollision (GameObject other)
    {
        int colAmount = particle.GetCollisionEvents(other, colEvents);

        float ran = Random.value;
        //Debug.Log(ran.ToString("F2") + " < " + (spawnProcentage / 100));

        if(ran < (spawnProcentage / 100))
        {
            bc.SpawnBlood(colEvents[0].intersection, other);
        }
    }
    */
}