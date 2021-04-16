using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;

    public virtual void OnShot(){
    }
    
    private void OnDestroy(){
        parentSpawn?.ReportDeath(this);
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }
}
