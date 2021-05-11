using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tutorialText = null;

    private bool skipTutorial = false; //save this
    private PauseController pc;
    private int sceneIndex = 0;

    private void Start ()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        pc = GetComponent<PauseController>();
        Debug.Log(sceneIndex);

        if(!skipTutorial && sceneIndex == 1)
        {
            pc.ButtonTutorialQuestion();
        }
    }
}
