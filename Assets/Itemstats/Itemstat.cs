using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Itemstat", menuName = "Itemstat")]
public class Itemstats : ScriptableObject
{

    public string itemName;
    public string itemDescription;
    public string attribute;
    public string secondAttribute;
    public float attributePercentage;
    public float secondAttributePercentage;

}
