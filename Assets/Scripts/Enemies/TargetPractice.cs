using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPractice : EnemyBehavior
{
    public override void OnShot(HitObject hit){
        Destroy(this.gameObject);
    }
}
