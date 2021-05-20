using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable]
public struct RecoilVariables
{
    [Tooltip("The amount of which the recoil is added when shooting")]
    public float recoilAmount;
    [Tooltip("For each shot how much recoil is added to the gun, works togeather with recoilAmount, should be very low like 0.1")]
    public float reocilIncrease;
    [Tooltip("How fast the recoil is reduced back to default state")]
    public float recoilSpeed;
    [Tooltip("The destination for the rotational changes to the weapon")]
    public Vector3 recoilRotation;
    [Tooltip("The destination for the positional changes to the weapon")]
    public Vector3 recoilPosition;
    [Tooltip("Additional noise to the recoilRotation and recoilPosition, adds depth, should also be very low (1.0 - 2.0)")]
    public Vector3 recoilNoise;
}

[Serializable]
public struct ShakeVariables
{
    public float shakeAmount;
    public float shakeTime;
    public float shakeRotationTangent;
}

public class Screenshake : MonoBehaviour
{
    [Header("Assignable: ")]
    [SerializeField] private VolumeProfile profile = null;
    [SerializeField] Transform shakeOrigin = null;
    [SerializeField] Transform mainOrigin = null;

    [SerializeField] ShakeVariables shakeVar = new ShakeVariables();
    [SerializeField] RecoilVariables recoilVar = new RecoilVariables();

    private Vector3 startPos = Vector3.zero;
    private Vector3 desiredPos = Vector3.zero;

    private Vector3 mainStartPos = Vector3.zero;

    private Vector3 startRot = Vector3.zero;
    private Vector3 desiredRot = Vector3.zero;

    private float recoil = 0.0f;
    private Vector3 noise = Vector3.zero;

    private void Start ()
    {
        startPos = shakeOrigin.localPosition;
        mainStartPos = mainOrigin.localPosition;

        startRot = shakeOrigin.localEulerAngles;
    }

    public void SetRecoilIncrease(float increment)
    {
        recoilVar.reocilIncrease = increment;
    }

    private void Update ()
    {
        /*if(Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Shake(shakeVar.shakeAmount, shakeVar.shakeTime));
        }*/

        Recoil();
    }

    public void RecoilCall ()
    {
        noise = new Vector3(UnityEngine.Random.Range(-recoilVar.recoilNoise.x, recoilVar.recoilNoise.x),
                UnityEngine.Random.Range(-recoilVar.recoilNoise.y, recoilVar.recoilNoise.y),
                UnityEngine.Random.Range(-recoilVar.recoilNoise.z, recoilVar.recoilNoise.z)) * 0.5f;

        //on firerate change also change recoilIncrease to keep it stable
        recoil += recoilVar.reocilIncrease;
    }

    private void Recoil ()
    {
        if(recoil > 0.01f)
        {
            Quaternion newNoise = Quaternion.Euler(recoilVar.recoilRotation + noise);
            Quaternion mainNoise = Quaternion.Euler((-recoilVar.recoilRotation * 0.75f) + noise);
            shakeOrigin.transform.localRotation = Quaternion.Slerp(shakeOrigin.transform.localRotation, newNoise, Time.deltaTime * recoilVar.recoilSpeed * 0.25f);
            mainOrigin.transform.localRotation = Quaternion.Slerp(mainOrigin.transform.localRotation, mainNoise, Time.deltaTime * recoilVar.recoilSpeed * 0.5f);

            //ja jo men lite collision kan förekomma här med screenshake
            desiredPos = (recoilVar.recoilPosition + noise) * recoilVar.recoilAmount;
            shakeOrigin.localPosition = Vector3.MoveTowards(shakeOrigin.localPosition, desiredPos + startPos, Time.deltaTime * recoilVar.recoilSpeed);

            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0.0f;
            shakeOrigin.transform.localRotation = Quaternion.Slerp(shakeOrigin.transform.localRotation, Quaternion.Euler(startRot), Time.deltaTime * recoilVar.recoilSpeed * 10.0f);
            shakeOrigin.localPosition = Vector3.MoveTowards(shakeOrigin.localPosition, startPos, Time.deltaTime * recoilVar.recoilSpeed * 5.0f);
            mainOrigin.transform.localRotation = Quaternion.Slerp(mainOrigin.transform.localRotation, Quaternion.Euler(startRot), Time.deltaTime * recoilVar.recoilSpeed * 10.0f);
        }
    }
    
    public IEnumerator Shake (float shakeAmount, float shakeTime)
    {
        float time = 0.0f;

        float step = Time.deltaTime * (shakeAmount * 2.0f);
        float rotStep = Time.deltaTime * (shakeAmount * shakeVar.shakeRotationTangent);

        //read hdrp profile if null add it
        if (!profile.TryGet<MotionBlur>(out var motion))
        {
            Debug.LogWarning("THIS SHOULD NOT HAPPEN");
            motion = profile.Add<MotionBlur>(false);
        }

        while (time < shakeTime)
        {
            motion.active = true;

            float up = UnityEngine.Random.Range(-1.0f, 1.0f) * (shakeAmount * 0.1f);
            float right = UnityEngine.Random.Range(-1.0f, 1.0f) * (shakeAmount * 0.1f);

            desiredPos = new Vector3(right, up, 0);

            shakeOrigin.localPosition = Vector3.MoveTowards(shakeOrigin.localPosition, desiredPos * 0.2f + startPos, step);
            mainOrigin.localPosition = Vector3.MoveTowards(mainOrigin.localPosition, desiredPos + startPos, step);

            //rotation only works with weapon, cuz movementcontroller controls all main camera rotation
            desiredRot = new Vector3(right, up, 0) * shakeVar.shakeRotationTangent;
            Quaternion dest = Quaternion.Euler(startRot + desiredRot);

            shakeOrigin.localRotation = Quaternion.Slerp(shakeOrigin.localRotation, dest, rotStep);

            time += Time.deltaTime;
            yield return null;
        }

        shakeOrigin.localEulerAngles = startRot;
        StartCoroutine(ReturnHome(step, motion));
    }

    //smooth the return to startPos, infinite loop lol
    private IEnumerator ReturnHome (float step, MotionBlur motion)
    {
        float time = 0.0f;

        while(time < step)
        {
            time += Time.deltaTime;
            float dist = Vector3.Distance(shakeOrigin.localPosition, startPos);

            if(dist > 0.001f)
            {
                shakeOrigin.localPosition = Vector3.MoveTowards(shakeOrigin.localPosition, startPos, step * 0.1f);
                yield return null;
            }
        }

        shakeOrigin.localPosition = startPos;
        mainOrigin.localPosition = mainStartPos;
        motion.active = false;
    }
}