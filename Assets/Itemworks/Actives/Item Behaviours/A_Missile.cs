using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actives/Mechanical/Projectile")]
public class A_Missile : Active
{
    [Tooltip("The projectile prefab of which to shoot")]
    [SerializeField] private Rigidbody projectile = null;
    [Tooltip("The force which the projectile will exit the gun")]
    [SerializeField] private float projectileForce = 0f;

    private ProjectileShotController launcher;

    public override void Initialize(GameObject obj)
    {
        launcher = obj.GetComponent<ProjectileShotController>();
        launcher.projectileForce = projectileForce;
        launcher.projectile = projectile;
    }

    public override void TriggerActive()
    {
        launcher.Launch();
    }

    public override void DetriggerActive()
    {

    }
}
