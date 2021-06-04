using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextLevelTrigger : MonoBehaviour
{
    LevelManager levelManager = null;
    UIController uc = null;
    [SerializeField] Collider triggerCollider = null;

    private void Start ()
    {
        uc = FindObjectOfType<UIController>();
    }

    private IEnumerator LevelLoadDelay()
    {
        AudioManager.Instance.PlaySound(ref AudioManager.Instance.endOfLevelBell);
        float time = uc.DisplayPoemText();
        if(time > 0.0f){
            GameObject player = GameObject.Find("Player");
            player.GetComponent<MovementController>().enabled = false;
            player.GetComponent<FiringController>().enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            yield return new WaitForSeconds(time - time/8.0f);
            GoToNextLevel();
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<FiringController>().enabled = true;
            player.GetComponent<MovementController>().enabled = true;
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
