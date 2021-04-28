using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Active : ScriptableObject
{
    [Tooltip("The name of the active, currently not used anywhere.")]
    [SerializeField] public string activeName = "New active";
    [Tooltip("The cooldown of the active in seconds, added with buffTime gives resultant effective cooldown.")]
    [SerializeField] public float cooldown = 1f;
    [Tooltip("The amount of time for which the active remains on. Added with cooldown gives resultant effective cooldown. Put to 0 if it's not needed or doesnt make sense.")]
    [SerializeField] public float buffTime = 0f;
    [Tooltip("Whether or not the item will depool after being spawned once, preventing further spawns of the item.")]
    [SerializeField] public bool depool;
    [HideInInspector] public bool dontSpawn = false;

    //Intialize the active, create any needed references in here
    public abstract void Initialize(GameObject obj);

    //This will trigger the active's effect somehow, wether that be buffing, fire a missile or otherwise
    public abstract void TriggerActive();

    //Automatically called when the active's initial cooldown time is over, nullifies the active's behaviour somehow
    public abstract void DetriggerActive();
}