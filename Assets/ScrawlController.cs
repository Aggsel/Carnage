using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ScrawlController : MonoBehaviour
{
    [Header("Act I:")]
    [Tooltip("The set of possible wallscrawlls that will appear during act I")]
    [SerializeField] private Material[] collectionOne;

    [Header("Act II:")]
    [Tooltip("The set of possible wallscrawlls that will appear during act II")]
    [SerializeField] private Material[] collectionTwo;

    [Header("Act III:")]
    [Tooltip("The set of possible wallscrawlls that will appear during act III")]
    [SerializeField] private Material[] collectionThree;

    private Material decal = null;
    private DecalProjector decalProjector = null;
    public int act = 1;

    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();

        if (true /*temporary. If key exists*/)
        {
            //act = 1;
            //act = key value
            //find key value
        }
        else
        {
            act = 1;
            //create key ?
        }

        switch (act)
        {
            case 1:
                RandomizeScrawl(collectionOne);
                break;
            case 2:
                RandomizeScrawl(collectionTwo);
                break;
            case 3:
                RandomizeScrawl(collectionThree);
                break;
            default:
                Debug.Log("Act value has been incorrectly set, or not set at all! Act I scrawls will be generated.");
                RandomizeScrawl(collectionOne);
                break;
        }
    }

    private void RandomizeScrawl(Material[] collection)
    {
        if(collection.Length != 0)
        {
            int index = Random.Range(0, collection.Length);
            decal = collection[index];
            PlaceScrawl();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void PlaceScrawl()
    {
        decalProjector.material = decal;
    }
}
