using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Room Generation/Level", order = 1)]
public class LevelAsset : ScriptableObject
{    
    [Header("Rooms Pool")]
    [Tooltip("What rooms this level can contain.")]
    [SerializeField] private RoomAsset[] rooms = new RoomAsset[1];
    [Tooltip("A random room from this list will be used as the initial/spawn room for this level.")]
    [SerializeField] private RoomAsset[] initialRoomPool = new RoomAsset[1];
    [Tooltip("A random room from this list will be used as the final room for this level.")]
    [SerializeField] private RoomAsset[] finalRoomPool = new RoomAsset[1];

    [Header("Generation Settings")]
    [Tooltip("How large the underlying grid for the generation should be. No room is placed outside this grid.")]
    [SerializeField] private Vector2Int desiredLevelGridSize;
    [Tooltip("How far away from the start room any room is allowed, e.g. 4 Max Depth means no room is allowed further away than 4 rooms from the start room.")]
    [SerializeField] private int maxDepth = 20;
    [Tooltip("How many iterations the random door placement should run. More iterations = more doors between branches.")]
    [SerializeField] private int randomDoorIterations = 0;

    public RoomAsset GetRandomRoom(int doorMask = -1, RoomType type = RoomType.COMMON){
        List<RoomAsset> compatibleRooms = new List<RoomAsset>();

        switch(type){
            case RoomType.COMMON:
                for (int i = 0; i < rooms.Length; i++){
                    if(RoomAsset.CompatibleDoorMask(doorMask, rooms[i].GetDoorMask()))
                        compatibleRooms.Add(rooms[i]);
                }
                break;

            case RoomType.FINAL:
                for (int i = 0; i < finalRoomPool.Length; i++){
                    if(RoomAsset.CompatibleDoorMask(doorMask, finalRoomPool[i].GetDoorMask()))
                        compatibleRooms.Add(finalRoomPool[i]);
                }
                break;
            
            case RoomType.INITIAL:
                for (int i = 0; i < initialRoomPool.Length; i++){
                    if(RoomAsset.CompatibleDoorMask(doorMask, initialRoomPool[i].GetDoorMask()))
                        compatibleRooms.Add(initialRoomPool[i]);
                }
                break;
        }

        int randomIndex = Random.Range(0, compatibleRooms.Count);
        return compatibleRooms[randomIndex];
    }

    public Vector2Int GetDesiredLevelGridSize(){
        return this.desiredLevelGridSize;
    }

    public int GetMaxDepth(){
        return this.maxDepth;
    }

    public int GetRandomDoorIterations(){
        return this.randomDoorIterations;
    }

}