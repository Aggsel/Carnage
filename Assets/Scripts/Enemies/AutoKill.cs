using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKill : EnemyBehavior
{
    [SerializeField] private float timeToKill = 4.0f;
    
    protected override void Update(){
        timeToKill -= Time.deltaTime;
        if(timeToKill <= 0){
            Destroy(this.gameObject);
        }
    }
}