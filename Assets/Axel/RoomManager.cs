using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPosition;
    [SerializeField] private Door[] doors = new Door[4];
    public int roomID = -1; //Serial number for the room. In generation order. 
    public int depth = -1; //How far from the initial room this room is.
    private int doorMask = 0;

    //From a given mask, will turn correct walls into doors.
    public void SetDoors(int mask){
        int tempMask = mask;
        for (int i = 0; i < doors.Length; i++){
            if((tempMask & 0b1) == 1)
                doors[i].SetDoor(true);
            tempMask = tempMask >> 1;
        }
    }

    public int GetDoorMask(){
        CalculateDoorMask();
        return this.doorMask;
    }

    [ContextMenu("Calculate Door Mask")]
    private void CalculateDoorMask(){
        Door[] doors = GetComponentsInChildren<Door>();

        for (int i = 0; i < doors.Length; i++){
            Vector3 doorRelativePos = (doors[i].transform.position - transform.position).normalized;
            float angle = Mathf.Round(Quaternion.FromToRotation(Vector3.forward, doorRelativePos).eulerAngles.y / 90);
            switch (angle){
                case 0:
                    this.doorMask |= 0b1;
                    break;
                case 1:
                    this.doorMask |= 0b10;
                    break;
                case 2:
                    this.doorMask |= 0b100;
                    break;
                case 3:
                    this.doorMask |= 0b1000;
                    break;
            }
        }
    }

    //Will draw a sphere at the spawn/initial room.
    void OnDrawGizmos(){
        if(this.depth == 0){
            Gizmos.DrawSphere(transform.position, 1.0f);
        }
    }
}