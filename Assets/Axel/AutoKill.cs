using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKill : EnemyBase
{
    [SerializeField] private float timeToKill = 4.0f;
    
    void Update(){
        timeToKill -= Time.deltaTime;
        if(timeToKill <= 0){
            Destroy(this.gameObject);
        }
    }
}