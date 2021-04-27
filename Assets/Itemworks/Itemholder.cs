using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itemholder : MonoBehaviour
{
    public Itemhold itemholder;

    void Awake()
    {
        itemholder = Resources.Load<Itemhold>("Itemhold");
    }

    public void DepoolItemActive(int itemIndex)
    {
        itemholder.actives[itemIndex].dontSpawn = true;
    }

    public void DepoolItemPassive(int itemIndex)
    {
        itemholder.passives[itemIndex].dontSpawn = true;
    }
}