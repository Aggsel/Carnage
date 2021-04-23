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

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerPassive();
    public abstract void DeTriggerPassive();
    public abstract bool CheckValidity();

}