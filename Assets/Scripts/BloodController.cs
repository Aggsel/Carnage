using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BloodController : MonoBehaviour
{
    [Header("Blood options: ")]
    [SerializeField] private bool poolBlood = false;
    [SerializeField] private int poolSize = 0;
    [Range(0, 100)]
    [SerializeField] private int bloodSpawnProcentage = 100;

    [Header("Assignables: ")]
    [SerializeField] private DecalProjector decal = null;
    [SerializeField] private GameObject splatEffect = null;
    [SerializeField] private GameObject dieEffect = null;
    [SerializeField] private LayerMask lm = new LayerMask();
    [SerializeField] private Texture[] texs = null;

    private Queue<DecalProjector> decalPool = new Queue<DecalProjector>();

    private void Start ()
    {
        if(texs.Length == 0)
        {
            Debug.LogWarning("WARNING, No decal texture on bloodcontroller.cs");
        }
    }

    public int GetBloodSpawnProcentage ()
    {
        return bloodSpawnProcentage;
    }

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

    //spawn a blood decal, is called from particle system in BloodParticle.cs
    public void SpawnBlood (Vector3 hit, GameObject obj)
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

                Quaternion decalRot = Quaternion.LookRotation(Vector3.down/*dir*/);
                selectedDecal.transform.position = hit;
                selectedDecal.transform.rotation = decalRot;

                //randomize scale and rotation
                float ranScale = Random.Range(-0.5f, 3.5f);
                Vector3 scale = selectedDecal.size + new Vector3(ranScale, ranScale, 0);
                selectedDecal.size = scale;

                float ranRot = Random.Range(-180, 180);
                selectedDecal.transform.RotateAround(selectedDecal.transform.position, Vector3.up, ranRot);
                decalPool.Enqueue(selectedDecal);
            }
            else
            {
                DecalProjector newDecal = Instantiate(decal) as DecalProjector;
                decalPool.Enqueue(newDecal);

                if ((lm.value & (1 << obj.layer)) > 0)
                {
                    newDecal.transform.SetParent(obj.transform, true);
                }

                Quaternion decalRot = Quaternion.LookRotation(Vector3.down/*dir*/);
                newDecal.transform.position = hit;
                newDecal.transform.rotation = decalRot;

                //randomize scale and rotation
                float ranScale = Random.Range(-0.5f, 3.5f);
                Vector3 scale = newDecal.size + new Vector3(ranScale, ranScale, 0);
                newDecal.size = scale;

                float ranRot = Random.Range(-180, 180);
                newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
            }
        }
        else
        {
            DecalProjector newDecal = Instantiate(decal) as DecalProjector;
            decalPool.Enqueue(newDecal);

            if ((lm.value & (1 << obj.layer)) > 0)
            {
                newDecal.transform.SetParent(obj.transform, true);
            }

            Quaternion decalRot = Quaternion.LookRotation(Vector3.down/*dir*/);
            newDecal.transform.position = hit;
            newDecal.transform.rotation = decalRot;

            //randomize scale and rotation
            float ranScale = Random.Range(-0.5f, 3.5f);
            Vector3 scale = newDecal.size + new Vector3(ranScale, ranScale, 0);
            newDecal.size = scale;

            float ranRot = Random.Range(-180, 180);
            newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
        }
    }

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
