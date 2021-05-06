using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawnpoint", menuName = "Spawn point", order = 1)]
public class EnemySpawnPointAsset : ScriptableObject
{
    [Header("Enemy pool")]
    [Tooltip("What types of enemies are allowed to spawn at spawn locations that are using this asset.")]
    [SerializeField] private List<GameObject> availableEnemies = new List<GameObject>();
    [Header("Spawn criteria")]
    [Tooltip(@"A threshold used to describe what depth must be met before a certain type of enemy is allowed to spawn. 
    Each element in this list corresponds to a enemy prefab in the list above. If element 0 in this list has a value of 0.5, the 
    enemy at element 0 in the other list is only allowed to spawn during the final 50% of the rooms in that level. If the value is 0.0, the enemy will always has a chance to spawn.")]
    [SerializeField] private List<float> normalizedDepthThreshold = new List<float>();
    [Tooltip(@"Similar to the Normalized Depth Threshold, but instead uses the difficulty (set in the room asset as difficulty range) to determine whether or not
    to allow enemies-types to spawn on locations using this data asset. If the value is negative, this threshold will be ignored and the enemy can always spawn regardless of difficulty in that particular room.")]
    [SerializeField] private List<float> difficultyThreshold = new List<float>();

    public GameObject GetRandomEnemy(float normalizedDepth = 0.0f, float difficulty = -1.0f){
        List<int> potentialEnemies = new List<int>();
        for (int i = 0; i < availableEnemies.Count; i++){
            if(normalizedDepthThreshold[i] < normalizedDepth)
                continue;
            if(difficultyThreshold[i] < difficulty && difficultyThreshold[i] > 0.0f)
                continue;
            potentialEnemies.Add(i);
        }

        if(potentialEnemies.Count <= 0)
            return null;

        int randIndex = Random.Range(0, potentialEnemies.Count);
        GameObject randomEnemy = availableEnemies[potentialEnemies[randIndex]];
        return randomEnemy;
    }

    void OnValidate(){
        while(normalizedDepthThreshold.Count < availableEnemies.Count){
            normalizedDepthThreshold.Add(1.0f);
        }
        while(normalizedDepthThreshold.Count > availableEnemies.Count){
            normalizedDepthThreshold.RemoveAt(normalizedDepthThreshold.Count - 1);
        }

        while(difficultyThreshold.Count < availableEnemies.Count){
            difficultyThreshold.Add(-1.0f);
        }
        while(difficultyThreshold.Count > availableEnemies.Count){
            difficultyThreshold.RemoveAt(difficultyThreshold.Count - 1);
        }
    }
}