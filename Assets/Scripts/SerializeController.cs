using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

[Serializable]
public struct Preferences
{
    public string preferenceFilename;
    [TextArea(15, 20)]
    public string preferenceStartContent;
}

public class SerializeController : MonoBehaviour
{
    [SerializeField] private Preferences preferences = new Preferences();

    private PauseController pc;
    private string dir = "";

    //simple player prefs saving
    private int hideTutorial = 1; //1 is no (show it), 2 is yes (hide it)
    private int firstTime = 1; //1 is first time, 2 is not first time

    private void OnEnable()
    {
        pc = FindObjectOfType<PauseController>();

        string[] lines = null;
        dir = GetPreferenceDirectory();

        //On gamestart get tutorial stuff
        if(PlayerPrefs.GetInt("hideTutorial", 0) == 0)
        {
            PlayerPrefs.SetInt("hideTutorial", 1);
            hideTutorial = PlayerPrefs.GetInt("hideTutorial");
        }
        else
        {
            if(PlayerPrefs.GetInt("hideTutorial") == 1)
            {
                //PlayerPrefs.SetInt("hideTutorial", 2);
                hideTutorial = PlayerPrefs.GetInt("hideTutorial");
            }
            else if (PlayerPrefs.GetInt("hideTutorial") == 2)
            {
                hideTutorial = PlayerPrefs.GetInt("hideTutorial");
            }
            else
            {
                Debug.LogWarning("This should not happen!");
            }
        }

        //First time playing
        if (PlayerPrefs.GetInt("firstTime1", 0) == 0)
        {
            PlayerPrefs.SetInt("firstTime1", 1);
            firstTime = PlayerPrefs.GetInt("firstTime1");
        }
        else
        {
            firstTime = PlayerPrefs.GetInt("firstTime1");
        }

        //On gamestart load in or create preferences
        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!CheckPreferenceFile(dir))
            {
                //Debug.LogWarning("DID NOT FIND PREFERENCE FILE, CREATE ONE");
                CreateNewPreferences();
                lines = System.IO.File.ReadAllLines(dir);
                LoadPreferences(lines);
            }
            else
            {
                lines = System.IO.File.ReadAllLines(dir);
                LoadPreferences(lines);
            }
        }
    }

    //tutorial player prefs save
    public void SetHideTutorial (int i)
    {
        hideTutorial = i;
        //Debug.Log("SET: " + i);
        PlayerPrefs.SetInt("hideTutorial", hideTutorial);
    }

    public int GetHideTutorial ()
    {
        //Debug.Log("GET: " + hideTutorial);
        hideTutorial = PlayerPrefs.GetInt("hideTutorial");
        return hideTutorial;
    }

    //first time playing
    public void SetFirstTime (int i)
    {
        firstTime = i;
        PlayerPrefs.SetInt("firstTime1", firstTime);
    }

    public int GetFirstTime ()
    {
        return firstTime;
    }

    #region preferences
    private void CreateNewPreferences()
    {
        System.IO.File.WriteAllText(dir, preferences.preferenceStartContent);
    }

    private void WriteToPreferences (string text)
    {
        System.IO.File.WriteAllText(dir, text);
    }

    private string GetPreferenceDirectory ()
    {
        string directory = Directory.GetCurrentDirectory() + preferences.preferenceFilename;
        return directory;
    }

    private bool CheckPreferenceFile (string dirr)
    {
        if(System.IO.File.Exists(dirr))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //kinda disgusting
    private void LoadPreferences (string[] lines)
    {
        //0            1       2      3    4      5        6     7        8     9      10      11    12    13     14      15
        //sensitivity, sounds, music, fov, gamma, graphics dash, forward, back, pause, right, left, jump, melee, action, status
        //this does not check if the preferences is valid

        OptionAssignments oa = pc.GetOptions();

        //Sliders
        oa.mouseSlider.value = float.Parse(lines[0]);
        pc.ChangeSensitivity(oa.mouseSlider);

        oa.soundSlider.value = float.Parse(lines[1]);
        pc.ChangeSound(oa.soundSlider);

        oa.musicSlider.value = float.Parse(lines[2]);
        pc.ChangeMusic(oa.musicSlider);

        oa.fovSlider.value = float.Parse(lines[3]);
        pc.ChangeFov(oa.fovSlider);

        oa.gammaSlider.value = float.Parse(lines[4]);
        pc.ChangeGamma(oa.gammaSlider);

        oa.graphicsSlider.value = float.Parse(lines[5]);
        pc.ChangeGraphics(oa.graphicsSlider);

        //Debug.Log("Loaded graphics as " + oa.graphicsSlider.value);

        //Keybindings
        for (int i = 6; i < lines.Length; i++)
        {
            KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), lines[i]);
            pc.SetKeyBindings(i - 6, newKey);
        }
    }

    public void SavePreferences ()
    {
        OptionAssignments oa = pc.GetOptions();
        KeyBindAsignments ka = pc.GetKeybindings();

        //sliders
        string saveString = oa.mouseSlider.value.ToString() + "\n" +
            oa.soundSlider.value.ToString() + "\n" +
            oa.musicSlider.value.ToString() + "\n" +
            oa.fovSlider.value.ToString() + "\n" +
            oa.gammaSlider.value.ToString() + "\n" +
            oa.graphicsSlider.value.ToString() + "\n" +

            //keybindings
            ka.moveForward.ToString() + "\n" +
            ka.moveBack.ToString() + "\n" +
            ka.moveRight.ToString() + "\n" +
            ka.moveLeft.ToString() + "\n" +
            ka.dash.ToString() + "\n" +
            ka.pause.ToString() + "\n" +
            ka.jump.ToString() + "\n" +
            ka.melee.ToString() + "\n" +
            ka.action.ToString() + "\n" +
            ka.status.ToString();

        //Debug.Log("Saved graphics as " + oa.graphicsSlider.value.ToString());
        WriteToPreferences(saveString);
    }
    #endregion
}