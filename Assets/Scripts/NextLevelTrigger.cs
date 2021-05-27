using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextLevelTrigger : MonoBehaviour
{
    LevelManager levelManager = null;
    UIController uc = null;

    private void Start ()
    {
        uc = FindObjectOfType<UIController>();
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == 12)
        {
            uc.StartCoroutine(uc.WhiteFade(false, 0.5f));
            GoToNextLevel();
            this.enabled = false;
        }
    }

    private void GoToNextLevel(){
        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
        
        levelManager.GoToNextLevel();
    }
}
