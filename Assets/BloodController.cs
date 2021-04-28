using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BloodController : MonoBehaviour
{
    [SerializeField] private DecalProjector decal = null;
    [SerializeField] private GameObject splatEffect = null;
    [SerializeField] private Texture[] texs = null;

    private void Start ()
    {
        if(texs.Length == 0)
        {
            Debug.LogWarning("WARNING, No decal texture on bloodcontroller.cs");
        }
    }

    public void InstantiateBlood (Vector3 hit, Vector3 dirPos)
    {
        //main
        Vector3 dir = hit - dirPos;
        Quaternion rot = Quaternion.LookRotation(-dir);

        //decal
        DecalProjector newDecal = Instantiate(decal) as DecalProjector;

        Quaternion decalRot = Quaternion.LookRotation(dir);
        newDecal.transform.position = hit;
        newDecal.transform.rotation = decalRot;

        float ranScale = Random.Range(-0.5f, 3.5f);
        Vector3 scale = newDecal.size + new Vector3(ranScale, ranScale, 0);
        newDecal.size = scale;

        //particle
        GameObject par = Instantiate(splatEffect) as GameObject;
        par.transform.position = hit;
        par.transform.rotation = rot;

        Destroy(par, 5.0f);
    }
}
