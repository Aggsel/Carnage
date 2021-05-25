using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ScrawlController : MonoBehaviour
{
    [Header("Act I:")]
    [Tooltip("The set of possible wallscrawlls that will appear during act I")]
    [SerializeField] private Material[] collectionOne = null;

    [Header("Act II:")]
    [Tooltip("The set of possible wallscrawlls that will appear during act II")]
    [SerializeField] private Material[] collectionTwo = null;

    [Header("Act III:")]
    [Tooltip("The set of possible wallscrawlls that will appear during act III")]
    [SerializeField] private Material[] collectionThree = null;

    private Material decal = null;
    private DecalProjector decalProjector = null;
    private int act = 1;

    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();

        if (PlayerPrefs.HasKey("Act"))
        {
            act = PlayerPrefs.GetInt("Act");
            Debug.Log("Act is act " + act);
        }
        else
        {
            act = 1;
            PlayerPrefs.SetInt("Act", 1);
            Debug.Log("Act has been set to 1");
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
