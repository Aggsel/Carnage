using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public Itemstat itemstat;
    private GameObject player;
    private AttributeController attributeInstance;
    private float rotationSpeed = 50f;

    //When the mouse hovers over the GameObject, it turns to this color (red)
    Color m_MouseOverColor = Color.red;

    //This stores the GameObject’s original color
    Color m_OriginalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    MeshRenderer m_Renderer;


    void Start()
    {
        player = GameObject.Find("Player");
        attributeInstance = player.GetComponent<AttributeController>();

        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();
        //Fetch the original color of the GameObject
        m_OriginalColor = m_Renderer.material.color;
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

    void OnMouseOver()
    {
        Debug.Log("This is " + itemstat.itemName);
        m_Renderer.material.color = m_MouseOverColor;
    }

    void OnMouseExit()
    {
        m_Renderer.material.color = m_OriginalColor;
    }
}
