using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Itemgenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject tex = null;
    [SerializeField]
    private float rotationSpeed = 0f;

    private GameObject model = null;
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
    private AudioManager am = null;
    

    void Start()
    {
        player = FindObjectOfType<MovementController>().transform.gameObject;
        cc = player.GetComponentInChildren<CooldownController>();
        pc = player.GetComponentInChildren<PassiveController>();
        reference = GameObject.Find("Game Controller Controller/ItemHolder").GetComponent<Itemholder>();
        correctGenerate(); //make it seeded later tbh
        startPos = model.transform.position;
        uic = GameObject.Find("Game Controller Controller/Canvas").GetComponent<UIController>();
        flashImageGO = GameObject.Find("Game Controller Controller/Canvas/FlashImage");
        recieved = false;
        am = AudioManager.Instance;
        am.PlaySound(am.itemSpawn, this.transform.gameObject);
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
                ActivateModel();
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
                ActivateModel();
                if (reference.itemholder.actives[randomIndex].depool == true)
                {
                    reference.DepoolItemActive(randomIndex);
                }
                tex.GetComponentInChildren<TextMeshPro>().text = active.activeName;
            }
        }
    }

    private void ActivateModel()
    {
        //if model exists, delete the old one before spawning a new model, for active re-pickup functionality
        if(model != null)
        {
            Destroy(model);
        }

        if (active != null)
        {
            model = Instantiate(active.modelPrefab, gameObject.transform);
        }
        else
        {
            model = Instantiate(passive.modelPrefab, gameObject.transform);
        }
        model.transform.parent = gameObject.transform;
        Vector3 r = new Vector3 (0f, model.transform.rotation.y, 0f);
        model.transform.rotation = Quaternion.Euler(r);
    }

    private void LateUpdate ()
    {
        Billboard();
        Bobbing();
        Rotation();
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

    private void Rotation()
    {
        transform.Rotate(0f, Time.deltaTime * rotationSpeed, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player" && recieved == false)
        {
            am.PlaySound(ref am.itemsPickup);
            if (active != null)
            {
                uic.UIAlertText(active.activeDescription, 3.0f);

                //if player has an active, replace and "remake" this active to player's currently held active
                if(cc.active != null)
                {
                    Active tmp = active;
                    active = cc.active;
                    cc.Initialize(tmp, other.gameObject);
                    flashImageGO.SetActive(true);
                    flashImage = flashImageGO.GetComponent<RawImage>();
                    flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0.0f);
                    recieved = true;
                    uic.StartCoroutine(uic.FadeImage(flashImage, 1.2f, true));
                    ActivateModel();
                    tex.GetComponentInChildren<TextMeshPro>().text = active.activeName;
                    recieved = false;
                }
                //..otherwise dont xd
                else
                {
                    cc.Initialize(active, other.gameObject);
                    flashImageGO.SetActive(true);
                    flashImage = flashImageGO.GetComponent<RawImage>();
                    flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0.0f);
                    recieved = true;
                    uic.StartCoroutine(uic.FadeImage(flashImage, 1.2f, true));
                    ActivateModel();
                    Destroy(this.gameObject);
                }
                

            }
            else
            {
                uic.UIAlertText(passive.passiveDescription, 3.0f);
                pc.Initialize(passive, other.gameObject);
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