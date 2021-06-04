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

    private void Start ()
    { 
        particle = GetComponent<ParticleSystem>();

        switch (bloodType)
        {
            //kinda hardcoded per what type of particle
            case BloodType.NORMAL:
                bc.SpawnBloodOptimized(1f, 1.0f, 1.5f, gameObject);
                break;
            case BloodType.DEATH:
                bc.SpawnBloodOptimized(1f, 1.0f, 1.5f, gameObject);
                break;
            default:
                break;
        }
    }

    public void SetBloodController (BloodController bc)
    {
        this.bc = bc;
    }
}