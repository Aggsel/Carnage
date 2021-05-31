using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BloodController : MonoBehaviour
{
    [Header("Blood options: ")]
    [SerializeField] private bool poolBlood = false;
    [SerializeField] private int poolSize = 0;

    [Header("Assignables: ")]
    [SerializeField] private Texture[] bloodTextures = null;
    [SerializeField] private DecalProjector decal = null;
    [SerializeField] private GameObject splatEffect = null;
    [SerializeField] private GameObject dieEffect = null;
    [SerializeField] private LayerMask lm = new LayerMask();
    [SerializeField] private Texture[] texs = null;

    private Queue<DecalProjector> decalPool = new Queue<DecalProjector>();
    private Vector3 decalDefaultScale = Vector3.zero;

    private void Start ()
    {
        if(texs.Length == 0)
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
    public void SpawnBloodOptimized (int bloodAmount, float bloodScale, float bloodSpread, GameObject enemyOrigin)
    {
        for (int i = 0; i < bloodAmount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-bloodSpread, bloodSpread), 0, Random.Range(-bloodSpread, bloodSpread));

            RaycastHit hit;
            Ray forwardRay = new Ray(enemyOrigin.transform.position + offset, -enemyOrigin.transform.up);
            Debug.DrawRay(enemyOrigin.transform.position + offset, -enemyOrigin.transform.up, Color.magenta);

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
        }
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
