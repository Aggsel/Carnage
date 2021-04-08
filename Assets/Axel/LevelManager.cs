using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("Seed for the random generation.")]
    [SerializeField] private int seed = 0;
    [Tooltip("What prefabs CAN be placed during generation.")]
    [SerializeField] private LevelAsset level = null;
    [Tooltip("The maximum number of rooms allowed to be generated.")]
    [SerializeField] private int roomCount = 20;
    [Tooltip("Room size in world units.")]
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [Tooltip("How large the underlying grid for the generation should be.")]
    [SerializeField] private Vector2Int levelGridSize = new Vector2Int(10, 10);

    private RoomManager[,] grid;
    private Vector2Int currentDirection;
    [HideInInspector] [SerializeField] private List<RoomManager> instantiatedRooms = new List<RoomManager>();

    //Start at (0,0), go to a random unvisited neighbor, place room there, go to new room coordinates.
    //Repeat until roomCount has been reached OR no more unvisited neighbors exist.
    [ContextMenu("Generate Level")]
    private void GenerateLevel(){
        Random.InitState(seed);

        this.grid = new RoomManager[levelGridSize.x, levelGridSize.y];
        Vector2Int initPosition = new Vector2Int(0, 0); //Start at the bottom left for now.
        currentDirection = new Vector2Int(0,0);

        MazeCrawl(initPosition);
    }

    [ContextMenu("Delete Level")]
    private void DeleteLevel(){
        for (int i = 0; i < instantiatedRooms.Count; i++){
            if(instantiatedRooms[i] != null)
                DestroyImmediate(instantiatedRooms[i].gameObject);
        }
        instantiatedRooms = new List<RoomManager>();
    }

    private void PlaceRoom(Vector2Int position){
        Vector3 offset = new Vector3(position.x * roomSize.x, 0, position.y * roomSize.y);

        GameObject roomPrefab = level.GetRandomRoom();
        GameObject newRoomObject = Instantiate(roomPrefab, transform.position + offset, transform.rotation, transform);
        RoomManager newRoom = newRoomObject.GetComponent<RoomManager>();
        newRoom.gridPosition = new Vector2Int(position.x, position.y);
        this.grid[position.x, position.y] = newRoom;
        instantiatedRooms.Add(newRoom);
    }

    //Limited to 1x1-sized rooms for now.
    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int position){
        List<Vector2Int> unvisited = new List<Vector2Int>();
        Vector2Int[] offsets = {new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)};

        for (int i = 0; i < offsets.Length; i++){
            Vector2Int newCoord = position + offsets[i];
            if(newCoord.x >= 0 && newCoord.x < this.levelGridSize.x && newCoord.y >= 0 && newCoord.y < this.levelGridSize.y){
                RoomManager activeCell = this.grid[newCoord.x, newCoord.y];
                if(activeCell == null)
                    unvisited.Add(newCoord);
            }
        }
        return unvisited;
    }
    
    private void MazeCrawl(Vector2Int position){
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(position);

        //Place new room and set door from incoming-room.
        PlaceRoom(position);
        PlaceDoors(position, currentDirection);

        //Exit condition.
        if(neighbors.Count == 0)
            return;

        int randomIndex = Random.Range(0, neighbors.Count);
        currentDirection = (position - neighbors[randomIndex]);

        //Place door to the destination-room.
        PlaceDoors(position, currentDirection * -1);

        MazeCrawl(neighbors[randomIndex]);
    }

    //Jank-city, population: this function. TODO: burn this.
    private void PlaceDoors(Vector2Int position, Vector2Int direction){
        if(direction.x > 0)
            grid[position.x, position.y].SetDoor(1, true);
        if(direction.x < 0)
            grid[position.x, position.y].SetDoor(3, true);

        if(direction.y > 0)
            grid[position.x, position.y].SetDoor(0, true);
        if(direction.y < 0)
            grid[position.x, position.y].SetDoor(2, true);
    }
}