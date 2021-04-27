using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool mainMenu = false;
    private string version = null;

    private void Start ()
    {
        Time.timeScale = 1.0f;

        version = Application.version;

        //useful later
        mainMenu = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
    }

    private void OnGUI ()
    {
        GUI.Label(new Rect(Screen.width - 40, 10, 70, 50), "v " + version.ToString());
    }

    #region mainMenu crap
    public void StartButton ()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitButton ()
    {
        Application.Quit();
    }
    #endregion

}