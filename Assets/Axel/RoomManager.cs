using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPosition;
    [SerializeField] private Door[] doors;
    public int roomID = -1; //Serial number for the room. In generation order. 
    public int depth = -1; //How far from the initial room this room is.

    //From a given mask, will turn correct walls into doors.
    public void SetDoors(int mask){
        int tempMask = mask;
        for (int i = 0; i < doors.Length; i++){
            if((tempMask & 0b1) == 1)
                doors[i].SetDoor(true);
            tempMask = tempMask >> 1;
        }
    }

    //Will draw a sphere at the spawn/initial room.
    void OnDrawGizmos(){
        if(this.depth == 0){
            Gizmos.DrawSphere(transform.position, 1.0f);
        }
    }
}