using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnterRoom = new UnityEvent();
    [HideInInspector] [SerializeField] private RoomManager parentRoom;
    [SerializeField] private Collider doorCollider = null;
    [SerializeField] private Collider triggerZone = null;
    [SerializeField] private GameObject door = null;
    private bool isOpen = true;

    public void SetParent(RoomManager parentRoom){
        this.parentRoom = parentRoom;
    }

    private void OnTriggerEnter(Collider other){
        //Check if other is player.
        parentRoom.OnEnterRoom();
    }

    public void OpenDoor(bool open){
        if(doorCollider == null || door == null)
            return;

        if(!open & this.triggerZone != null)
            this.triggerZone.enabled = false;

        doorCollider.enabled = !open;
        door.SetActive(!open);
        isOpen = open;
    }
}
