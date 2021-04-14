using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKill : MonoBehaviour
{
    [SerializeField] private float timeToKill = 4.0f;
    public EnemySpawnPoint parentSpawn;
    
    void Update()
    {
        timeToKill -= Time.deltaTime;
        if(timeToKill <= 0){
            parentSpawn.ReportDeath(this);
            Destroy(this.gameObject);
        }
    }
}