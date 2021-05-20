using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType{
    Shot,
    Melee
}

public class HitObject
{
    public float damage = 0.0f;
    public float knockback = 0.0f;
    public Vector3 shotDirection;
    public Vector3 hitPosition;
    public HitType type;

    /// <summary>
    /// Constructor for HitObject, used as a container for information transfer between player and enemies when attacking and taking damage.
    /// </summary>
    /// <param name="shotDirection">Direction from the attacker to the attackee.</param>
    /// <param name="hitPosition">Hit position on the receiving entity. If player attacks enemy, this is the point on the enemy where the players attack hit.</param>
    /// <param name="damage">How much damage the attack should deal to the receiving entity.</param>
    /// <param name="knockback">How much knockback the receiving entity should receive.</param>
    public HitObject(Vector3 shotDirection, Vector3 hitPosition, float damage = 0.0f, float knockback = 0.0f, HitType type = HitType.Shot){
        this.shotDirection = shotDirection;
        this.hitPosition = hitPosition;
        this.damage = damage;
        this.knockback = knockback;
        this.type = type;
    }
}
