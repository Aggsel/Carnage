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
    private Vector2Int gridPosition;
    private int roomID = -1; //Serial number for the room. In generation order. 
    private int depth = -1; //How far from the initial room this room is.
    private float normalizedDepth = 0.0f;
    [HideInInspector] [SerializeField] private DoorPlacer[] doorPlacers = new DoorPlacer[4];
    [HideInInspector] [SerializeField] private List<Door> doors = new List<Door>();
    [HideInInspector] [SerializeField] private RoomAsset roomAsset;
    private UnityEvent onCombatComplete = new UnityEvent();
    private int doorMask = 0;
    private LevelManager parentLevelManager = null;
    private bool hasBeenVisited = false;
    private GameObject playerReference = null;
    private float difficultyMultiplier = 1.0f;
    
    private GameEvent onRoomEnterFirstGameEvent;
    private GameEvent onCombatCompleteGameEvent;
    private GameEvent onCombatStartGameEvent;

    private UIController uic;
    private AudioManager am;
    private MapDrawer mapReference;
    private TutorialController tc = null;

    [Header("Enemy Spawning")]
    [SerializeField] List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
    [SerializeField] WaveHandler waveHandler;

    [Header("Item Spawn Points")]
    [SerializeField] private Transform itemSpawnPoint = null;
    [SerializeField] private GameObject trailPrefab = null;

    void OnEnable(){
        onCombatComplete.AddListener(OnCombatComplete);
        am = AudioManager.Instance;
        if(uic == null) 
            uic = FindObjectOfType<UIController>();

        if (tc == null)
            tc = FindObjectOfType<TutorialController>();
    }

    void OnDisable(){
        onCombatComplete.RemoveListener(OnCombatComplete);
    }

    public void OnEnterRoom(){
        if(!hasBeenVisited)
            OnEnterRoomFirstTime();
        else{
            this.parentLevelManager.ActivateNeighbors(this.gridPosition);
        }
        mapReference?.SetRoomAsVisited(this.gridPosition);
        mapReference?.SetCurrentRoom(this.gridPosition);
    }

    //Is called whenever a room is entered for the first time.
    public void OnEnterRoomFirstTime(){
        //disable tutorial if enabled
        if(tc == null)
            tc = FindObjectOfType<TutorialController>();

        if(tc.GetTutorial())
        {
            tc.TriggerNoTutorial();
        }

        float difficulty = WaveHandler.CalculateDifficulty(normalizedDepth, roomAsset.GetDifficultyRange(), roomAsset.GetRandomness());
        difficulty = difficulty * difficultyMultiplier;
        //playerReference should NEVER be null here, but just to be sure.
        if(playerReference == null)
            playerReference = GameObject.FindObjectOfType<MovementController>().gameObject;
        this.waveHandler = new WaveHandler(onCombatComplete, spawnPoints, difficulty, roomAsset.GetEnemyWaveCount(), playerReference, normalizedDepth);
        int enemyCount = waveHandler.Start();
        
        //Close door if any enemies were spawned.
        if(enemyCount > 0){
            OpenDoors(false);
            am.SetParameterByName(ref am.ambManager, "Battle", 1.0f);
            am.SetParameterByName(ref am.ambManager, "State", 1.0f);
            onCombatStartGameEvent?.Invoke();
            this.parentLevelManager.SetOnlyRoomActive(this.gridPosition);
        }else{  //No enemies were spawned, consider the room completed.
            parentLevelManager?.IncrementCompletedRooms();
        }
        onRoomEnterFirstGameEvent?.Invoke();
        hasBeenVisited = true;
    }

    //Is called whenever wavehandler has finished the last wave.
    private void OnCombatComplete(){
        OpenDoors(true);
        am.SetParameterByName(ref am.ambManager, "Battle", 0.0f);
        am.SetParameterByName(ref am.ambManager, "State", 0.0f);
        parentLevelManager?.IncrementCompletedRooms();
        uic?.UIAlertText("Combat complete!", 1.5f);
        SpawnItem();
        onCombatCompleteGameEvent?.Invoke();

        this.parentLevelManager.ActivateNeighbors(this.gridPosition);
    }

    private void SpawnItem(){
        if(itemSpawnPoint != null) {
            GameObject spawnPrefab = roomAsset.GetItemSpawnPrefab();

            if(spawnPrefab != null) {
                GameObject newItem = Instantiate(spawnPrefab) as GameObject;
                newItem.transform.SetPositionAndRotation(itemSpawnPoint.position, Quaternion.identity);
                newItem.transform.SetParent(transform, true);

                //trail stuff
                newItem.SetActive(false);
                GameObject newTrail = Instantiate(trailPrefab, playerReference.transform.position, Quaternion.identity);
                newTrail.GetComponent<ItemTrail>().SetStuff(newItem);
            }
        }
    }

    private void OpenDoors(bool open){
        for (int i = 0; i < doors.Count; i++){
            if(doors[i] != null)
                doors[i].OpenDoor(open);
        }
    }

    public void NewRoom(Vector2Int gridPos, int roomID = -1, int depth = -1, float normalizedDepth = 0.0f, 
    LevelManager newManager = null, GameObject playerReference = null, float difficultyMultiplier = 1.0f,
    GameEvent onRoomEnterFirstTime = null, GameEvent onCombatComplete = null, GameEvent onCombatStart = null, MapDrawer mapReference = null){
        this.gridPosition = gridPos;
        this.roomID = roomID;
        this.depth = depth;
        this.normalizedDepth = normalizedDepth;
        this.parentLevelManager = newManager;
        this.playerReference = playerReference;
        this.difficultyMultiplier = difficultyMultiplier;
        this.onRoomEnterFirstGameEvent = onRoomEnterFirstTime;
        this.onCombatCompleteGameEvent = onCombatComplete;
        this.onCombatStartGameEvent = onCombatStart;
        this.mapReference = mapReference;
        //MergeMeshes();
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
        //tempDoors is a HACK! Doorplacers should ALWAYS be a length of four.
        this.doorPlacers = GetComponentsInChildren<DoorPlacer>();
        DoorPlacer[] tempDoors = new DoorPlacer[4];
        for (int i = 0; i < doorPlacers.Length; i++){
            Vector3 doorRelativePos = (doorPlacers[i].transform.position - transform.position).normalized;
            float angle = Mathf.Round(Quaternion.FromToRotation(Vector3.forward, doorRelativePos).eulerAngles.y / 90);
            switch (angle){
                case 0:
                    tempDoors[0] = doorPlacers[i];
                    this.doorMask |= 0b1;
                    break;
                case 1:
                    tempDoors[1] = doorPlacers[i];
                    this.doorMask |= 0b10;
                    break;
                case 2:
                    tempDoors[2] = doorPlacers[i];
                    this.doorMask |= 0b100;
                    break;
                case 3:
                    tempDoors[3] = doorPlacers[i];
                    this.doorMask |= 0b1000;
                    break;
            }
        }
        this.doorPlacers = tempDoors;
    }

    public void MergeMeshes(){
        MeshRenderer[] renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        HashSet<Material> materialsMerge = new HashSet<Material>();

        for (int i = 0; i < renderers.Length; i++){
            materialsMerge.Add(renderers[i].sharedMaterial);
        }

        Material[] validMaterials = new Material[materialsMerge.Count];
        materialsMerge.CopyTo(validMaterials);
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        for (int j = 0; j < validMaterials.Length; j++){
            GameObject roomMesh = new GameObject(string.Format("RoomMesh {0}", j), typeof(MeshFilter), typeof(MeshRenderer));
            roomMesh.transform.SetParent(this.transform);
            roomMesh.isStatic = true;

            Quaternion oldRot = transform.rotation;
            Vector3 oldPos = transform.position;

            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;

            Mesh finalMesh = new Mesh();

            CombineInstance[] combiners = new CombineInstance[meshFilters.Length];
            int meshCount = 0;

            for (int i = 0; i < meshFilters.Length; i++){
                MeshRenderer currentMeshRenderer = meshFilters[i].GetComponent<MeshRenderer>();
                bool destroyObject = false;

                if(currentMeshRenderer.sharedMaterial != validMaterials[j])
                    continue;
                if(meshFilters[i].sharedMesh == null)
                    continue;

                if(currentMeshRenderer.gameObject.GetComponents<Component>().Length <= 3){
                    Component[] components = currentMeshRenderer.gameObject.GetComponents<Component>();
                    //We ONLY want to merge and destroy game objects that ONLY have MeshFilters, MeshRenderers and Transforms.
                    //These are the objects that we're merging and creating new ones for.
                    //Objects with more specialized are difficult to merge because the component data must be transferred to the new mesh object.
                    for (int k = 0; k < components.Length; k++){
                        if(!Types.Equals(components[k], typeof(MeshRenderer)) || !Types.Equals(components[k], typeof(MeshFilter)) || !Types.Equals(components[k], typeof(Transform))){
                            destroyObject = true;
                            break;
                        }
                    }
                }

                if(destroyObject){
                    //Before removing, make sure we transfer all the objects children up one step.
                    //We might not want to remove them. And if we do, we will in another loop iteration.
                    foreach(Transform child in currentMeshRenderer.transform){
                        child.SetParent(currentMeshRenderer.transform.parent);
                    }
                    Destroy(currentMeshRenderer.gameObject);
                }
                else{
                        continue;
                }

                combiners[i].subMeshIndex = 0;
                combiners[i].mesh = meshFilters[i].sharedMesh;
                combiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
                combiners[i].lightmapScaleOffset = currentMeshRenderer.lightmapScaleOffset;
                meshCount++;
            }
            CombineInstance[] newCombiners = new CombineInstance[meshCount];
            meshCount = 0;
            for (int i = 0; i < combiners.Length; i++){
                if(combiners[i].mesh != null){
                    newCombiners[meshCount] = combiners[i];
                    meshCount++;
                }
            }

            finalMesh.CombineMeshes(newCombiners, true, true, true);

            MeshFilter meshFilter = roomMesh.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = roomMesh.GetComponent<MeshRenderer>();
            
            meshFilter.sharedMesh = finalMesh;
            meshRenderer.material = validMaterials[j];
            meshRenderer.lightmapIndex = 0;

            transform.position = oldPos;
            transform.rotation = oldRot;
            roomMesh.transform.position = oldPos;
            roomMesh.transform.rotation = oldRot;
        }
    }

    //Will draw a sphere at the spawn/initial room.
    private void OnDrawGizmos(){
        if(this.depth == 0){
            Gizmos.DrawSphere(transform.position, 1.0f);
        }

        if(itemSpawnPoint != null){
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(itemSpawnPoint.position, 0.8f);
        }
    }
}