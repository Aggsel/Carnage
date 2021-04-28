using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BloodController : MonoBehaviour
{
    [SerializeField] private DecalProjector decal = null;
    [SerializeField] private GameObject splatEffect = null;
    [SerializeField] private Material[] mats = null;

    private void Start ()
    {
        if(mats.Length == 0)
        {
            Debug.LogWarning("WARNING, No decal materials on bloodcontroller.cs");
        }
    }

    public void InstantiateBlood (Vector3 hit, Vector3 dirPos)
    {
        //decal
        DecalProjector newDecal = Instantiate(decal) as DecalProjector;
        newDecal.material = mats[Random.Range(0, mats.Length)];

        Vector3 dir = hit - dirPos;
        Quaternion rot = Quaternion.Euler(dir);

        newDecal.transform.SetPositionAndRotation(hit, rot);

        //particle
        GameObject par = Instantiate(splatEffect) as GameObject;
        par.transform.position = hit;
        par.transform.rotation = rot;

        Destroy(par, 5.0f);
    }
}
