using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : ScriptableObject
{
    [Tooltip("The name of the passive, currently not used anywhere.")]
    [SerializeField] public string passiveName = "New passive";
    [Tooltip("Wether or not the item will depool after being spawned once, preventing more spawns of the item.")]
    [SerializeField] public bool depool;
    [HideInInspector] public bool dontSpawn = false;

    //Every passive needs to be initialized, in here any necessary references are made and variables pre-defined before use
    public abstract void Initialize(GameObject obj);

    //This will trigger the passives actual effect, wether that be to apply a buff, increase health or change the attributes of the next shot
    public abstract void TriggerPassive();

    //This will de-trigger the passive, next-shots can be restored to default and other such things for example
    public abstract void DeTriggerPassive();

    //Every passive's validity gets checked every frame. If this returns true then it means the passive's ability will be triggered this frame. 
    public abstract bool CheckValidity();

}