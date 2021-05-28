using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    private ParticleSystem particle = null;
    private List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

    private BloodController bc = null;

    private void Start ()
    { 
        particle = GetComponent<ParticleSystem>();
    }

    public void SetBloodController (BloodController bc)
    {
        this.bc = bc;
    }

    private void OnParticleCollision (GameObject other)
    {
        int colAmount = particle.GetCollisionEvents(other, colEvents);
        bc.SpawnBlood(colEvents[0].intersection, other);
    }
}