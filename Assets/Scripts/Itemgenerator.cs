using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Itemgenerator : MonoBehaviour
{
    [SerializeField] private GameObject tex = null;

    private Passive passive;
    private Active active;
    private CooldownController cc;
    private PassiveController pc;
    private Itemholder reference;
    private int randomIndex;

    private GameObject player = null;

    void Start()
    {
        player = FindObjectOfType<MovementController>().transform.gameObject;
        cc = GameObject.Find("Player/ActiveHolder").GetComponent<CooldownController>();
        pc = GameObject.Find("Player/PassiveHolder").GetComponent<PassiveController>();
        reference = GameObject.Find("Game Controller Controller/ItemHolder").GetComponent<Itemholder>();
        Generate(); //make it seeded later tbh
    }

    private void Generate()
    {
        randomIndex = Random.Range(0, (reference.itemholder.actives.Length + reference.itemholder.passives.Length));
        if (randomIndex > (reference.itemholder.actives.Length - 1))
        {
            randomIndex -= (reference.itemholder.actives.Length); 
            if(reference.itemholder.passives[randomIndex].dontSpawn == true)
            {
                Generate();
            } 
            else
            {
                passive = reference.itemholder.passives[randomIndex];
                if (reference.itemholder.passives[randomIndex].depool == true)
                {
                    reference.DepoolItemPassive(randomIndex);
                }

                tex.GetComponentInChildren<TextMeshPro>().text = passive.passiveName;
            }
        }
        else
        {
            if(reference.itemholder.actives[randomIndex].dontSpawn == true)
            {
                Generate();
            }
            else
            {
                active = reference.itemholder.actives[randomIndex];
                if (reference.itemholder.actives[randomIndex].depool == true)
                {
                    reference.DepoolItemActive(randomIndex);
                }

                tex.GetComponentInChildren<TextMeshPro>().text = active.activeName;
            }
        }
    }

    private void LateUpdate ()
    {
        Billboard();
    }
    
    private void Billboard ()
    {
        tex.transform.LookAt(player.transform.position);
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