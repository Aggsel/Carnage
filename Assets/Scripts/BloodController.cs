using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BloodController : MonoBehaviour
{
    [Header("Blood options: ")]
    [SerializeField] private bool poolBlood = false;
    [SerializeField] private int poolSize = 0;

    //[SerializeField] private int rayAmount = 0;
    //[SerializeField] private int raySpread = 0;

    [Header("Assignables: ")]
    [SerializeField] private Texture[] bloodTextures = null;
    [SerializeField] private DecalProjector decal = null;
    [SerializeField] private GameObject splatEffect = null;
    [SerializeField] private GameObject dieEffect = null;
    [SerializeField] private LayerMask lm = new LayerMask();

    private Queue<DecalProjector> decalPool = new Queue<DecalProjector>();
    private Vector3 decalDefaultScale = Vector3.zero;
    private GameObject player = null;

    private void Start ()
    {
        if (player == null)
            player = FindObjectOfType<MovementController>().gameObject;

        if(bloodTextures.Length == 0)
        {
            Debug.LogWarning("WARNING, No decal texture on bloodcontroller.cs");
        }
    }

    /*public int GetBloodSpawnProcentage ()
    {
        return bloodSpawnProcentage;
    }*/

    #region bloodSpawning
    //when an enemy dies
    public void InstantiateDeathBlood (Vector3 hit)
    {
        //particle
        GameObject par = Instantiate(dieEffect) as GameObject;
        par.GetComponentInChildren<BloodParticle>().SetBloodController(this);
        par.transform.SetPositionAndRotation(hit, Quaternion.identity);

        //particle randomizer
        float parRan = Random.Range(0.9f, 2.0f);
        par.transform.localScale = par.transform.localScale * parRan;

        Destroy(par, 2.0f);
    }

    //spawn blood decal but optimized!
    public void SpawnBloodOptimized (float bloodChance, float bloodScale, float bloodSpread, GameObject enemyOrigin)
    {
        for (int i = 0; i < 5; i++)
        {
            //Vector3 dir = (-enemyOrigin.transform.up + enemyOrigin.transform.rotation * new Vector3(j * (raySpread * 0.1f), 0, i * (raySpread * 0.1f))).normalized;
            Vector3 dir = Vector3.zero;

            switch (i)
            {
                case 0:
                    dir = -enemyOrigin.transform.up;
                    break;
                case 1:
                    dir = enemyOrigin.transform.forward;
                    break;
                case 2:
                    dir = -enemyOrigin.transform.forward;
                    break;
                case 4:
                    dir = enemyOrigin.transform.right;
                    break;
                case 5:
                    dir = -enemyOrigin.transform.right;
                    break;
                default:
                    break;
            }

            RaycastHit hit;
            Ray ray = new Ray(enemyOrigin.transform.position, dir);

            if (Physics.Raycast(ray, out hit, 3f, lm))
            {
                if (Random.value > bloodChance)
                {
                    if (poolBlood)
                    {
                        if (decalPool.Count > poolSize)
                        {
                            DecalProjector selectedDecal = decalPool.Dequeue();

                            //Quaternion decalRot = Quaternion.LookRotation(hit.normal);
                            Quaternion decalRot = Quaternion.FromToRotation(enemyOrigin.transform.up, hit.normal);
                            selectedDecal.material.mainTexture = bloodTextures[Random.Range(0, bloodTextures.Length)];
                            selectedDecal.transform.position = hit.point;
                            selectedDecal.transform.rotation = decalRot;
                            selectedDecal.transform.SetParent(hit.transform, true);

                            decalPool.Enqueue(selectedDecal);
                        }
                        else
                        {
                            DecalProjector newDecal = Instantiate(decal) as DecalProjector;
                            decalPool.Enqueue(newDecal);
                            decalDefaultScale = newDecal.size;

                            //Quaternion decalRot = Quaternion.LookRotation(-hit.normal);
                            Quaternion decalRot = Quaternion.FromToRotation(enemyOrigin.transform.up, hit.normal);
                            newDecal.material.mainTexture = bloodTextures[Random.Range(0, bloodTextures.Length)];
                            newDecal.transform.position = hit.point;
                            newDecal.transform.rotation = decalRot;

                            newDecal.transform.SetParent(hit.transform, true);

                            //randomize scale and rotation
                            float ranScale = Random.Range(-bloodScale, bloodScale);
                            Vector3 scale = decalDefaultScale + new Vector3(ranScale, ranScale, newDecal.size.z);
                            newDecal.size = scale;

                            float ranRot = Random.Range(-180, 180);
                            newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
                        }
                    }
                    else
                    {
                        DecalProjector newDecal = Instantiate(decal) as DecalProjector;
                        decalPool.Enqueue(newDecal);
                        decalDefaultScale = newDecal.size;

                        //Quaternion decalRot = Quaternion.LookRotation(-hit.normal);
                        Quaternion decalRot = Quaternion.FromToRotation(enemyOrigin.transform.up, hit.normal);
                        newDecal.material.mainTexture = bloodTextures[Random.Range(0, bloodTextures.Length)];
                        newDecal.transform.position = hit.point;
                        newDecal.transform.rotation = decalRot;

                        newDecal.transform.SetParent(hit.transform, true);

                        //randomize scale and rotation
                        float ranScale = Random.Range(-bloodScale, bloodScale);
                        Vector3 scale = decalDefaultScale + new Vector3(ranScale, ranScale, newDecal.size.z);
                        newDecal.size = scale;

                        float ranRot = Random.Range(-180, 180);
                        newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
                    }
                }
            }
            else
            {
                //Debug.Log("THIS RAY HIT NOTHING");
            }
        }

        /*for (int i = 0; i < bloodAmount; i++)
        {
            RaycastHit hit;
            Ray forwardRay = new Ray(enemyOrigin.transform.position, -enemyOrigin.transform.up);

            if (Physics.Raycast(forwardRay, out hit, 100f, lm)) //lm is a layermask and 0.5f is the ray lenght
            {
                if (poolBlood)
                {
                    if (decalPool.Count > poolSize)
                    {
                        DecalProjector selectedDecal = decalPool.Dequeue();

                        Quaternion decalRot = Quaternion.LookRotation(Vector3.down);
                        selectedDecal.material.mainTexture = bloodTextures[Random.Range(0, bloodTextures.Length)];
                        selectedDecal.transform.position = hit.point;
                        selectedDecal.transform.rotation = decalRot;
                        selectedDecal.transform.SetParent(hit.transform, true);

                        decalPool.Enqueue(selectedDecal);
                    }
                    else
                    {
                        DecalProjector newDecal = Instantiate(decal) as DecalProjector;
                        decalPool.Enqueue(newDecal);
                        decalDefaultScale = newDecal.size;

                        Quaternion decalRot = Quaternion.LookRotation(Vector3.down);
                        newDecal.material.mainTexture = bloodTextures[Random.Range(0, bloodTextures.Length)];
                        newDecal.transform.position = hit.point;
                        newDecal.transform.rotation = decalRot;

                        newDecal.transform.SetParent(hit.transform, true);

                        //randomize scale and rotation
                        float ranScale = Random.Range(-bloodScale, bloodScale);
                        Vector3 scale = decalDefaultScale + new Vector3(ranScale, ranScale, newDecal.size.z);
                        newDecal.size = scale;

                        float ranRot = Random.Range(-180, 180);
                        newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
                    }
                }
                else
                {
                    DecalProjector newDecal = Instantiate(decal) as DecalProjector;
                    decalPool.Enqueue(newDecal);
                    decalDefaultScale = newDecal.size;

                    Quaternion decalRot = Quaternion.LookRotation(Vector3.down);
                    newDecal.material.mainTexture = bloodTextures[Random.Range(0, bloodTextures.Length)];
                    newDecal.transform.position = hit.point;
                    newDecal.transform.rotation = decalRot;

                    newDecal.transform.SetParent(hit.transform, true);

                    //randomize scale and rotation
                    float ranScale = Random.Range(-bloodScale, bloodScale);
                    Vector3 scale = decalDefaultScale + new Vector3(ranScale, ranScale, newDecal.size.z);
                    newDecal.size = scale;

                    float ranRot = Random.Range(-180, 180);
                    newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
                }
            }
        }*/
    }

    //Removed for performance
    //spawn a blood decal, is called from particle system in BloodParticle.cs
    /*public void SpawnBlood (Vector3 hit, GameObject obj)
    {
        if(poolBlood)
        {
            if(decalPool.Count > poolSize)
            {
                DecalProjector selectedDecal = decalPool.Dequeue();

                //default spawn code
                if ((lm.value & (1 << obj.layer)) > 0)
                {
                    selectedDecal.transform.SetParent(obj.transform, true);
                }

                Quaternion decalRot = Quaternion.LookRotation(Vector3.down);
                selectedDecal.transform.position = hit;
                selectedDecal.transform.rotation = decalRot;

                decalPool.Enqueue(selectedDecal);
            }
            else
            {
                DecalProjector newDecal = Instantiate(decal) as DecalProjector;
                decalPool.Enqueue(newDecal);
                decalDefaultScale = newDecal.size;

                if ((lm.value & (1 << obj.layer)) > 0)
                {
                    newDecal.transform.SetParent(obj.transform, true);
                }

                Quaternion decalRot = Quaternion.LookRotation(Vector3.down);
                newDecal.transform.position = hit;
                newDecal.transform.rotation = decalRot;

                //randomize scale and rotation
                float ranScale = Random.Range(-0.5f, 3.5f);
                Vector3 scale = decalDefaultScale + new Vector3(ranScale, ranScale, newDecal.size.z);
                newDecal.size = scale;

                float ranRot = Random.Range(-180, 180);
                newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
            }
        }
        else
        {
            DecalProjector newDecal = Instantiate(decal) as DecalProjector;
            decalPool.Enqueue(newDecal);
            decalDefaultScale = newDecal.size;

            if ((lm.value & (1 << obj.layer)) > 0)
            {
                newDecal.transform.SetParent(obj.transform, true);
            }

            Quaternion decalRot = Quaternion.LookRotation(Vector3.down);
            newDecal.transform.position = hit;
            newDecal.transform.rotation = decalRot;

            //randomize scale and rotation
            float ranScale = Random.Range(-0.5f, 3.5f);
            Vector3 scale = decalDefaultScale + new Vector3(ranScale, ranScale, newDecal.size.z);
            newDecal.size = scale;

            float ranRot = Random.Range(-180, 180);
            newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
        }
    }*/

    //smaller hit effect of blood comming from the enemy
    public void InstantiateBlood (Vector3 hit, Vector3 dirPos)
    {
        //main
        Vector3 dir = hit - dirPos;
        Vector3 ranRot = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0);
        Quaternion rot = Quaternion.LookRotation(-dir + ranRot);

        //particle
        GameObject par = Instantiate(splatEffect) as GameObject;
        par.GetComponentInChildren<BloodParticle>().SetBloodController(this);
        par.transform.position = hit;
        par.transform.rotation = rot;

        //particle randomizer
        float parRan = Random.Range(0.9f, 2.0f);
        par.transform.localScale = par.transform.localScale * parRan;

        Destroy(par, 5.0f);
    }
    #endregion
}
