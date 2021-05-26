using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("Assignables: ")]
    [SerializeField] GameObject tutorialObject = null;
    [SerializeField] TextMeshProUGUI tutorialText = null;
    [SerializeField] TextMeshProUGUI nextText = null;
    [SerializeField] TextMeshProUGUI previousText = null;

    [Header("Tutorial Info: ")]
    [TextArea(10, 15)]
    public string[] tutorialInfo = null;

    private bool skipTutorial = false; //save this
    private PauseController pc;
    private int tutorialIndex = -1;

    private Color startColor = new Color(0, 0, 0, 0);

    private void Start()
    {
        tutorialObject.SetActive(false);
        startColor = nextText.color;
        pc = GetComponent<PauseController>();

        tutorialIndex = -1;
        tutorialText.text = "";
        previousText.color = new Color(0.2f, 0.2f, 0.2f, 0.2f);

        if (!skipTutorial && SceneManager.GetActiveScene().name == "Level1")
        {
            pc.ButtonTutorialQuestion();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            TriggerNextTutorial();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            TriggerPreviousTutorial();
        }
    }

    private void TriggerNextTutorial ()
    {
        if(tutorialIndex < tutorialInfo.Length - 1)
        {
            tutorialIndex++;
            tutorialText.text = tutorialInfo[tutorialIndex].ToString();
            UpdateTutorialTexts();
        }
    }

    private void TriggerPreviousTutorial()
    {
        if (tutorialIndex > 0)
        {
            tutorialIndex--;
            tutorialText.text = tutorialInfo[tutorialIndex].ToString();
            UpdateTutorialTexts();
        }
    }

    private void UpdateTutorialTexts ()
    {
        if (tutorialIndex == 0)
        {
            previousText.color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        }
        else if(tutorialIndex == tutorialInfo.Length - 1)
        {
            nextText.color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        }
        else
        {
            nextText.color = startColor;
            previousText.color = startColor;
        }
    }

    public void TriggerNoTutorial()
    {
        tutorialObject.SetActive(false);
    }

    public void TriggerTutorial ()
    {
        tutorialObject.SetActive(true);
        TriggerNextTutorial();
    }
}
