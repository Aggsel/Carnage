using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : EnemyBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject player;
    private float health = 10.0f;

    void OnEnable(){
        player = GameObject.Find("Player");
    }

    public override void OnShot(){
        agent.SetDestination(player.transform.position);

        this.health -= 5.0f;

        if(CheckDeathCriteria())
            Destroy(this.gameObject);
    }

    private bool CheckDeathCriteria(){
        return this.health < 0.0f;
    }
}
