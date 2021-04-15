using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Events;

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

[Serializable]
public struct KeyBindAsignments
{
    [Header("Movement keys: ")]
    public KeyCode moveForward; //0
    public KeyCode moveBack; //1
    public KeyCode moveRight; //2
    public KeyCode moveLeft; //3
    public KeyCode dash; //4
    public KeyCode pause; //5
    public KeyCode jump; //6
    public KeyCode melee; //7
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
    [SerializeField] private KeyBindAsignments keybindAssignments = new KeyBindAsignments();

    private bool paused = false;
    private int menuStage = 0; //nothing, pause, options
    private MovementController mc = null;

    private bool changingKey = false;
    private int changingKeyIndex = 0;
    //private Button changingKeyButton = null;
    private TextMeshProUGUI changingKeyText = null;
    private KeyCode changingKeycode = KeyCode.Exclaim;
    private bool keydown = false;

    //test
    public static UnityAction<KeyBindAsignments> updateKeysFunction = (_) => { }; //wtf är ens detta

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
        updateKeysFunction.Invoke(keybindAssignments);
    }

    private void Update ()
    {
        if(Input.GetKeyDown(keybindAssignments.pause) && !paused) //change to escape later just for testing
        {
            UpdatePause(true);
        }
    }

    //setting & changine keycode
    private void OnGUI ()
    {
        if(changingKey)
        {
            Event e = Event.current;

            /*if(e.type.Equals(EventType.KeyDown) && !keydown)
            {
                keydown = true;
                changingKeycode = e.keyCode;

                SetKeycode(changingKeyIndex, changingKeycode);
            }*/

            if(e.isKey && !keydown)
            {
                keydown = true;
                changingKeycode = e.keyCode;

                SetKeycode(changingKeyIndex, changingKeycode);
            }

            if(e.type.Equals(EventType.KeyUp))
            {
                keydown = false;
            }
        }
    }

    private void SetKeycode (int keyIndex, KeyCode key)
    {
        Debug.Log("Change " + keyIndex + " to " + key.ToString());
        keydown = false;

        switch (keyIndex)
        {
            case 0: //moveForward
                keybindAssignments.moveForward = key;
                break;
            case 1: //moveBack
                keybindAssignments.moveBack = key;
                break;
            case 2: //moveRight
                keybindAssignments.moveRight = key;
                break;
            case 3: //moveLeft
                keybindAssignments.moveLeft = key;
                break;
            case 4: //dash
                keybindAssignments.dash = key;
                break;
            case 5: //pause
                keybindAssignments.pause = key;
                break;
            case 6: //jump
                keybindAssignments.jump = key;
                break;
            case 7: //melee
                keybindAssignments.melee = key;
                break;
            default:
                break;
        }

        changingKey = false;
        changingKeyIndex = 0;
        updateKeysFunction.Invoke(keybindAssignments);

        LockCursor(true);
        //changingKeyButton.interactable = true;
        changingKeyText.text = key.ToString();
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

    private void LockCursor (bool yes)
    {
        Cursor.lockState = !yes ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = yes;
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
        LockCursor(paused);
    }
    #endregion
    
    #region keybinds
    //keybindings
    public void ChangeButton_Dash(TextMeshProUGUI text)
    {
        if(!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 4;
        }
    }

    public void ChangeButton_Forward(TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 0;
        }
    }

    public void ChangeButton_Back(TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 1;
        }
    }

    public void ChangeButton_Right(TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 2;
        }
    }

    public void ChangeButton_Left(TextMeshProUGUI text)
    {
        if (!changingKey)
        { 
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 3;
        }
    }

    public void ChangeButton_Pause(TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 5;
        }
    }

    public void ChangeButton_Jump (TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 6;
        }
    }

    public void ChangeButton_Melee (TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 7;
        }
    }
    #endregion

    #region button calls / options
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