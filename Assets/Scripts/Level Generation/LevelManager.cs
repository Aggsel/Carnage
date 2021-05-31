using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LevelManager : MonoBehaviour
{
    //User accessible variables
    [Header("Generation Attributes")]
    [Tooltip("Seed for the random generation.")]
    [SerializeField] private int seed = 0;
    [Tooltip("Will ignore the seed above if active.")]
    [SerializeField] private bool randomizeSeed = false;
    [Tooltip("What prefabs CAN be placed during generation.")]
    [SerializeField] private LevelAsset[] levels = null;

    [Tooltip("How much the difficulty should increase for each new level (in percent points). ")]
    [SerializeField] private float difficultyMultiplier = 1.0f;

    [Header("Grid Settings")]
    [Tooltip("Room size in world units.")]
    [SerializeField] private Vector2 roomSize = new Vector2(44, 44);
    [Tooltip("Where in the grid the initial room should spawn.")]
    [SerializeField] private Vector2Int spawnRoomLocation = new Vector2Int(0, 0);

    [Header("Callback")]
    [Tooltip("Will invoke all of these functions when level generation is complete.")]
    [SerializeField] private UnityEvent OnFinishedGeneration = null;
    [Tooltip("What event should be invoked when the player enters a room for the first time.")]
    [SerializeField] private GameEvent onRoomEnterFirst = null;
    [Tooltip("What event should be invoked when the player clears a room and the combat is complete.")]
    [SerializeField] private GameEvent onCombatComplete = null;
    [Tooltip("What event should be invoked whenever the player enters a room and enemies spawn.")]
    [SerializeField] private GameEvent onCombatStart = null;

    [Header("UI References")]
    [Tooltip("Reference to the UI element that is showing how many rooms have been cleared on the floor so far.")]
    [SerializeField] private TextMeshProUGUI progressionUIReference = null;
    [SerializeField] private MapDrawer mapReference = null;

    private GameObject playerReference = null;
    private AudioManager am = null;
    private UIController uic = null;

    //Hidden variables
    [HideInInspector] [SerializeField] private List<RoomManager> instantiatedRooms = new List<RoomManager>();
    [HideInInspector] [SerializeField] private MazeGenerator maze;
    private int roomCounter = 0;
    private int completedRooms = 0;
    private int currentLevel = 0;
    private float currentLevelDifficultyMultiplier = 1.0f;
    private RoomManager[,] grid;


    void Start(){
        GenerateLevel();
        this.am = AudioManager.Instance;
        this.am.PlaySound(ref am.ambManager);
        this.am.SetParameterByName(ref am.ambManager, "Music Random", Mathf.Round(Random.Range(0.0f, 1.0f)));

        uic = FindObjectOfType<UIController>();

        //Ideally these should be referenced while still in editor. If not, as a last resort; try to find them.
        if(mapReference == null){
            //Must do it this way since the mapdrawer object is inactive.
            MapDrawer[] maps = Resources.FindObjectsOfTypeAll<MapDrawer>();
            if(maps.Length > 0)
                mapReference = maps[0];
            Debug.Log("Map reference is empty. Please attach a reference to the map object.", this);
        }
        if(progressionUIReference == null){
            progressionUIReference = GameObject.Find("Level Progression").GetComponent<TextMeshProUGUI>();
            Debug.Log("Progression UI reference is empty. Please attach a reference to the progression UI.", this);
        }
    }

    //Called by other object whenever level should progress.
    public void GoToNextLevel(){
        uic.StartCoroutine(uic.WhiteFade(false, 0.5f));
        roomCounter = 0;
        completedRooms = 0;

        currentLevel++;
        currentLevelDifficultyMultiplier += difficultyMultiplier;

        if (PlayerPrefs.HasKey("Act"))
        {
            if(PlayerPrefs.GetInt("Act") < (currentLevel + 1))
            {
                PlayerPrefs.SetInt("Act", currentLevel);
                Debug.Log("Act has been set to " + (currentLevel + 1));
            }
        }

        if(currentLevel >= levels.Length){
            EndOfFinalLevel();
            return;
        }

        this.am.SetParameterByName(ref am.ambManager, "Music Random", Mathf.Round(Random.Range(0.0f, 1.0f)));
        GenerateLevel();
        this.gameObject.transform.position = playerReference.transform.position; //Teleport level to player instead of player to level lol.
        uic.UIAlertText("Going to next level!", 1.0f);
    }

    private void EndOfFinalLevel(){
        uic.UIAlertText("End of final level!", 2.0f);
    }

    [ContextMenu("Generate Level")]
    private void GenerateLevel(){
        //If we already have generated a level, make sure we remove the first one first.
        if(this.maze != null)
            DeleteLevel();

        int seed = randomizeSeed ? System.Environment.TickCount : this.seed;

        Vector2Int desiredGridSize = levels[currentLevel].GetDesiredLevelGridSize();
        this.maze = new MazeGenerator(desiredGridSize, seed, levels[currentLevel].GetMaxDepth(), spawnRoomLocation, levels[currentLevel].GetRandomDoorIterations());
        this.grid = new RoomManager[desiredGridSize.x, desiredGridSize.y];

        PopulateLevel();
        InitialRoomDeactivation(spawnRoomLocation);
        ActivateNeighbors(spawnRoomLocation);
        UpdateProgressionUI();
        mapReference?.SetGrid(this.maze.grid, this.spawnRoomLocation);

        FindObjectOfType<MovementController>().SetSpawnPoint(playerReference.transform.position/*spawnRoomLocation * roomSize*/);

        OnFinishedGeneration.Invoke();
    }

    [ContextMenu("Delete Level")]
    private void DeleteLevel(){
        for (int i = 0; i < instantiatedRooms.Count; i++){
            if(instantiatedRooms[i] != null)
                Destroy(instantiatedRooms[i].gameObject);
        }
        instantiatedRooms = new List<RoomManager>();

        this.maze = null;
    }

    private void InitialRoomDeactivation(Vector2Int pos){
        for (int y = 0; y < this.maze.actualGridSize.y; y++){
            for (int x = 0; x < this.maze.actualGridSize.x; x++){
                if(Mathf.Abs(this.maze.grid[x,y].depth - this.maze.grid[pos.x, pos.y].depth) > 1)
                    this.grid[x,y]?.gameObject.SetActive(false);
                else
                    this.grid[x,y]?.gameObject.SetActive(true);
            }
        }
    }

    public void ActivateNeighbors(Vector2Int pos){
        for (int y = -2; y <= 2; y++){
            for (int x = -2; x <= 2; x++){
                int posX = pos.x + x;
                int posY = pos.y + y;
                if(posX < 0 || posX > this.maze.actualGridSize.x-1 || posY < 0 || posY > this.maze.actualGridSize.y -1)
                    continue;

                if(this.grid[posX,posY] == null)
                    continue;

                if(Mathf.Abs(this.maze.grid[posX,posY].depth - this.maze.grid[pos.x, pos.y].depth) > 1){
                    if(this.grid[posX,posY].gameObject.activeInHierarchy)
                        this.grid[posX,posY].gameObject.SetActive(false);
                }
                else
                    this.grid[posX,posY].gameObject.SetActive(true);
            }
        }
    }

    public void SetOnlyRoomActive(Vector2Int pos){
        for (int y = -2; y <= 2; y++){
            for (int x = -2; x <= 2; x++){
                int posX = pos.x + x;
                int posY = pos.y + y;
                if(posX < 0 || posX > this.maze.actualGridSize.x-1 || posY < 0 || posY > this.maze.actualGridSize.y -1)
                    continue;

                if(this.grid[posX,posY] == null)
                    continue;

                if(posX == pos.x && posY == pos.y)
                    this.grid[posX,posY].gameObject.SetActive(true);
                else if(this.grid[posX,posY].gameObject.activeInHierarchy)
                    this.grid[posX,posY].gameObject.SetActive(false);
            }
        }
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
        
        Vector3 offset = new Vector3(pos.x * roomSize.x, 0, pos.y * roomSize.y);
        RoomAsset roomAsset = levels[currentLevel].GetRandomRoom(this.maze.grid[pos.x, pos.y].doorMask, this.maze.grid[pos.x, pos.y].type);
        GameObject roomPrefab = roomAsset.GetRoom();
        GameObject newRoomObject = Instantiate(roomPrefab, transform.position + offset, transform.rotation, transform);
        RoomManager newRoom = newRoomObject.GetComponent<RoomManager>();
        this.grid[pos.x, pos.y] = newRoom;

        newRoom.SetDoors(doorMask);
        newRoom.SetRoomAsset(roomAsset);
        if(playerReference == null)
            playerReference = GameObject.FindObjectOfType<MovementController>().gameObject;

        newRoom.NewRoom(new Vector2Int(pos.x, pos.y), roomCounter, depth, normalizedDepth, this, playerReference, 
        currentLevelDifficultyMultiplier, onRoomEnterFirst, onCombatComplete, onCombatStart, mapReference);

        instantiatedRooms.Add(newRoom);
        
        roomCounter++;
    }

    public void IncrementCompletedRooms(){
        completedRooms++;
        UpdateProgressionUI();
    }

    // public void ProgressionUISetActive(bool enabled){
    //     progressionUIReference?.gameObject.SetActive(enabled);
    // }

    private void UpdateProgressionUI(){
        if(progressionUIReference != null)
            progressionUIReference.text = string.Format("{0, 2:D2}/{1, 2:D2}", completedRooms, roomCounter);
        else
            Debug.LogWarning("UI progression reference not set in the LevelManager", this.gameObject);
    }
}

public struct MazeCell{
    public int doorMask;
    public bool visited;
    public int depth;
    public int roomCounter;
    public RoomType type;
}

//Recursive backtracker for maze generation.
public class MazeGenerator{
    
    public MazeCell[,] grid;
    public Vector2Int desiredGridSize;  //How large of an area the algorithm is allowed to explore.
    public Vector2Int actualGridSize;  //After generation, this will be how large the level actually is.
    private Vector2Int initPosition;
    public int maxDepthReached = 0;
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
        PlaceSpecialRooms();
        if(randomDoorIterations > 0)
            PlaceRandomDoors();
    }

    private void MazeCrawl(Vector2Int pos, Vector2Int dir, int depth){
        List<Vector2Int> neighbors = GetUnvisitedNeighbors(pos);
        PlaceRoom(pos, depth);
        AddDoorToMask(pos, dir);

        while(neighbors.Count > 0){
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
            if(!grid[randomCoord.x, randomCoord.y].visited || grid[randomCoord.x, randomCoord.y].type != RoomType.COMMON)
                continue;
            
            Vector2Int[] offsets = {new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)};
            Vector2Int randDir = offsets[Random.Range(0, offsets.Length)];
            Vector2Int newCoord = randDir + randomCoord;
            if(newCoord.x >= 0 && newCoord.x < this.desiredGridSize.x && newCoord.y >= 0 && newCoord.y < this.desiredGridSize.y){
                if(grid[newCoord.x, newCoord.y].visited && grid[newCoord.x, newCoord.y].type == RoomType.COMMON){
                    AddDoorToMask(randomCoord, randDir);
                    AddDoorToMask(newCoord, randDir * -1);
                }
            }
        }
        RecalculateDepth(initPosition);
    }

    private void RecalculateDepth(Vector2Int pos){
        for (int y = 0; y < grid.GetLength(1); y++){
            for (int x = 0; x < grid.GetLength(0); x++){
                this.grid[x,y].visited = false;
            }
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(pos);
        grid[pos.x, pos.y].visited = true;
        int depth = 0;
        this.maxDepthReached = 0;

        while(queue.Count > 0){
            int depthLength = queue.Count;

            while(depthLength > 0){
                Vector2Int newPos = queue.Dequeue();
                roomCount++;

                grid[newPos.x, newPos.y].visited = true;
                grid[newPos.x, newPos.y].depth = depth;
                this.maxDepthReached = depth > this.maxDepthReached ? depth : this.maxDepthReached;

                int mask = this.grid[newPos.x, newPos.y].doorMask;
                List<Vector2Int> neighbors = new List<Vector2Int>();
                if((mask & 0b0001) == 0b0001)
                    neighbors.Add(new Vector2Int(0,1) + newPos);
                if((mask & 0b0010) == 0b0010)
                    neighbors.Add(new Vector2Int(1,0) + newPos);
                if((mask & 0b0100) == 0b0100)
                    neighbors.Add(new Vector2Int(0,-1) + newPos);
                if((mask & 0b1000) == 0b1000)
                    neighbors.Add(new Vector2Int(-1,0) + newPos);
                for (int i = 0; i < neighbors.Count; i++){
                    if(!grid[neighbors[i].x, neighbors[i].y].visited){
                        queue.Enqueue(neighbors[i]);
                    }
                }
                depthLength--;
            }
            depth++;
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