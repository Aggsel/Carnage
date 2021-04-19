using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPractice : EnemyMeleeBehavior
{
    public override void OnShot(){
        Destroy(this.gameObject);
    }
}
