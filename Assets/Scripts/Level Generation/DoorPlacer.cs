using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorPlacer : MonoBehaviour
{
    [Tooltip("Reference to a door prefab.")]
    [SerializeField] private GameObject doorObject = null;
    private GameObject setObject = null;

    public void SetDoor(bool isDoor, RoomManager parentRoom){
        if(setObject != null)
            DestroyImmediate(setObject);

        this.setObject = Instantiate(doorObject, transform.position, transform.rotation, transform.parent);

        Door newDoor = this.setObject.GetComponent<Door>();
        parentRoom.AddDoor(newDoor);
        newDoor.SetParent(parentRoom);
        
        this.gameObject.SetActive(false);
    }
}