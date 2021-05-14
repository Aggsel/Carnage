using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itemholder : MonoBehaviour
{
    public Itemhold itemholder;

    void Awake()
    {
        itemholder = Resources.Load<Itemhold>("Itemhold");  //säkert asfult och tungt vid load time men om det fungerar så BRYR JAG MIG INTE HAHAH SUG PÅ DEN CARL! jk actually bad fixa sen 
        foreach(Active i in itemholder.actives)
        {
            i.dontSpawn = false;
        }
        foreach (Passive i in itemholder.passives)
        {
            if(i == null)
            {
                Debug.LogWarning("Item index bug currently not fixed, this message will remain until it is fixed!");
            }
            else
            {
                i.dontSpawn = false;
            }
        }
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