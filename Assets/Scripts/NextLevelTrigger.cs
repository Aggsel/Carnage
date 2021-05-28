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
        AudioManager.Instance.PlaySound(ref AudioManager.Instance.endOfLevelBell);
        uc.StartCoroutine(uc.WhiteFade(true, 0.5f));

        yield return new WaitForSeconds(4f);
        AudioManager.Instance.StopSound(ref AudioManager.Instance.endOfLevelBell);
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

        Debug.Log("NEW LEVEL");
        levelManager.GoToNextLevel();
    }
}
