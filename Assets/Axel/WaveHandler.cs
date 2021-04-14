using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class WaveHandler
{
    private UnityEvent onCombatComplete = new UnityEvent();
    private List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
    private List<EnemySpawnPoint> activeSpawnPoints = new List<EnemySpawnPoint>();
    private float roomDifficulty = 0.0f;
    private float accumulatedDifficulty = 0.0f;

    public WaveHandler(UnityEvent onCombatComplete, List<EnemySpawnPoint> spawnPoints, float difficulty){
        this.onCombatComplete = onCombatComplete;
        this.spawnPoints = spawnPoints;
        this.roomDifficulty = difficulty;

        for (int i = 0; i < spawnPoints.Count; i++){
            spawnPoints[i].SetWaveHandler(this);
        }
    }
    
    public void ReportDeath(EnemySpawnPoint point){
        activeSpawnPoints.Remove(point);

        if(IsWaveOver())
            onCombatComplete.Invoke();
    }

    private bool IsWaveOver(){
        return activeSpawnPoints.Count == 0;
    }

    //Returns how many enemies were spawned.
    public int Start(){
        List<EnemySpawnPoint> spawnPoints = ShuffleSpawnPoints();
        
        int guaranteedEnemyCount = 0;
        int naturalEnemyCount = 0;

        //First spawn all garanteed spawns.
        for (int i = 0; i < spawnPoints.Count; i++){
            if(spawnPoints[i].IsGuaranteedSpawn()){
                spawnPoints[i].SpawnRandomEnemy();
                activeSpawnPoints.Add(spawnPoints[i]);
                guaranteedEnemyCount++;
            }
        }

        //Then randomize the rest.
        for (int i = 0; i < spawnPoints.Count; i++){
            //We've already spawned this one.
            if(spawnPoints[i].IsGuaranteedSpawn())
                continue;

            accumulatedDifficulty += spawnPoints[i].SpawnRandomEnemy();
            activeSpawnPoints.Add(spawnPoints[i]);
            naturalEnemyCount++;

            if(accumulatedDifficulty >= roomDifficulty)
                break;
        }

        if(naturalEnemyCount + guaranteedEnemyCount > 0)
            Debug.Log(string.Format("Entering new room. Difficulty: {0}, Enemies Spawned: {1}\nNatural: {2} - Guaranteed: {3}", this.roomDifficulty, naturalEnemyCount + guaranteedEnemyCount, naturalEnemyCount, guaranteedEnemyCount));

        return naturalEnemyCount + guaranteedEnemyCount;
    }

    private List<EnemySpawnPoint> ShuffleSpawnPoints(){
        List<EnemySpawnPoint> originalSpawnPoints = this.spawnPoints;
        List<EnemySpawnPoint> shuffledPoints = new List<EnemySpawnPoint>();
        while(originalSpawnPoints.Count > 0){
            int randIndex = Random.Range(0, originalSpawnPoints.Count);
            shuffledPoints.Add(originalSpawnPoints[randIndex]);
            originalSpawnPoints.RemoveAt(randIndex);
        }
        return shuffledPoints;
    }

    public static float CalculateDifficulty(float normalizedDepth, Vector2 randomRange, float randomness){
        return Mathf.Lerp(randomRange.x, randomRange.y, normalizedDepth) + Random.Range(-randomness, randomness);
    }
}
