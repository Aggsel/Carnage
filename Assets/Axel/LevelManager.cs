﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //User accessible variables
    [Header("Generation Attributes")]
    [Tooltip("Seed for the random generation.")]
    [SerializeField] private int seed = 0;
    [Tooltip("What prefabs CAN be placed during generation.")]
    [SerializeField] private LevelAsset level = null;
    [Tooltip("How far away from the start room any room is allowed, e.g. 4 Max Depth means no room is allowed further away than 4 rooms from the start room.")]
    [SerializeField] private int maxDepth = 5;

    [Header("Grid Settings")]
    [Tooltip("Room size in world units.")]
    [SerializeField] private Vector2 roomSize = new Vector2(10, 10);
    [Tooltip("How large the underlying grid for the generation should be. No room is placed outside this grid.")]
    [SerializeField] private Vector2Int desiredLevelGridSize = new Vector2Int(10, 10);
    [Tooltip("Where in the grid the initial room should spawn.")]
    [SerializeField] private Vector2Int spawnRoomLocation = new Vector2Int(0, 0);

    //Debug
    [Header("Debug Variables")]
    [SerializeField] float itterationOffset = 1.0f;

    //Hidden variables
    [HideInInspector] [SerializeField] private List<RoomManager> instantiatedRooms = new List<RoomManager>();
    [HideInInspector] [SerializeField] private MazeGenerator maze;
    private int roomCounter = 0;
    private RoomManager[,] grid;

    [ContextMenu("Generate Level")]
    private void GenerateLevel(){
        //If we already have generated a level, make sure we remove the first one first.
        if(this.maze != null)
            DeleteLevel();

        roomCounter = 0;
        this.maze = new MazeGenerator(desiredLevelGridSize, seed, maxDepth, spawnRoomLocation);
        this.grid = new RoomManager[desiredLevelGridSize.x, desiredLevelGridSize.y];
        PopulateLevel();
    }

    [ContextMenu("Delete Level")]
    private void DeleteLevel(){
        for (int i = 0; i < instantiatedRooms.Count; i++){
            if(instantiatedRooms[i] != null)
                DestroyImmediate(instantiatedRooms[i].gameObject);
        }
        instantiatedRooms = new List<RoomManager>();

        //Yikes?
        this.maze = null;
    }

    private void PopulateLevel(){
        //Should this really access the maze gridsize like this?
        for (int y = 0; y < this.maze.desiredGridSize.y; y++){
            for (int x = 0; x < this.maze.desiredGridSize.x; x++){
                if(this.maze.grid[x,y].visited){
                    PlaceRoom(new Vector2Int(x,y), this.maze.grid[x,y].depth, 0);
                    this.grid[x,y].SetDoorMask(this.maze.grid[x,y].doorMask);
                }  
            } 
        }
    }

    private void PlaceRoom(Vector2Int pos, int depth, int type){
        Vector3 offset = new Vector3(pos.x * roomSize.x, (float)roomCounter * itterationOffset, pos.y * roomSize.y);
        GameObject roomPrefab = level.GetRandomRoom();
        GameObject newRoomObject = Instantiate(roomPrefab, transform.position + offset, transform.rotation, transform);
        RoomManager newRoom = newRoomObject.GetComponent<RoomManager>();
        newRoom.gridPosition = new Vector2Int(pos.x, pos.y);
        this.grid[pos.x, pos.y] = newRoom;

        newRoom.roomID = roomCounter;
        newRoom.depth = depth;
        roomCounter++;
        
        instantiatedRooms.Add(newRoom);
    }
}

public struct MazeCell{
    public int doorMask;
    public bool visited;
    public int depth;
}

public class MazeGenerator{
    
    public MazeCell[,] grid;
    public Vector2Int desiredGridSize;  //How large of an area the algorithm is allowed to explore.
    private Vector2Int actualGridSize;  //After generation, this will be how large the level actually is.
    private Vector2Int initPosition;
    public int maxDepthReached = 0;
    private int seed = 0;
    private int maxDepth = 0;
    private int roomCount = 0;

    public MazeGenerator(Vector2Int desiredGridSize, int seed, int maxDepth, Vector2Int initPosition){
        this.grid = new MazeCell[desiredGridSize.x, desiredGridSize.y];
        this.desiredGridSize = desiredGridSize;
        this.seed = seed;
        this.maxDepth = maxDepth;
        this.initPosition = initPosition;
        GenerateMaze();
    }

    public void GenerateMaze(){
        Random.InitState(this.seed);

        MazeCrawl(this.initPosition, new Vector2Int(0,0), 0);
        //Maze generation is done
        Debug.Log(string.Format("Maze generation done!\nNumber of rooms: {0}, Maximum Depth Reached: {1}, Actual Size: {2}", this.roomCount, this.maxDepthReached, this.actualGridSize));
    }

    private void MazeCrawl(Vector2Int pos, Vector2Int dir, int depth){
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(pos);
        PlaceRoom(pos, depth);
        DirToDoorMask(pos, dir);

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
            DirToDoorMask(pos, newDir * -1);

            neighbors.RemoveAt(randomIndex);
            MazeCrawl(selectedNeighbor, newDir, depth + 1);
        }
    }

    private void PlaceRoom(Vector2Int pos, int depth){
        this.roomCount++;
        grid[pos.x, pos.y].visited = true;
        //Set cell-depth to depth unless already set previously.
        grid[pos.x, pos.y].depth = grid[pos.x, pos.y].depth == 0 ? depth : 0;
        maxDepthReached = maxDepthReached < depth ? depth : maxDepthReached;

        //Update actual grid size if we've reached further than before.
        actualGridSize.x = pos.x + 1 > actualGridSize.x ? pos.x + 1 : actualGridSize.x;
        actualGridSize.y = pos.y + 1 > actualGridSize.y ? pos.y + 1 : actualGridSize.y;
    }

    private void DirToDoorMask(Vector2Int pos, Vector2Int dir){
        //Use bit manipulation to additivly add directions to the mask.
        if(dir.y > 0)
            grid[pos.x, pos.y].doorMask |= 0b1;
        if(dir.x > 0)
            grid[pos.x, pos.y].doorMask |= 0b10;
        if(dir.y < 0)
            grid[pos.x, pos.y].doorMask |= 0b100;
        if(dir.x < 0)
            grid[pos.x, pos.y].doorMask |= 0b1000;
    }

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