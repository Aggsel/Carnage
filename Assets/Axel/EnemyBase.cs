using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public EnemySpawnPoint parentSpawn;

    void OnDestroy(){
        parentSpawn?.ReportDeath(this);
    }
}
