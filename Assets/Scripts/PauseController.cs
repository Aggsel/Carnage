using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
///         Holds the pausing,
///         back to main menu and all option changes
/// </summary>

#region structs
[Serializable]
public struct OptionAssignments
{
    [Header("Mouse: ")]
    public TextMeshProUGUI mouseValue;
    [Header("Sound & Music: ")]
    public TextMeshProUGUI soundValue;
    public TextMeshProUGUI musicValue;
    [Header("FOV: ")]
    public TextMeshProUGUI fovValue;
    [Header("Post processing: ")]
    public TextMeshProUGUI gammaValue;
}
#endregion

public class PauseController : MonoBehaviour
{
    [Header("Set things, dont touch")]
    [SerializeField] private VolumeProfile profile = null;
    [Tooltip("Programmer stuff, no touchy")]
    [SerializeField] private MonoBehaviour[] scripts = null;
    [Tooltip("Programmer stuff, no touchy")]
    [SerializeField] private GameObject[] menuObjects = null;
    [SerializeField] private OptionAssignments optionAssignments = new OptionAssignments();

    private bool paused = false;
    private int menuStage = 0; //nothing, pause, options

    private MovementController mc = null;

    private void Start ()
    {
        mc = FindObjectOfType<MovementController>();

        //reset gamma, dont do this in build
        if (!profile.TryGet<LiftGammaGain>(out var gamma))
        {
            Debug.LogWarning("THIS SHOULD NOT HAPPEN");
            gamma = profile.Add<LiftGammaGain>(false);
        }
        gamma.gamma.value = new Vector4(gamma.gamma.value.x, gamma.gamma.value.y, gamma.gamma.value.z, 0.0f);


        UpdatePause(false);
    }

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !paused) //change to escape later just for testing
        {
            UpdatePause(true);
        }
    }

    #region main code
    private void UpdateUi (int index)
    {
        menuStage = index;

        for (int i = 0; i < menuObjects.Length; i++)
        {
            menuObjects[i].SetActive(false);
        }

        menuObjects[menuStage].SetActive(true);
    }

    private void UpdatePause (bool yes)
    {
        paused = yes;

        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = !paused;
        }

        if(paused)
        {
            UpdateUi(1);
        }
        else
        {
            UpdateUi(0);
        }

        Time.timeScale = paused ? 0.0f : 1.0f; //maybe not
        Cursor.lockState = !paused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = paused;
    }
    #endregion

    #region button calls
    //main pause
    public void ButtonResume ()
    {
        UpdatePause(false);
    }

    public void ButtonMenu()
    {
        Application.Quit();
    }
    
    public void ButtonOptions ()
    {
        UpdateUi(2);
    }

    public void ButtonBack ()
    {
        UpdateUi(1);
    }

    //options
    public void ChangeSensitivity (Slider slider)
    {
        mc.SetSensitivity(slider.value);
        optionAssignments.mouseValue.text = slider.value.ToString("F1");
    }

    public void ChangeSound(Slider slider)
    {
        //Do sound change here
        optionAssignments.soundValue.text = slider.value.ToString("F1") + "%";
    }

    public void ChangeMusic(Slider slider)
    {
        //Do music change here
        optionAssignments.musicValue.text = slider.value.ToString("F1") + "%";
    }

    public void ChangeFov (Slider slider)
    {
        Camera.main.fieldOfView = slider.value;
        optionAssignments.fovValue.text = slider.value.ToString("F1") + "°";
    }

    public void ChangeGamma (Slider slider)
    {
        if (!profile.TryGet<LiftGammaGain>(out var gamma))
        {
            Debug.LogWarning("THIS SHOULD NOT HAPPEN");
            gamma = profile.Add<LiftGammaGain>(false);
        }

        gamma.gamma.value = new Vector4(gamma.gamma.value.x, gamma.gamma.value.y, gamma.gamma.value.z, (slider.value) * 0.25f);
        optionAssignments.gammaValue.text = slider.value.ToString("F2");
    }

    public void ChangeInvertedControls ()
    {
        mc.SetInvertedControls();
    }
    #endregion
}