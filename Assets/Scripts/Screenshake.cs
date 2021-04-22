using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Screenshake : MonoBehaviour
{
    [SerializeField] private VolumeProfile profile = null;
    [SerializeField] float defaultAmount = 1.0f;
    [SerializeField] float defaultTime = 1.0f;

    private Vector3 startPos = Vector3.zero;
    private Vector3 desiredPos = Vector3.zero;

    private Vector3 startRot = Vector3.zero;
    private Vector3 desiredRot = Vector3.zero;

    private Transform cam = null;

    private void Start ()
    {
        startPos = Camera.main.transform.localPosition;
        cam = Camera.main.transform;
    }

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(Shake(defaultAmount, defaultTime));
        }
    }

    public IEnumerator Shake (float shakeAmount, float shakeTime)
    {
        float time = 0.0f;

        float step = Time.deltaTime * (shakeAmount * 2.0f);
        float rotStep = Time.deltaTime * (shakeAmount * 0.5f);

        //read hdrp profile if null add it
        if (!profile.TryGet<MotionBlur>(out var motion))
        {
            Debug.LogWarning("THIS SHOULD NOT HAPPEN");
            motion = profile.Add<MotionBlur>(false);
        }

        while (time < shakeTime)
        {
            motion.active = true;

            float up = Random.Range(-1.0f, 1.0f) * (shakeAmount * 0.1f);
            float right = Random.Range(-1.0f, 1.0f) * (shakeAmount * 0.1f);

            desiredPos = new Vector3(right, up, 0);

            cam.localPosition = Vector3.MoveTowards(cam.localPosition, desiredPos + startPos, step);

            #region rotation
            //rotation
            //cannot rotate as it is entierly controlled by MovementController.cs
            /*desiredRot = new Vector3(right, up, 0);
            Quaternion dest = Quaternion.Euler(startRot + desiredRot);
            
            transform.localRotation = Quaternion.Slerp(cam.localRotation, dest, rotStep);
            */
            #endregion

            time += Time.deltaTime;
            yield return null;
        }

        //cam.localPosition = startPos;
        StartCoroutine(ReturnHome(step, motion));
    }

    //smooth the return to startPos, infinite loop lol
    private IEnumerator ReturnHome (float step, MotionBlur motion)
    {
        float time = 0.0f;

        while(time < step)
        {
            time += Time.deltaTime;
            float dist = Vector3.Distance(cam.localPosition, startPos);

            if(dist > 0.001f)
            {
                cam.localPosition = Vector3.MoveTowards(cam.localPosition, startPos, step * 0.1f);
                yield return null;
            }
        }

        motion.active = false;
    }
}