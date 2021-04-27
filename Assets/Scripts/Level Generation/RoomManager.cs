using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RoomType{
    COMMON,
    INITIAL,
    FINAL
}

public class RoomManager : MonoBehaviour
{
    [HideInInspector] private Vector2Int gridPosition;
    [HideInInspector] private int roomID = -1; //Serial number for the room. In generation order. 
    [HideInInspector] private int depth = -1; //How far from the initial room this room is.
    [HideInInspector] private float normalizedDepth = 0.0f;
    [HideInInspector] [SerializeField] private DoorPlacer[] doorPlacers = new DoorPlacer[4];
    [HideInInspector] [SerializeField] private List<Door> doors = new List<Door>();
    [HideInInspector] [SerializeField] private RoomAsset roomAsset;
    private UnityEvent onCombatComplete = new UnityEvent();
    private int doorMask = 0;
    private LevelManager parentLevelManager = null;
    private bool hasBeenVisited = false;
    
    [Header("Enemy Spawning")]
    [SerializeField] List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
    [SerializeField] WaveHandler waveHandler;

    [Header("Item Spawn Points")]
    [SerializeField] private Transform itemSpawnPoint = null;

    [Header("Mesh Merging")]
    [Tooltip(@"When merging the meshes the final combined mesh will just have one material. 
    The merging process will therefore only combine meshes from objects with this material on them.")]
    [SerializeField] private Material validMaterial;

    void OnEnable(){
        onCombatComplete.AddListener(OnCombatComplete);
    }

    void OnDisable(){
        onCombatComplete.RemoveListener(OnCombatComplete);
    }

    public void OnEnterRoom(){
        if(!hasBeenVisited)
            OnEnterRoomFirstTime();

        this.parentLevelManager.ActivateNeighbors(this.gridPosition);
    }

    //Is called whenever a room is entered for the first time.
    public void OnEnterRoomFirstTime(){
        float difficulty = WaveHandler.CalculateDifficulty(normalizedDepth, roomAsset.GetDifficultyRange(), roomAsset.GetRandomness());
        this.waveHandler = new WaveHandler(onCombatComplete, spawnPoints, difficulty, roomAsset.GetEnemyWaveCount());
        int enemyCount = waveHandler.Start();
        
        //Close door if any enemies were spawned.
        if(enemyCount > 0)
            OpenDoors(false);

        hasBeenVisited = true;
    }

    //Is called whenever wavehandler has finished the last wave.
    private void OnCombatComplete(){
        OpenDoors(true);
        
        SpawnItem();
    }

    private void SpawnItem(){
        if(itemSpawnPoint != null){
            GameObject spawnPrefab = roomAsset.GetItemSpawnPrefab();
            if(spawnPrefab != null)
                Instantiate(spawnPrefab, itemSpawnPoint);
        }
    }

    private void OpenDoors(bool open){
        for (int i = 0; i < doors.Count; i++){
            if(doors[i] != null)
                doors[i].OpenDoor(open);
        }
    }

    public void NewRoom(Vector2Int gridPos, int roomID = -1, int depth = -1, float normalizedDepth = 0.0f, LevelManager newManager = null){
        this.gridPosition = gridPos;
        this.roomID = roomID;
        this.depth = depth;
        this.normalizedDepth = normalizedDepth;
        this.parentLevelManager = newManager;
        MergeMeshes();
    }

    public void SetRoomAsset(RoomAsset roomAsset){
        this.roomAsset = roomAsset;
    }

    public void AddDoor(Door newDoor){
        this.doors.Add(newDoor);
    }

    //From a given mask, will turn correct walls into doors.
    public void SetDoors(int mask){
        int tempMask = mask;

        for (int i = 0; i < doorPlacers.Length; i++){
            if(doorPlacers[i] == null)
                CalculateDoorMask();

            if((tempMask & 0b1) == 1)
                doorPlacers[i].SetDoor(true, this);
            tempMask = tempMask >> 1;
        }
    }

    public int GetDoorMask(){
        CalculateDoorMask();
        return this.doorMask;
    }

    private void CalculateDoorMask(){
        this.doorPlacers = GetComponentsInChildren<DoorPlacer>();

        for (int i = 0; i < doorPlacers.Length; i++){
            Vector3 doorRelativePos = (doorPlacers[i].transform.position - transform.position).normalized;
            float angle = Mathf.Round(Quaternion.FromToRotation(Vector3.forward, doorRelativePos).eulerAngles.y / 90);
            switch (angle){
                case 0:
                    this.doorMask |= 0b1;
                    break;
                case 1:
                    this.doorMask |= 0b10;
                    break;
                case 2:
                    this.doorMask |= 0b100;
                    break;
                case 3:
                    this.doorMask |= 0b1000;
                    break;
            }
        }
    }

    public void MergeMeshes(){
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        Mesh finalMesh = new Mesh();

        CombineInstance[] combiners = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++){
            if(meshFilters[i].transform == transform)
                continue;

            MeshRenderer currentMeshRenderer = meshFilters[i].GetComponent<MeshRenderer>();

            if(currentMeshRenderer.sharedMaterial != validMaterial)
                continue;

            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = meshFilters[i].sharedMesh;
            combiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
            combiners[i].lightmapScaleOffset = currentMeshRenderer.lightmapScaleOffset;

            Destroy(meshFilters[i]);
            Destroy(currentMeshRenderer);
        }

        finalMesh.CombineMeshes(combiners, true, true, true);
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = finalMesh;
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = validMaterial;
        meshRenderer.lightmapIndex = 0;

        transform.position = oldPos;
        transform.rotation = oldRot;
    }

    //Will draw a sphere at the spawn/initial room.
    private void OnDrawGizmos(){
        if(this.depth == 0){
            Gizmos.DrawSphere(transform.position, 1.0f);
        }
    }
}