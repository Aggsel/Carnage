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

    private IEnumerator LevelLoadDelay ()
    {
        uc.StartCoroutine(uc.WhiteFade(false, 0.1f));
        //string endText = "Cast from shackles which bound them, this bell shall ring out hope for the mentally ill and victory over mental illness";
        //uc.UIAlertText(endText, 4.0f);

        yield return new WaitForSeconds(0.5f);
        GoToNextLevel();
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == 12)
        {
            StartCoroutine(LevelLoadDelay());
        }
    }

    private void GoToNextLevel(){
        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
        
        levelManager.GoToNextLevel();
    }
}
