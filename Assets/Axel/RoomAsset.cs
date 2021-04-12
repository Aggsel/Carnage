using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Room", menuName = "Room Generation/Room", order = 1)]
public class RoomAsset : ScriptableObject
{
    [SerializeField] private GameObject roomPrefab = null;
    [SerializeField] private int doorMask;  //What walls CAN be doors.

    public GameObject GetRoom(){
        return roomPrefab;
    }

    //Checks whether or not a mask works with this room configuration.
    //A doormask of -1 means the mask is not taken into account, all masks are compatible.
    //This does not take into account possible rotational symmetry, might be worth implementing in the future.
    public bool CompatibleDoorMask(int doorMask){
        if(doorMask == -1)
            return true;

        return (this.doorMask | doorMask) == this.doorMask;
    }
}