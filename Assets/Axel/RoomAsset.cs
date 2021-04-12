using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Room", menuName = "Room Generation/Room", order = 1)]
public class RoomAsset : ScriptableObject
{
    [SerializeField] private GameObject roomPrefab = null;
    [HideInInspector] [SerializeField] private RoomManager roomManager = null;
    [SerializeField] private int doorMask = 0;  //What walls CAN be doors.

    public GameObject GetRoom(){
        return roomPrefab;
    }

    public int GetDoorMask(){
        return this.doorMask;
    }

    void OnValidate(){
        this.roomManager = roomPrefab.GetComponent<RoomManager>();
        this.doorMask = roomManager.GetDoorMask();
    }

    //Checks whether or not a mask works with a room configuration.
    //A doormask of -1 means the mask is not taken into account, all masks are compatible.
    //This does not take into account possible rotational symmetry, might be worth implementing in the future.
    public static bool CompatibleDoorMask(int mask1, int roomConfiguration){
        return (roomConfiguration | mask1) == roomConfiguration;
    }
}