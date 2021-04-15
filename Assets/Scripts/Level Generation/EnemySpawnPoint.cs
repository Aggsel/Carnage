using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("Enemy prefab")]
    [SerializeField] private List<GameObject> availableEnemies = new List<GameObject>();
    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>();
    private WaveHandler waveHandler;

    [Tooltip("Will guarantee that this enemy spawns upon entering the room. The difficulty that this enemy adds to the room will not be taken into account.")]
    [SerializeField] private bool guaranteedSpawn = false;

    //Spawn random enemy and return how much that enemy
    //contributed to the difficulty score of the room.
    public float SpawnRandomEnemy(){
        int randIndex = Random.Range(0, availableEnemies.Count);
        GameObject randomEnemy = availableEnemies[randIndex];
        randomEnemy = Instantiate(randomEnemy, transform);

        EnemyBase enemy = randomEnemy.GetComponent<EnemyBase>();
        enemy.SetParentSpawn(this);
        spawnedEnemies.Add(enemy);

        //TODO: Return difficulty score.
        return 1.0f;
    }

    public void ReportDeath(EnemyBase enemy){
        spawnedEnemies.Remove(enemy);
        if(AreAllEnemiesDead()){
            waveHandler.ReportDeath(this);
        }

    }

    public void SetWaveHandler(WaveHandler waveHandler){
        this.waveHandler = waveHandler;
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