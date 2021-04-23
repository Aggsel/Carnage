using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controllers/Itemhold")]
public class Itemhold : ScriptableObject
{
    public Active[] actives;    // All actives
    public Passive[] passives;  // All passives
}
