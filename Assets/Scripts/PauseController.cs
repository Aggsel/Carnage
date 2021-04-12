using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    //[Tooltip("Main object of the menu which will appear when you pause the game")]
    //[SerializeField] private GameObject pauseMenu = null;
    [Tooltip("Programmer stuff, no touchy")]
    [SerializeField] private MonoBehaviour[] scripts = null;

    private bool paused = false;

    private void Start ()
    {
        paused = false;

        if(!paused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UpdatePause();
        }
    }

    private void UpdatePause ()
    {
        paused = !paused;

        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = !paused;
        }

        Time.timeScale = paused ? 0.0f : 1.0f; //maybe not
        Cursor.lockState = !paused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !paused;
        //pauseMenu.SetActive(!paused); //replace later
    }

    private void OnGUI ()
    {
        if(paused)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 50), "PAUSED");

            if(GUI.Button(new Rect(5, Screen.height - 55, 100, 50), "EXIT"))
            {
                Application.Quit();
            }

            if (GUI.Button(new Rect(5, Screen.height - 105, 100, 50), "OPTIONS"))
            {
                //Toggle options here
            }
        }
    }
}