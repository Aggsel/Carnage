using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private EnemySpawnPoint parentSpawn;
    protected EnemyState currentState = null;
    [Header("Blood Decals")]
    [SerializeField] GameObject bloodDecalProjector = null;
    [Tooltip("A value of 0.0f will set the decals rotation to continue from the shot direction. A value of 1.0f will rotate the decals to face straight down.")]
    [Range(0.0f,1.0f)]
    [SerializeField] private float decalRotation = 0.35f;

    public void SetState(EnemyState newState){
        currentState?.OnStateExit();
        this.currentState = newState;
        currentState.OnStateEnter();
    }

    public virtual void OnShot(HitObject hit){
        if(bloodDecalProjector != null) 
            Instantiate(bloodDecalProjector, transform.position, Quaternion.Lerp(Quaternion.LookRotation(hit.shotDirection, Vector3.up), Quaternion.Euler(new Vector3(90, 0, 0)), decalRotation));
    }
    
    private void OnDestroy(){
        parentSpawn?.ReportDeath(this);
    }

    public void SetParentSpawn(EnemySpawnPoint newSpawn){
        this.parentSpawn = newSpawn;
    }
}