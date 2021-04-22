using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : ScriptableObject
{
    [Tooltip("The name of the passive, currently not used anywhere.")]
    [SerializeField] public string passiveName = "New passive";
    public abstract void Initialize(GameObject obj);
    public abstract void TriggerPassive();
    public abstract void DeTriggerPassive();
    public abstract bool CheckValidity();

}
