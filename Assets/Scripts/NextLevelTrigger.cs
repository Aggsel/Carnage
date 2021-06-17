using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NextLevelTrigger : MonoBehaviour
{
    LevelManager levelManager = null;
    UIController uc = null;
    [SerializeField] Collider triggerCollider = null;

    [HideInInspector]
    public bool inPoem = false; //very gay bool

    private void Start ()
    {
        uc = FindObjectOfType<UIController>();
    }

    private void Update ()
    {
        //skip poem
        if (Input.GetKeyDown(KeyCode.Q) && inPoem)
        {
            GameObject player = GameObject.Find("Player");
            StopCoroutine(LevelLoadDelay());
            uc.DisablePoem();
            NextLevelSkip(player);
        }
    }

    private IEnumerator LevelLoadDelay()
    {
        FindObjectOfType<ChallangeController>().StopCurrentChallange();
        AudioManager.Instance.PlaySound(ref AudioManager.Instance.endOfLevelBell);
        float time = uc.DisplayPoemText();
        GameObject player = GameObject.Find("Player");

        if (time > 0.0f){
            player.GetComponent<MovementController>().enabled = false;
            player.GetComponent<FiringController>().enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            yield return new WaitForSeconds(time - time/8.0f);
            NextLevelSkip(player);
        }
        else
        {
            NextLevelSkip(player);
        }
    }

    private void NextLevelSkip (GameObject player)
    {
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<FiringController>().enabled = true;
        player.GetComponent<MovementController>().enabled = true;
        AudioManager.Instance.StopSound(ref AudioManager.Instance.endOfLevelBell);
        inPoem = false;
        GoToNextLevel();
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
