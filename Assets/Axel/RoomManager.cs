using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [HideInInspector] [SerializeField] public Bounds roomBounds; //Could be used as the room size. Is handled by level manager atm. 
    [HideInInspector] public Vector2Int gridPosition;
    [SerializeField] private Door[] doors;
    public int roomID = -1; //Serial number for the room. In generation order. 
    public int depth = -1; //How far from the initial room this room is.
    [SerializeField] public int doorMask = 0; 

    // [ContextMenu("Get Doors")]
    private void GetDoors(){
        this.doors = GetComponentsInChildren<Door>();

        for (int i = 0; i < doors.Length; i++){
            Vector3 doorRelativePos = (doors[i].transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(doorRelativePos.x, doorRelativePos.y);
            Debug.Log(doorRelativePos);
            Debug.Log(angle);
        }
    }

    [ContextMenu("Calculate Door Mask")]
    private void CalculateDoorMask(){
        GetDoors();
    }

    public void SetDoorMask(int mask){
        int doormask = 0b1;
        this.doorMask = mask;
        for (int i = 0; i < this.doors.Length; i++){
            if((doormask & mask) == doormask)
                this.doors[i].SetDoor(true);
            doormask = doormask << 1;
        }
    }

    void OnDrawGizmos(){
        if(this.depth == 0){
            Gizmos.DrawSphere(transform.position, 1.0f);
        }
    }
}