using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText = null;
    [SerializeField] private GameObject[] objects = null;

    private bool mainMenu = false;
    private string version = null;
    private SerializeController sc = null;

    [SerializeField] private Image fadeImage = null;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Start ()
    {
        StartCoroutine(FadeFromBlack());

        sc = FindObjectOfType<SerializeController>();

        Time.timeScale = 1.0f;

        version = Application.version;
        versionText.text = "v " + version.ToString();

        //useful later
        mainMenu = SceneManager.GetActiveScene().name == "MainMenu" ? true : false;

        if (mainMenu)
        {
            AudioManager.Instance.PlaySound(ref AudioManager.Instance.mainMenuMusic);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (SceneManager.GetActiveScene().name == "Actual_Hub")
        {
            AudioManager.Instance.PlaySound(ref AudioManager.Instance.hubMusic);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update ()
    {
        //DEBUG, RESET FIRST TIME
        if(Input.GetKeyDown(KeyCode.Comma))
        {
            sc.SetFirstTime(1);
        }
    }

    #region mainMenu crap
    public void StartButton ()
    {
        AudioManager.Instance.StopSound(ref AudioManager.Instance.mainMenuMusic);
        if (sc.GetFirstTime() == 1)
        {
            
            sc.SetFirstTime(2);
            SceneManager.LoadScene("Alexander");
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

    private IEnumerator FadeFromBlack(){
        Color fadeColor = new Color(0,0,0,1.0f);
        fadeImage.color = fadeColor;
        float duration = fadeDuration;
        while(duration >= 0.0f){
            fadeColor.a = duration/fadeDuration;
            fadeImage.color = fadeColor;
            yield return null;
            duration -= Time.deltaTime;
        }
    }
}