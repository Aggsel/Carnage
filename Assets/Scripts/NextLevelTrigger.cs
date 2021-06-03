using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextLevelTrigger : MonoBehaviour
{
    LevelManager levelManager = null;
    UIController uc = null;
    [SerializeField ] Collider triggerCollider = null;

    private void Start ()
    {
        uc = FindObjectOfType<UIController>();
    }

    private IEnumerator LevelLoadDelay()
    {
        AudioManager.Instance.PlaySound(ref AudioManager.Instance.endOfLevelBell);
        float time = uc.DisplayPoemText();
        if(time > 0.0f){
            yield return new WaitForSeconds(time/2.0f);
            GoToNextLevel();
            yield return new WaitForSeconds(time/2.0f);
            AudioManager.Instance.StopSound(ref AudioManager.Instance.endOfLevelBell);
        }
        else{
            GoToNextLevel();
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == 12)
        {
            if(triggerCollider != null)
                triggerCollider.enabled = false;
            StartCoroutine(LevelLoadDelay());
        }
    }

    private void GoToNextLevel(){
        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        FindObjectOfType<BloodController>().ClearDecalPool();
        levelManager.GoToNextLevel();
    }
}
