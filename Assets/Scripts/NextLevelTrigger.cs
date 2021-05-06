using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextLevelTrigger : MonoBehaviour
{
    LevelManager levelManager = null;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == 12)
            GoToNextLevel();
    }

    private void GoToNextLevel(){
        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
        
        levelManager.GoToNextLevel();
    }
}
