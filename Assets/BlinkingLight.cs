using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BlinkingLight : MonoBehaviour
{
    private HDAdditionalLightData lightData = null;
    private Light l = null;
    private float strength = 0.0f;
    private Queue<float> queue = null;
    private int startQueue = 5;
    private float lastValue = 0.0f;

    private void Start ()
    {
        lightData = GetComponent<HDAdditionalLightData>();
        l = GetComponent<Light>();
        strength = lightData.intensity;
        queue = new Queue<float>(startQueue);

        StartCoroutine(Blink());
    }

    private void Update ()
    {
        if(lightData == null)
        {
            return;
        }

        while(queue.Count >= startQueue)
        {
            lastValue -= queue.Dequeue();
        }

        float value = strength * Random.Range(0.5f, 1.0f);
        queue.Enqueue(value);
        lastValue += value;

        lightData.intensity = lastValue / (float)queue.Count;
    }

    IEnumerator Blink ()
    {
        float randomTime = Random.Range(1.5f, 6.0f);

        yield return new WaitForSeconds(randomTime);
        l.enabled = false;
        float randomTimee = Random.Range(0.25f, 0.6f);
        yield return new WaitForSeconds(randomTimee);
        l.enabled = true;

        StartCoroutine(Blink());
    }
}
