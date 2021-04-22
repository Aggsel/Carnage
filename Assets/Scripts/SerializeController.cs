using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
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

    //[TextArea(15, 20)]
    //public string finLine;

    private void OnEnable()
    {
        pc = FindObjectOfType<PauseController>();

        string[] lines = null;
        dir = GetPreferenceDirectory();

        //On gamestart load in or create preferences
        if (!CheckPreferenceFile(dir))
        {
            Debug.LogWarning("DID NOT FIND PREFERENCE FILE, CREATE ONE");
            CreateNewPreferences();
        }
        else
        {
            lines = System.IO.File.ReadAllLines(dir);

            //before load check for weird shit
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], "[^\\w\\._]", "");
            }
            
            LoadPreferences(lines);
        }
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
        //0            1       2      3    4      5     6        7     8      9      10    11    12     13
        //sensitivity, sounds, music, fov, gamma, dash, forward, back, pause, right, left, jump, melee, action
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

        //Keybindings
        for (int i = 5; i < lines.Length; i++)
        {
            KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), lines[i]);
            pc.SetKeyBindings(i - 5, newKey);
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

            //keybindings
            ka.moveForward.ToString() + "\n" +
            ka.moveBack.ToString() + "\n" +
            ka.moveRight.ToString() + "\n" +
            ka.moveLeft.ToString() + "\n" +
            ka.dash.ToString() + "\n" +
            ka.pause.ToString() + "\n" +
            ka.jump.ToString() + "\n" +
            ka.melee.ToString() + "\n" +
            ka.action.ToString();

        WriteToPreferences(saveString);
    }
    #endregion
}