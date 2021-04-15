using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;

    private void OnDestroy(){
        parentSpawn?.ReportDeath(this);
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }
}
