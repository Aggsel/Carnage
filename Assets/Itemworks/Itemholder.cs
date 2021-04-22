using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itemholder : MonoBehaviour
{
    public Itemhold itemholder;

    void Start()
    {
        itemholder = Resources.Load<Itemhold>("Itemhold");
    }
}
