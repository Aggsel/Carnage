using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeController : MonoBehaviour
{
    #region Variable struct
    [Serializable]
    public struct MeleeVariables
    {
        [Tooltip("Enable visual in unity Editor, useful for when changing variables")]
        public bool showDebug;
        [Tooltip("The range of the melee attack")]
        public float range;
        [Tooltip("The speed of which the player can attack (like fireRate of a weapon)")]
        public float rate;
        [Tooltip("After a hit ratePadding is added to pad the melee-rate (this reduces melee spam)")]
        public float ratePadding;
        [Tooltip("Amount of raycasts to detect enemies when using melee, high number means more possibilities to hit enemies")] [Range(1.0f, 10.0f)]
        public int rayAmount;
        [Tooltip("The spread of the 'rayAmount' raycasts. High number means more distance between each ray, these two go hand in hand")] [Range(0.5f, 5.0f)]
        public float raySpread;
    }
    #endregion

    [Tooltip("Dont change this please")]
    [SerializeField] private GameObject weaponModel = null;
    [SerializeField] private MeleeVariables meleeVar = new MeleeVariables();

    private bool inHit = false;
    private KeyCode meleeKey = KeyCode.F;
    private Vector3 origin = Vector3.zero;

    //read keybinds
    private void ReadKeybinds(KeyBindAsignments keys)
    {
        meleeKey = keys.melee;
    }

    private void Awake()
    {
        PauseController.updateKeysFunction += ReadKeybinds;
    }

    private void OnDestroy()
    {
        PauseController.updateKeysFunction -= ReadKeybinds;
    }

    //main
    private void Update ()
    {
        origin = Camera.main.transform.position;

        MeleeInitiator();

        //DEBUG VISUALS
        if(meleeVar.showDebug)
        {
            for (int i = -meleeVar.rayAmount; i < meleeVar.rayAmount; i++)
            {
                for (int j = -meleeVar.rayAmount; j < meleeVar.rayAmount; j++)
                {
                    Vector3 dir = (Camera.main.transform.forward + Camera.main.transform.rotation * new Vector3(j * (meleeVar.raySpread * 0.1f), i * (meleeVar.raySpread * 0.1f), 0)).normalized;
                    Debug.DrawRay(origin, dir * meleeVar.range, Color.magenta);
                }
            }
        }
    }

    private IEnumerator Melee ()
    {
        //raycast here
        float temp = 69.0f;
        Transform hitObj = null;

        for (int i = -meleeVar.rayAmount; i < meleeVar.rayAmount; i++)
        {
            for (int j = -meleeVar.rayAmount; j < meleeVar.rayAmount; j++)
            {
                Vector3 dir = (Camera.main.transform.forward + Camera.main.transform.rotation * new Vector3(j * (meleeVar.raySpread * 0.1f), i * (meleeVar.raySpread * 0.1f), 0)).normalized;

                RaycastHit hit;
                Ray ray = new Ray(origin, dir);

                if (Physics.Raycast(ray, out hit, meleeVar.range))
                {
                    float dist = Vector3.Distance(origin, hit.point);

                    if (i < -meleeVar.rayAmount + 1)
                    {
                        temp = dist;
                    }
                    
                    if(dist < temp)
                    {
                        temp = dist;
                        hitObj = hit.transform;
                        continue;
                    }
                }
                else
                {
                    Debug.Log("THIS RAY HIT NOTHING");
                }
            }
        }

        if(hitObj != null)
        {
            Debug.Log("CLOSEST: " + hitObj + ", " + temp);
        }

        yield return new WaitForSeconds(meleeVar.rate);
        weaponModel.SetActive(true);
        yield return new WaitForSeconds(meleeVar.ratePadding);
        inHit = false;
    }

    private void MeleeInitiator()
    {
        if (Input.GetKeyDown(meleeKey) && !inHit)
        {
            inHit = true;
            weaponModel.SetActive(false);

            //Instead of calling the coroutine directly
            //wait for animation and use animation keys to call the Melee IEnumerator this way we sync the hit animation to the raycast
            StartCoroutine(Melee());
        }
    }
}