using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("A asset describing which enemies can spawn on this")]
    [SerializeField] private EnemySpawnPointAsset spawnPointData = null;
    [SerializeField] private float spawnSafeZoneRadius = 8.0f;
    private List<EnemyBehavior> spawnedEnemies = new List<EnemyBehavior>();
    private WaveHandler waveHandler;
    private Queue<GameObject> enemySpawnQueue = new Queue<GameObject>();
    private GameObject player = null;

    [Tooltip("Will guarantee that this enemy spawns upon entering the room. The difficulty that this enemy adds to the room will not be taken into account.")]
    [SerializeField] private bool guaranteedSpawn = false;

    //Spawn random enemy and return how much that enemy
    //contributed to the difficulty score of the room.
    public float SpawnRandomEnemy(){
        GameObject randomEnemy = spawnPointData?.GetRandomEnemy(waveHandler.GetNormalizedDepth(), waveHandler.GetDifficulty());
        if(randomEnemy != null){
            enemySpawnQueue.Enqueue(randomEnemy);
            EnemyBehavior enemy = randomEnemy.GetComponent<EnemyBehavior>();
            StartCoroutine(RandomizeSpawnTiming());
            return enemy.GetDifficulty();
        }
        return 0.0f;
    }

    private IEnumerator RandomizeSpawnTiming(){
        while(enemySpawnQueue.Count > 0){
            yield return new WaitForSeconds(Random.Range(0.0f, 3.0f));
            while(Vector3.Distance(this.transform.position, player.transform.position) <= spawnSafeZoneRadius){
                yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
            }
            if(enemySpawnQueue.Count > 0)
            {
                GameObject newEnemy = enemySpawnQueue.Dequeue();
                newEnemy = Instantiate(newEnemy, transform);
                EnemyBehavior enemy = newEnemy.GetComponent<EnemyBehavior>();
                enemy.SetParentSpawn(this);
                spawnedEnemies.Add(enemy);
            }
        }
    }

    public void ReportDeath(EnemyBehavior enemy){
        spawnedEnemies.Remove(enemy);
        if(AreAllEnemiesDead()){
            waveHandler.ReportDeath(this);
        }
    }

    public void SetWaveHandler(WaveHandler waveHandler){
        this.waveHandler = waveHandler;
    }

    public void SetPlayerReference(GameObject player){
        this.player = player;
    }

    public bool IsGuaranteedSpawn(){
        return this.guaranteedSpawn;
    }

    private bool AreAllEnemiesDead(){
        return spawnedEnemies.Count == 0;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}