using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapDrawer drawer = null;
    private GameObject mapReference = null;
    //private bool mapActive = false;
    private PauseController pc = null;

    void Start(){
        pc = Object.FindObjectOfType<PauseController>();
       
        if(drawer == null){
            drawer = GetComponentInChildren<MapDrawer>();
            Debug.LogWarning("Map reference not set in the Map Controller, trying to fetch during runtime instead.", this.gameObject);
        }

        if(mapReference == null)
            mapReference = drawer.gameObject;
    }

    void Update(){
        if(Input.GetKey(pc.GetKeybindings().status) && !pc.GetPaused())
        {
            //mapActive = !mapActive;
            foreach (Transform child in transform){
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
