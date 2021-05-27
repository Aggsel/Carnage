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
    private SerializeController sc = null;

    private void Start ()
    {
        sc = FindObjectOfType<SerializeController>();

        if(mainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Time.timeScale = 1.0f;

        version = Application.version;
        versionText.text = "v " + version.ToString();

        //useful later
        mainMenu = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
    }

    #region mainMenu crap
    public void StartButton ()
    {
        if(sc.GetFirstTime() == 1)
        {
            sc.SetFirstTime(2);
            SceneManager.LoadScene("Level1");
        }
        else if (sc.GetFirstTime() == 2)
        {
            SceneManager.LoadScene("Actual_Hub");
        }
        else
        {
            Debug.LogWarning("This should not happen!");
        }
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