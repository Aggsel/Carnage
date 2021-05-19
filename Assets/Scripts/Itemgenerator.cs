using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Itemgenerator : MonoBehaviour
{
    [SerializeField] private GameObject tex = null;
    [SerializeField] private GameObject model = null;
    private GameObject flashImageGO = null;
    private UIController uic;
    private Passive passive;
    private Active active;
    private CooldownController cc;
    private PassiveController pc;
    private Itemholder reference;
    private int randomIndex;
    private GameObject player = null;
    private RawImage flashImage;
    private bool recieved = false;

    private Vector3 startPos = Vector3.zero;

    void Start()
    {
        startPos = model.transform.position;
        player = FindObjectOfType<MovementController>().transform.gameObject;
        cc = player.GetComponentInChildren<CooldownController>();
        pc = player.GetComponentInChildren<PassiveController>();
        reference = GameObject.Find("Game Controller Controller/ItemHolder").GetComponent<Itemholder>();
        uic = GameObject.Find("Game Controller Controller/Canvas").GetComponent<UIController>();
        flashImageGO = GameObject.Find("Game Controller Controller/Canvas/FlashImage");
        recieved = false;
        correctGenerate(); //make it seeded later tbh
    }

    private void correctGenerate()
    {
        randomIndex = Random.Range(0, (reference.itemholder.actives.Length + reference.itemholder.passives.Length) - 1);
        if(randomIndex >= reference.itemholder.actives.Length)  //if index landed outside of active length, spawn a passive
        {
            randomIndex -= (reference.itemholder.actives.Length);
            if (reference.itemholder.passives[randomIndex].dontSpawn == true)
            {
                correctGenerate();
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
        else if(randomIndex < reference.itemholder.actives.Length)  //if index landed inside of active length, spawn an active
        {
            if (reference.itemholder.actives[randomIndex].dontSpawn == true)
            {
                correctGenerate();
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
        Bobbing();
    }

    private void Bobbing ()
    {
        float step = Mathf.Cos(Time.time) * 0.12f;
        model.transform.position = startPos + new Vector3(0, step, 0);
    }
    
    private void Billboard ()
    {
        tex.transform.LookAt(player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player" && recieved == false)
        {
            if(active != null)
            {
                //Debug.Log("not null active");
                uic.UIAlertText(active.activeDescription, 3.0f);
                cc.Initialize(active, other.gameObject);
            }
            else
            {
                //Debug.Log("null active");
                uic.UIAlertText(passive.passiveDescription, 3.0f);
                pc.Initialize(passive, other.gameObject);
            }
            
            
            if (flashImageGO == null)
            {
                Debug.LogWarning("Missing damage indicator reference!");
            }
            else
            {
                flashImageGO.SetActive(true);
                flashImage = flashImageGO.GetComponent<RawImage>();
                flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0.0f);
                recieved = true;
                uic.StartCoroutine(uic.FadeImage(flashImage, 1.2f, true));
                Destroy(this.gameObject);

            }
            
        }
    }
}