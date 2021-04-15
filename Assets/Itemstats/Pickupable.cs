using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public Itemstats itemstat;
    public GameObject player;
    private AttributeController attributeInstance;
    private float rotationSpeed = 50f;


    void Start()
    {
        attributeInstance = player.GetComponent<AttributeController>();
    }

    void Update()
    {
        transform.Rotate(new Vector3(1f, 0f, 1f) * Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Item: " + itemstat.itemName + ", " + itemstat.itemDescription);
        
        if (itemstat.secondAttribute != "")
        {
            attributeInstance.AddBuff(itemstat.attribute, itemstat.secondAttribute, itemstat.attributePercentage, itemstat.secondAttributePercentage);
        }
        else
        {
            attributeInstance.AddBuff(itemstat.attribute, itemstat.attributePercentage);
        }

        Destroy(this.gameObject);
        
    }
}
