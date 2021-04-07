using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int seed = 0;
    [SerializeField] private GameObject roomPrefab = null;
    [SerializeField] private int roomCount = 5;
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [SerializeField] private Vector2Int levelGridSize = new Vector2Int(10, 10);

    //Hide this later
    [SerializeField] private List<RoomManager> instantiatedRooms = new List<RoomManager>();
    private MazeGenerator maze;

    [ContextMenu("Generate Level")]
    private void GenerateLevel(){
        Random.InitState(seed);
        
        //Generate a maze. the MazeGenerator class will dictate where to place rooms.
        Vector2 offset = new Vector2(0, 0);
        maze = new MazeGenerator(levelGridSize, seed, roomCount);
        maze.GenerateMaze();

        //Instantiate prefab rooms for all the visited cells.
        for (int x = 0; x < maze.gridSize.x; x++){
            for (int y = 0; y < maze.gridSize.y; y++){
                if(maze.grid[x,y].visited){
                    offset = new Vector2(x * roomSize.x,y * roomSize.y);
                    GameObject newRoomObject = Instantiate(roomPrefab, transform.position + new Vector3(offset.x, 0, offset.y), transform.rotation, transform);
                    RoomManager newRoom = newRoomObject.GetComponent<RoomManager>();
                    instantiatedRooms.Add(newRoom);
                }
            }
        }
    }

    [ContextMenu("Delete Level")]
    private void DeleteLevel(){
        for (int i = 0; i < instantiatedRooms.Count; i++){
            if(instantiatedRooms[i] != null)
                DestroyImmediate(instantiatedRooms[i].gameObject);
        }
        instantiatedRooms = new List<RoomManager>();
    }

    // private void OnDrawGizmos() {
    //     for (int x = 0; x < maze.gridSize.x; x++){
    //         for (int y = 0; y < maze.gridSize.y; y++){
    //             if(maze.grid[x,y].visited){
    //                 Gizmos.DrawCube(new Vector3(x * roomSize.x, 0, y * roomSize.y), new Vector3(roomSize.x,1,roomSize.y));
    //             }
    //         }
    //     }
    // }
}

public struct MazeCell{
    // public byte doorConfig;
    public bool visited;
}

public class MazeGenerator{
    
    public MazeCell[,] grid;
    public Vector2Int gridSize;
    private int maxRoomCount;

    public MazeGenerator(Vector2Int gridSize, int seed, int maxRoomCount){
        this.grid = new MazeCell[gridSize.x, gridSize.y];
        this.gridSize = gridSize;
    }

    // Right now we're stopping when crashing and getting stuck in a corner of the grid.
    // We should probably backtrack and branch out.
    public void GenerateMaze(){
        //Initialize grid.
        for (int x = 0; x < this.gridSize.x; x++){
            for (int y = 0; y < this.gridSize.y; y++){
                this.grid[x,y].visited = false;
            }
        }

        //Start at the bottom left for now.
        Vector2Int initPosition = new Vector2Int(0, 0);
        MazeCrawl(initPosition);
    }

    private void MazeCrawl(Vector2Int position){
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(position);
        this.grid[position.x, position.y].visited = true;
        if(neighbors.Count == 0)
            return;

        int randomIndex = Random.Range(0, neighbors.Count);
        MazeCrawl(neighbors[randomIndex]);
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int position){
        List<Vector2Int> unvisited = new List<Vector2Int>();
        Vector2Int[] offsets = {new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)};

        for (int i = 0; i < offsets.Length; i++){
            Vector2Int newCoord = position + offsets[i];
            if(newCoord.x >= 0 && newCoord.x < this.gridSize.x && newCoord.y >= 0 && newCoord.y < this.gridSize.y){
                MazeCell activeCell = this.grid[newCoord.x, newCoord.y];
                if(!activeCell.visited)
                    unvisited.Add(newCoord);
            }
        }
        return unvisited;
    }
}