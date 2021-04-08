using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int seed = 0;
    [SerializeField] private GameObject roomPrefab = null;
    [SerializeField] private int roomCount = 20;
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [SerializeField] private Vector2Int levelGridSize = new Vector2Int(10, 10);
    public RoomManager[,] grid;

    //Hide this later
    [SerializeField] private List<RoomManager> instantiatedRooms = new List<RoomManager>();

    [ContextMenu("Generate Level")]
    private void GenerateLevel(){
        Random.InitState(seed);

        this.grid = new RoomManager[levelGridSize.x, levelGridSize.y];
        Vector2Int initPosition = new Vector2Int(0, 0); //Start at the bottom left for now.
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

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int newCoord = position + offsets[i];
            if(newCoord.x >= 0 && newCoord.x < this.levelGridSize.x && newCoord.y >= 0 && newCoord.y < this.levelGridSize.y){
                RoomManager activeCell = this.grid[newCoord.x, newCoord.y];
                Debug.Log(activeCell);
                if(activeCell == null)
                    unvisited.Add(newCoord);
            }
        }
        return unvisited;
    }

    private void MazeCrawl(Vector2Int position){
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(new Vector2Int(position.x, position.y));
        PlaceRoom(position);
        if(neighbors.Count == 0 || instantiatedRooms.Count >= roomCount)
            return;

        int randomIndex = Random.Range(0, neighbors.Count);
        MazeCrawl(neighbors[randomIndex]);
    }
}