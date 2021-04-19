using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;
    protected EnemyState currentState = null;

    public void SetState(EnemyState newState){
        currentState?.OnStateExit();
        this.currentState = newState;
        currentState.OnStateEnter();
    }

    public virtual void OnShot(){
    }
    
    private void OnDestroy(){
        parentSpawn?.ReportDeath(this);
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }
}