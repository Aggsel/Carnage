using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapDrawer drawer = null;
    private GameObject mapReference = null;
    private bool mapActive = false;
    private float animationDuration = 1.0f;

    void Start(){
        if(drawer == null){
            drawer = GetComponentInChildren<MapDrawer>();
            Debug.LogWarning("Map reference not set in the Map Controller, trying to fetch during runtime instead.", this.gameObject);
        }

        if(mapReference == null)
            mapReference = drawer.gameObject;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.M)){
            mapActive = !mapActive;
            mapReference.SetActive(mapActive);
        }
    }
}
