using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BloodController : MonoBehaviour
{
    [SerializeField] private DecalProjector decal = null;
    [SerializeField] private GameObject splatEffect = null;
    [SerializeField] private GameObject dieEffect = null;
    [SerializeField] private LayerMask lm = new LayerMask();
    [SerializeField] private Texture[] texs = null;

    private void Start ()
    {
        if(texs.Length == 0)
        {
            Debug.LogWarning("WARNING, No decal texture on bloodcontroller.cs");
        }
    }

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

    public void SpawnBlood (Vector3 hit, GameObject obj)
    {
        DecalProjector newDecal = Instantiate(decal) as DecalProjector;

        if((lm.value & (1 << obj.layer)) > 0)
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

    public void InstantiateBlood (Vector3 hit, Vector3 dirPos)
    {
        //main
        Vector3 dir = hit - dirPos;
        Quaternion rot = Quaternion.LookRotation(-dir);

        //SpawnBlood(hit);
        #region decal
        //decal
        /*
        DecalProjector newDecal = Instantiate(decal) as DecalProjector;

        Quaternion decalRot = Quaternion.LookRotation(dir);
        newDecal.transform.position = hit;
        newDecal.transform.rotation = decalRot;

        //randomize scale and rotation
        float ranScale = Random.Range(-0.5f, 3.5f);
        Vector3 scale = newDecal.size + new Vector3(ranScale, ranScale, 0);
        newDecal.size = scale;

        float ranRot = Random.Range(-180, 180);
        newDecal.transform.RotateAround(newDecal.transform.position, Vector3.up, ranRot);
        */
        #endregion

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
}
