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

    [Header("Items")]
    [SerializeField] private GameObject itemSpawnPrefab = null;
    [Range(0.0f,1.0f)]
    [SerializeField] private float spawnChance = 0.1f;

    [Header("Enemy Spawning")]
    [Tooltip(@"How the difficulty changes depending on where in the level this room spawns. 
    First value is if the room is at the begining of the level, second if it is at the end. 
    Difficulty will linearly scale throughout the level.")]
    [SerializeField] private Vector2 difficultyRange = new Vector2();
    [Tooltip("How much the difficulty is allowed to differ from the linearly scaled value above.")]
    [SerializeField] private float randomness = 0.0f;
    [Tooltip("How many waves of enemies should spawn in this room.")]
    [SerializeField] private int numberOfEnemyWaves = 1;

    void OnValidate(){
        if(roomPrefab != null){
            this.roomManager = roomPrefab.GetComponent<RoomManager>();
            this.doorMask = roomManager.GetDoorMask();
        }
    }

    public GameObject GetRoom(){
        return roomPrefab;
    }

    public int GetDoorMask(){
        return this.doorMask;
    }

    public Vector2 GetDifficultyRange(){
        return this.difficultyRange;
    }

    public float GetRandomness(){
        return this.randomness;
    }

    public int GetEnemyWaveCount(){
        return numberOfEnemyWaves;
    }

    public GameObject GetItemSpawnPrefab(){
        if(Random.Range(0.0f, 1.0f) >= spawnChance)
            return null;

        if(itemSpawnPrefab != null)
            return itemSpawnPrefab;
            
        return null;
    }

    //Checks whether or not a mask works with a room configuration.
    //A doormask of -1 means the mask is not taken into account, all masks are compatible.
    //This does not take into account possible rotational symmetry, might be worth implementing in the future.
    public static bool CompatibleDoorMask(int mask1, int roomConfiguration){
        return (roomConfiguration | mask1) == roomConfiguration;
    }
}