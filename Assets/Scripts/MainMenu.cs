using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private VolumeProfile profile = null;
    [SerializeField] private TextMeshProUGUI versionText = null;
    [SerializeField] private GameObject creditsText = null;
    [SerializeField] private TextMeshProUGUI descriptionText = null;
    [SerializeField] private GameObject[] objects = null;

    private bool mainMenu = false;
    private string version = null;
    private SerializeController sc = null;
    private bool scrollCredits = false;
    private Vector2 newPos = Vector2.zero;
    private Vector2 startPos = Vector2.zero;

    [SerializeField] private Image fadeImage = null;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Start ()
    {
        //think this fixed motionblur bug
        if (!profile.TryGet<MotionBlur>(out var motion))
        {
            Debug.LogWarning("THIS SHOULD NOT HAPPEN");
            motion = profile.Add<MotionBlur>(false);
            motion.active = false;
        }

        motion.active = false;

        startPos = creditsText.GetComponent<RectTransform>().position;
        creditsText.GetComponent<RectTransform>().position = startPos;
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (SceneManager.GetActiveScene().name == "Challange_Time")
        {
            //AudioManager.Instance.PlaySound(ref AudioManager.Instance.hubMusic);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update ()
    {
        //DEBUG, RESET FIRST TIME
        if(Input.GetKey(KeyCode.Comma) && Input.GetKey(KeyCode.Delete))
        {
            sc.SetFirstTime(1);
            PlayerPrefs.DeleteKey("Act");
        }

        if(scrollCredits)
        {
            newPos.y += 120f * Time.deltaTime;
            creditsText.GetComponent<RectTransform>().position = startPos + newPos;
        
            if(creditsText.GetComponent<RectTransform>().localPosition.y > 1800f)
            {
                Debug.Log(creditsText.GetComponent<RectTransform>().localPosition.y);
                newPos = Vector2.zero;
                creditsText.GetComponent<RectTransform>().position = startPos;
            }
        }
    }

    #region mainMenu crap
    
    public void TimeChallangeHover ()
    {
        descriptionText.text = "A set difficulty and levelsize where your run is being timed. The goal is simple, finish " +
            "the level as fast as possible and try to beat your own best time!";
    }

    public void StoryModeHover()
    {
        descriptionText.text = "The default story mode with three levels of different difficulties and sizes. Try to wrap your" +
            " head around the main characters story and debunk the insanities.";
    }

    public void LeaveHover ()
    {
        //reset description
        descriptionText.text = "";
    }

    public void TimeChallangeButton ()
    {
        AudioManager.Instance.StopSound(ref AudioManager.Instance.mainMenuMusic);
        SceneManager.LoadScene("Challange_Time");
    }

    public void StoryModeButton ()
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

    public void StartButton ()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
        }

        objects[2].SetActive(true);
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
        scrollCredits = true;
    }

    public void BackButton ()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
        }

        objects[0].SetActive(true);

        //reset credtis
        scrollCredits = false;
        newPos = Vector2.zero;
        creditsText.GetComponent<RectTransform>().position = startPos;
    }
    #endregion

    private IEnumerator FadeFromBlack(){
        if(fadeImage != null){
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
}