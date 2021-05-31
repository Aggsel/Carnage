using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    private enum BloodType { NORMAL, DEATH}

    [SerializeField] private BloodType bloodType = BloodType.NORMAL;
    private ParticleSystem particle = null;
    private List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

    private BloodController bc = null;
    private float spawnProcentage = 0;

    private void Start ()
    { 
        particle = GetComponent<ParticleSystem>();

        switch (bloodType)
        {
            //kinda hardcoded per what type of particle
            case BloodType.NORMAL:
                bc.SpawnBloodOptimized(0.05f, 1.0f, 0.0f, gameObject);
                break;
            case BloodType.DEATH:
                bc.SpawnBloodOptimized(0.25f, 1.5f, 0.5f, gameObject);
                break;
            default:
                break;
        }
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