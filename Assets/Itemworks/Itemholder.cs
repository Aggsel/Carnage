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
}
