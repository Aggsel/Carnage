using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText = null;
    [SerializeField] private GameObject[] objects = null;

    private bool mainMenu = false;
    private string version = null;

    private void Start ()
    {
        Time.timeScale = 1.0f;

        version = Application.version;
        versionText.text = "v " + version.ToString();

        //useful later
        mainMenu = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
    }

    /*private void OnGUI ()
    {
        GUI.Label(new Rect(Screen.width - 40, 10, 70, 50), "v " + version.ToString());
    }*/

    #region mainMenu crap
    public void StartButton ()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitButton ()
    {
        Application.Quit();
    }

    public void CreditsButton ()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
        }

        objects[1].SetActive(true);
    }

    public void BackButton ()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
        }

        objects[0].SetActive(true);
    }
    #endregion

}