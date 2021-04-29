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
    private int remainingWaves = 0;

    public WaveHandler(UnityEvent onCombatComplete, List<EnemySpawnPoint> spawnPoints, float difficulty, int numberOfWaves){
        this.onCombatComplete = onCombatComplete;
        this.spawnPoints = spawnPoints;
        this.roomDifficulty = difficulty;
        this.remainingWaves = numberOfWaves;

        for (int i = 0; i < spawnPoints.Count; i++){
            spawnPoints[i].SetWaveHandler(this);
        }
    }
    
    public void ReportDeath(EnemySpawnPoint point){
        activeSpawnPoints.Remove(point);
        if(IsWaveOver())
            NextWave();
    }

    private bool IsWaveOver(){
        return activeSpawnPoints.Count == 0;
    }

    private void NextWave(){
        if(remainingWaves <= 0)
            onCombatComplete.Invoke();
        else
            SpawnNewWave();
    }

    //Returns how many enemies were spawned.
    public int Start(){
        return SpawnNewWave();
    }

    //Returns how many enemies were spawned at this wave.
    private int SpawnNewWave(){
        List<EnemySpawnPoint> shuffledPoints = ShuffleSpawnPoints();
    
        float accumulatedDifficulty = 0.0f;
        int guaranteedEnemyCount = 0;
        int naturalEnemyCount = 0;

        //First spawn all garanteed spawns.
        for (int i = 0; i < shuffledPoints.Count; i++){
            if(shuffledPoints[i].IsGuaranteedSpawn()){
                shuffledPoints[i].SpawnRandomEnemy();
                activeSpawnPoints.Add(shuffledPoints[i]);
                guaranteedEnemyCount++;
            }
        }

        //Then randomize the rest.
        for (int i = 0; i < shuffledPoints.Count; i++){
            //We've already spawned this one.
            if(shuffledPoints[i].IsGuaranteedSpawn())
                continue;

            if(accumulatedDifficulty >= roomDifficulty)
                break;
                
            accumulatedDifficulty += shuffledPoints[i].SpawnRandomEnemy();
            activeSpawnPoints.Add(shuffledPoints[i]);
            naturalEnemyCount++;
        }
        this.remainingWaves--;
        return naturalEnemyCount + guaranteedEnemyCount;
    }

    private List<EnemySpawnPoint> ShuffleSpawnPoints(){
        List<EnemySpawnPoint> originalSpawnPoints = new List<EnemySpawnPoint>(this.spawnPoints);
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
