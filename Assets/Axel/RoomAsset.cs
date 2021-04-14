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

    [Header("Difficulty Scaling")]
    [Tooltip(@"How the difficulty changes depending on where in the level this room spawns. 
    First value is if the room is at the begining of the level, second if it is at the end. 
    Difficulty will linearly scale throughout the level.")]
    [SerializeField] public Vector2 difficultyRange = new Vector2();
    [Tooltip("How much the difficulty is allowed to differ from the linearly scaled value above.")]
    [SerializeField] public float randomness = 0.0f;

    public GameObject GetRoom(){
        return roomPrefab;
    }

    public int GetDoorMask(){
        return this.doorMask;
    }

    void OnValidate(){
        if(roomPrefab != null){
            this.roomManager = roomPrefab.GetComponent<RoomManager>();
            this.doorMask = roomManager.GetDoorMask();
        }
    }

    //Checks whether or not a mask works with a room configuration.
    //A doormask of -1 means the mask is not taken into account, all masks are compatible.
    //This does not take into account possible rotational symmetry, might be worth implementing in the future.
    public static bool CompatibleDoorMask(int mask1, int roomConfiguration){
        return (roomConfiguration | mask1) == roomConfiguration;
    }
}