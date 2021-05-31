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
        [Tooltip("Amount of raycasts to detect enemies when using melee, high number means more possibilities to hit enemies")] [Range(1.0f, 10.0f)]
        public int rayAmount;
        [Tooltip("The spread of the 'rayAmount' raycasts. High number means more distance between each ray, these two go hand in hand")] [Range(0.5f, 5.0f)]
        public float raySpread;
        //[Tooltip("If you melee & it misses, this penaltyTime is added to the melee recharge")]
        //public float penaltyTime;
    }
    #endregion

    [Tooltip("Dont change")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private LayerMask lm = new LayerMask();
    [Tooltip("For programmers, scripts that are disabled while using melee")]
    [SerializeField] private MonoBehaviour[] scripts = null;
    [SerializeField] private MeleeVariables meleeVar = new MeleeVariables();
    [SerializeField] private float damage = 100.0f;

    private KeyCode meleeKey = KeyCode.F;
    private Vector3 origin = Vector3.zero;
    private AudioManager am = null;
    private bool inHit = false;
    //private float penalty = 0.0f;

    private void Start ()
    {
        am = AudioManager.Instance;
    }

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

    public bool GetHit ()
    {
        return inHit;
    }

    //main
    private void Update ()
    {
        origin = Camera.main.transform.position;
        //penalty -= Time.deltaTime;
        MeleeInitiator();

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

    public void StartMelee ()
    {
        //raycast here
        float temp = 69.0f;
        Transform hitObj = null;
        RaycastHit lateHit = new RaycastHit();

        for (int i = -meleeVar.rayAmount; i < meleeVar.rayAmount; i++)
        {
            for (int j = -meleeVar.rayAmount; j < meleeVar.rayAmount; j++)
            {
                Vector3 dir = (Camera.main.transform.forward + Camera.main.transform.rotation * new Vector3(j * (meleeVar.raySpread * 0.1f), i * (meleeVar.raySpread * 0.1f), 0)).normalized;

                RaycastHit hit;
                Ray ray = new Ray(origin, dir);

                if (Physics.Raycast(ray, out hit, meleeVar.range, lm))
                {
                    float dist = Vector3.Distance(origin, hit.point);

                    if (i < -meleeVar.rayAmount + 1)
                    {
                        temp = dist;
                    }
                    
                    if(dist < temp)
                    {
                        temp = dist;
                        lateHit = hit;
                        hitObj = hit.transform;
                        continue;
                    }
                }
                else
                {
                    //Debug.Log("THIS RAY HIT NOTHING");
                    //penalty += meleeVar.penaltyTime;
                }
            }
        }

        if(hitObj != null)
        {
            StartCoroutine(GetComponent<Screenshake>().Shake(4f, 0.2f));
            am.PlaySound(am.playerMelee);

            //Debug.Log("CLOSEST: " + hitObj + ", " + temp);
            if (hitObj.GetComponentInParent<EnemyBehavior>() != null)
            {
                HitObject obj = new HitObject(transform.position, lateHit.point, damage, 0.0f, type: HitType.Melee); //set high melee damage
                hitObj.GetComponentInParent<EnemyBehavior>().OnShot(obj);
            }
        }
    }

    public void ResetMelee ()
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = true;
        }

        inHit = false;
    }

    private void MeleeInitiator()
    {
        if (Input.GetKeyDown(meleeKey) && !inHit /*&& penalty <= 0.0f*/)
        {
            inHit = true;

            for (int i = 0; i < scripts.Length; i++)
            {
                scripts[i].enabled = false;
            }

            anim.SetTrigger("melee");
        }
    }
}