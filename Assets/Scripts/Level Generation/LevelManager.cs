using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    //User accessible variables
    [Header("Generation Attributes")]
    [Tooltip("Seed for the random generation.")]
    [SerializeField] private int seed = 0;
    [Tooltip("Will ignore the seed above if active.")]
    [SerializeField] private bool randomizeSeed = false;
    [Tooltip("What prefabs CAN be placed during generation.")]
    [SerializeField] private LevelAsset level = null;
    [Tooltip("How far away from the start room any room is allowed, e.g. 4 Max Depth means no room is allowed further away than 4 rooms from the start room.")]
    [SerializeField] private int maxDepth = 5;
    [Tooltip("How many iterations the random door placement should run. More iterations = more doors between branches.")]
    [SerializeField] private int randomDoorIterations = 10;

    [Header("Grid Settings")]
    [Tooltip("Room size in world units.")]
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [Tooltip("How large the underlying grid for the generation should be. No room is placed outside this grid.")]
    [SerializeField] private Vector2Int desiredLevelGridSize = new Vector2Int(10, 10);
    [Tooltip("Where in the grid the initial room should spawn.")]
    [SerializeField] private Vector2Int spawnRoomLocation = new Vector2Int(0, 0);

    [Header("Callback")]
    [Tooltip("Will invoke all of these functions when level generation is complete.")]
    [SerializeField] private UnityEvent OnFinishedGeneration = null;

    //Debug
    [Header("Debug Variables")]
    [SerializeField] private float itterationOffset = 1.0f;

    //Hidden variables
    [HideInInspector] [SerializeField] private List<RoomManager> instantiatedRooms = new List<RoomManager>();
    [HideInInspector] [SerializeField] private MazeGenerator maze;
    private int roomCounter = 0;
    private RoomManager[,] grid;

    void Start(){
        GenerateLevel();
    }

    [ContextMenu("Generate Level")]
    private void GenerateLevel(){
        //If we already have generated a level, make sure we remove the first one first.
        if(this.maze != null)
            DeleteLevel();

        roomCounter = 0;
        
        int seed = randomizeSeed ? System.Environment.TickCount : this.seed;
        this.maze = new MazeGenerator(desiredLevelGridSize, seed, maxDepth, spawnRoomLocation, randomDoorIterations);
        this.grid = new RoomManager[desiredLevelGridSize.x, desiredLevelGridSize.y];

        PopulateLevel();
        OnFinishedGeneration.Invoke();
    }

    [ContextMenu("Delete Level")]
    private void DeleteLevel(){
        for (int i = 0; i < instantiatedRooms.Count; i++){
            if(instantiatedRooms[i] != null)
                DestroyImmediate(instantiatedRooms[i].gameObject);
        }
        instantiatedRooms = new List<RoomManager>();

        this.maze = null;
    }

    //Based on the maze, place rooms.
    private void PopulateLevel(){
        if(this.maze == null)
            return;

        //Should this really access the maze gridsize like this?
        for (int y = 0; y < this.maze.actualGridSize.y; y++){
            for (int x = 0; x < this.maze.actualGridSize.x; x++){
                if(this.maze.grid[x,y].visited){
                    PlaceRoom(new Vector2Int(x,y), this.maze.grid[x,y].depth, this.maze.grid[x,y].doorMask);
                }  
            } 
        }
    }

    //Place room at a given position. doorMask dictates where the doors in the room should be.
    private void PlaceRoom(Vector2Int pos, int depth, int doorMask){
        float normalizedDepth = depth / (float)this.maze.maxDepthReached;
        
        Vector3 offset = new Vector3(pos.x * roomSize.x, this.maze.grid[pos.x, pos.y].roomCounter * itterationOffset, pos.y * roomSize.y);
        RoomAsset roomAsset = level.GetRandomRoom(this.maze.grid[pos.x, pos.y].doorMask, this.maze.grid[pos.x, pos.y].type);
        GameObject roomPrefab = roomAsset.GetRoom();
        GameObject newRoomObject = Instantiate(roomPrefab, transform.position + offset, transform.rotation, transform);
        RoomManager newRoom = newRoomObject.GetComponent<RoomManager>();
        this.grid[pos.x, pos.y] = newRoom;

        newRoom.SetDoors(doorMask);
        newRoom.SetRoomAsset(roomAsset);
        newRoom.NewRoom(new Vector2Int(pos.x, pos.y), roomCounter, depth, normalizedDepth, this);
        instantiatedRooms.Add(newRoom);
        
        roomCounter++;
    }
}

internal struct MazeCell{
    internal int doorMask;
    internal bool visited;
    internal int depth;
    internal int roomCounter;
    internal RoomType type;
}

//Recursive backtracker for maze generation.
internal class MazeGenerator{
    
    internal MazeCell[,] grid;
    internal Vector2Int desiredGridSize;  //How large of an area the algorithm is allowed to explore.
    internal Vector2Int actualGridSize;  //After generation, this will be how large the level actually is.
    private Vector2Int initPosition;
    internal int maxDepthReached = 0;
    private int seed = 0;
    private int maxDepth = 0;
    private int roomCount = 0;
    private int randomDoorIterations = 0;

    public MazeGenerator(Vector2Int desiredGridSize, int seed, int maxDepth, Vector2Int initPosition, int randomDoorIterations){
        this.grid = new MazeCell[desiredGridSize.x, desiredGridSize.y];
        this.desiredGridSize = desiredGridSize;
        this.seed = seed;
        this.maxDepth = maxDepth;
        this.initPosition = initPosition;
        this.randomDoorIterations = randomDoorIterations;
        GenerateMaze();
    }

    public void GenerateMaze(){
            Random.InitState(this.seed);

        MazeCrawl(this.initPosition, new Vector2Int(0,0), 0);
        PlaceRandomDoors();
        PlaceSpecialRooms();
        //Maze generation is done
        Debug.Log(string.Format("Maze generation done!\nNumber of rooms: {0}, Maximum Depth Reached: {1}, Actual Size: {2}", this.roomCount, this.maxDepthReached, this.actualGridSize));
    }

    private void MazeCrawl(Vector2Int pos, Vector2Int dir, int depth){
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(pos);
        PlaceRoom(pos, depth);
        AddDoorToMask(pos, dir);

        for (int i = 0; i < neighbors.Count; i++){
            int randomIndex = Random.Range(0, neighbors.Count);
            Vector2Int selectedNeighbor =  neighbors[randomIndex];

            if(depth >= maxDepth){
                neighbors.RemoveAt(randomIndex);
                break;
            }

            //If cell was already visited by another branch:
            if(grid[selectedNeighbor.x, selectedNeighbor.y].visited){
                neighbors.RemoveAt(randomIndex);
                continue;
            }

            Vector2Int newDir = (pos - selectedNeighbor);
            //Place new door as soon as we know in what direction.
            AddDoorToMask(pos, newDir * -1);
            neighbors.RemoveAt(randomIndex);
            MazeCrawl(selectedNeighbor, newDir, depth + 1);
        }
    }

    private void PlaceRandomDoors(){
        for (int i = 0; i < randomDoorIterations; i++){
            Vector2Int randomCoord = new Vector2Int(Random.Range(0, actualGridSize.x), Random.Range(0, actualGridSize.y));
            if(!grid[randomCoord.x, randomCoord.y].visited)
                continue;
            
            Vector2Int[] offsets = {new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)};
            Vector2Int randDir = offsets[Random.Range(0, offsets.Length)];
            Vector2Int newCoord = randDir + randomCoord;
            if(newCoord.x >= 0 && newCoord.x < this.desiredGridSize.x && newCoord.y >= 0 && newCoord.y < this.desiredGridSize.y){
                if(grid[newCoord.x, newCoord.y].visited){
                    AddDoorToMask(randomCoord, randDir);
                    AddDoorToMask(newCoord, randDir * -1);
                }
            }
        }
    }

    private void PlaceSpecialRooms(){
        PlaceInitialRoom();
        PlaceFinalRoom();
    }

    private void PlaceFinalRoom(){
        List<Vector2Int> potentialFinals = new List<Vector2Int>();
        for (int y = 0; y < this.actualGridSize.y; y++){
            for (int x = 0; x < this.actualGridSize.x; x++){
                if(this.grid[x,y].depth == maxDepthReached && this.grid[x,y].type == RoomType.COMMON)
                    potentialFinals.Add(new Vector2Int(x,y));
            }
        }
        int randomIndex = Random.Range(0, potentialFinals.Count);
        Vector2Int randomPos = potentialFinals[randomIndex];

        this.grid[randomPos.x, randomPos.y].type = RoomType.FINAL;
    }

    private void PlaceInitialRoom(){
        this.grid[this.initPosition.x, this.initPosition.y].type = RoomType.INITIAL;
    }

    private void PlaceRoom(Vector2Int pos, int depth){
        this.roomCount++;
        grid[pos.x, pos.y].visited = true;
        //Set cell-depth to depth unless already set previously.
        grid[pos.x, pos.y].depth = grid[pos.x, pos.y].depth == 0 ? depth : 0;
        grid[pos.x, pos.y].roomCounter = grid[pos.x, pos.y].roomCounter == 0 ? roomCount : 0;
        maxDepthReached = maxDepthReached < depth ? depth : maxDepthReached;

        //Update actual grid size if we've reached further than before.
        actualGridSize.x = pos.x + 1 > actualGridSize.x ? pos.x + 1 : actualGridSize.x;
        actualGridSize.y = pos.y + 1 > actualGridSize.y ? pos.y + 1 : actualGridSize.y;
    }

    //Use bit manipulation to additivly add directions to the mask.
    private void AddDoorToMask(Vector2Int pos, Vector2Int dir){
        if(dir.y > 0)
            grid[pos.x, pos.y].doorMask |= 0b1;
        if(dir.x > 0)
            grid[pos.x, pos.y].doorMask |= 0b10;
        if(dir.y < 0)
            grid[pos.x, pos.y].doorMask |= 0b100;
        if(dir.x < 0)
            grid[pos.x, pos.y].doorMask |= 0b1000;
    }

    //Get list of positions of all neigboring cells that have not been visited yet.
    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int position){
        List<Vector2Int> unvisited = new List<Vector2Int>();
        Vector2Int[] offsets = {new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)};

        for (int i = 0; i < offsets.Length; i++){
            Vector2Int newCoord = position + offsets[i];
            if(newCoord.x >= 0 && newCoord.x < this.desiredGridSize.x && newCoord.y >= 0 && newCoord.y < this.desiredGridSize.y){
                MazeCell activeCell = this.grid[newCoord.x, newCoord.y];
                if(!activeCell.visited)
                    unvisited.Add(newCoord);
            }
        }
        return unvisited;
    }
}