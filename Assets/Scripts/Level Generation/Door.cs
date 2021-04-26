using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnterRoom = new UnityEvent();
    [HideInInspector] [SerializeField] private RoomManager parentRoom;
    [SerializeField] private Collider doorCollider = null;
    [SerializeField] private GameObject door = null;
    [SerializeField] private Collider trigger = null;
    private bool isOpen = true;
    private bool sentinel = false;
    
    public void SetParent(RoomManager parentRoom){
        this.parentRoom = parentRoom;
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.layer == 12 && !sentinel){ 
             sentinel = true;
            parentRoom.OnEnterRoom();
        }
        
    }

    void OnTriggerExit(Collider other){
        sentinel = sentinel ? false : sentinel;
    }

    public void OpenDoor(bool open){
        if(doorCollider == null || door == null)
            return;

        if(trigger != null)
            trigger.enabled = open;

        doorCollider.enabled = !open;
        door.SetActive(!open);
        isOpen = open;
    }
}
