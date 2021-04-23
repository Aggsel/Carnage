using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itemgenerator : MonoBehaviour
{
    private Passive passive;
    private Active active;
    private CooldownController cc;
    private PassiveController pc;
    private Itemholder reference;
    private int randomIndex;

    void Start()
    {
        cc = GameObject.Find("Player/ActiveHolder").GetComponent<CooldownController>();
        pc = GameObject.Find("Player/PassiveHolder").GetComponent<PassiveController>();
        reference = GameObject.Find("Game Controller Controller/ItemHolder").GetComponent<Itemholder>();
        Generate(); //make it seeded later tbh
    }

    private void Generate()
    {
        randomIndex = Random.Range(0, reference.itemholder.actives.Length + reference.itemholder.passives.Length);
        if(randomIndex > (reference.itemholder.actives.Length - 1))
        {
            randomIndex -= (reference.itemholder.actives.Length); 
            passive = reference.itemholder.passives[randomIndex];
        }
        else
        {
            active = reference.itemholder.actives[randomIndex];
        }
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            if(active != null)
            {
                cc.Initialize(active, other.gameObject);
            }
            else
            {
                pc.Initialize(passive, other.gameObject);
            }

            Destroy(this.gameObject);

        }
    }

}
