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
    public Slider mouseSlider;
    [Header("Sound & Music: ")]
    public TextMeshProUGUI soundValue;
    public Slider soundSlider;

    public TextMeshProUGUI musicValue;
    public Slider musicSlider;
    [Header("FOV: ")]
    public TextMeshProUGUI fovValue;
    public Slider fovSlider;
    [Header("Post processing: ")]
    public TextMeshProUGUI gammaValue;
    public Slider gammaSlider;
    [Header("Graphics settings: ")]
    public TextMeshProUGUI graphicsValue;
    public Slider graphicsSlider;
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
    public KeyCode action; //8
    public KeyCode status; //9
}

[Serializable]
public struct KeybindTexts
{
    public TextMeshProUGUI forwardText; //0
    public TextMeshProUGUI backText; //1
    public TextMeshProUGUI rightText; //2
    public TextMeshProUGUI leftText; //3
    public TextMeshProUGUI dashText; //4
    public TextMeshProUGUI pauseText; //5
    public TextMeshProUGUI jumpText; //6
    public TextMeshProUGUI meleeText; //7
    public TextMeshProUGUI actionText; //8
    public TextMeshProUGUI statusText; //9
}
#endregion

public class PauseController : MonoBehaviour
{
    [Header("Set things, dont touch")]
    [SerializeField] private VolumeProfile profile = null;
    [SerializeField] private MovementController mc = null;
    [Tooltip("Programmer stuff, no touchy")]
    [SerializeField] private MonoBehaviour[] scripts = null;
    [Tooltip("Programmer stuff, no touchy")]
    [SerializeField] private GameObject[] menuObjects = null;
    [SerializeField] private OptionAssignments optionAssignments = new OptionAssignments();
    [SerializeField] private KeyBindAsignments keybindAssignments = new KeyBindAsignments();
    [SerializeField] private KeybindTexts keybindTexts = new KeybindTexts();

    private bool paused = false;
    private int menuStage = 0; //nothing, pause, options, exitConfirm
    //private MovementController mc = null;
    private SerializeController sc = null; //use unityActions instead
    private TutorialController tc = null;

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
        //UpdatePause(false);
        //UpdateUi(0);
        updateKeysFunction.Invoke(keybindAssignments);
        tc = GetComponent<TutorialController>();
        sc = FindObjectOfType<SerializeController>();
    }

    //Get Sliders for serializing
    public OptionAssignments GetOptions ()
    {
        return optionAssignments;
    }

    public KeyBindAsignments GetKeybindings ()
    {
        return keybindAssignments;
    }

    public void SetKeyBindings (int index, KeyCode key)
    {
        switch (index)
        {
            case 0: //moveForward
                keybindAssignments.moveForward = key;
                keybindTexts.forwardText.text = key.ToString();
                break;
            case 1: //moveBack
                keybindAssignments.moveBack = key;
                keybindTexts.backText.text = key.ToString();
                break;
            case 2: //moveRight
                keybindAssignments.moveRight = key;
                keybindTexts.rightText.text = key.ToString();
                break;
            case 3: //moveLeft
                keybindAssignments.moveLeft = key;
                keybindTexts.leftText.text = key.ToString();
                break;
            case 4: //dash
                keybindAssignments.dash = key;
                keybindTexts.dashText.text = key.ToString();
                break;
            case 5: //pause
                keybindAssignments.pause = key;
                keybindTexts.pauseText.text = key.ToString();
                break;
            case 6: //jump
                keybindAssignments.jump = key;
                keybindTexts.jumpText.text = key.ToString();
                break;
            case 7: //melee
                keybindAssignments.melee = key;
                keybindTexts.meleeText.text = key.ToString();
                break;
            case 8: //action
                keybindAssignments.action = key;
                keybindTexts.actionText.text = key.ToString();
                break;
            case 9: //status
                keybindAssignments.status = key;
                keybindTexts.statusText.text = key.ToString();
                break;
            default:
                break;
        }
    }

    public bool GetPaused ()
    {
        return paused;
    }

    private void Update ()
    {
        if(Input.GetKeyDown(keybindAssignments.pause) && !paused) //change to escape later just for testing
        {
            UpdatePause(true);
            UpdateUi(1);
        }

        //change keys for shift
        if(changingKey)
        {
            //hardcode shift
            if (Input.GetKeyDown(KeyCode.LeftShift) && !keydown)
            {
                keydown = true;
                changingKeycode = KeyCode.LeftShift;

                SetKeycode(changingKeyIndex, changingKeycode);
            }

            if (Input.GetKeyDown(KeyCode.RightShift) && !keydown)
            {
                keydown = true;
                changingKeycode = KeyCode.RightShift;

                SetKeycode(changingKeyIndex, changingKeycode);
            }
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
            if(i == 3)
            {
                MeleeController mc = FindObjectOfType<MeleeController>();

                if (mc.inHit && !paused)
                {
                    continue;
                }
                else
                {
                    scripts[i].enabled = !paused;
                }
            }
            else
            {
                scripts[i].enabled = !paused;
            }
        }

        /*if(paused)
        {
            UpdateUi(1);
        }
        else
        {
            UpdateUi(0);
        }*/

        Time.timeScale = paused ? 0.0f : 1.0f; //maybe not
        LockCursor(paused);
    }
    #endregion

    #region keybinds

    //setting & changine keycode
    private void OnGUI()
    {
        if (changingKey)
        {
            //all input except shift & alt
            Event e = Event.current;

            if (e.isKey && !keydown)
            {
                keydown = true;
                changingKeycode = e.keyCode;

                SetKeycode(changingKeyIndex, changingKeycode);
            }

            if (e.type.Equals(EventType.KeyUp))
            {
                keydown = false;
            }
        }
    }

    private void SetKeycode(int keyIndex, KeyCode key)
    {
        //Debug.Log("Change " + keyIndex + " to " + key.ToString());
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
            case 8: //action
                keybindAssignments.action = key;
                break;
            case 9: //status
                keybindAssignments.status = key;
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

    //keybindings
    //optimize this break out to one extra func that does the repeating stuff
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

    public void ChangeButton_Action (TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 8;
        }
    }

    public void ChangeButton_Status(TextMeshProUGUI text)
    {
        if (!changingKey)
        {
            LockCursor(false);

            changingKeyText = text;
            changingKeyText.text = "None";

            changingKey = true;
            changingKeyIndex = 9;
        }
    }
    #endregion

    #region button calls / options
    //Tutorial
    public void ButtonTutorialQuestion ()
    {
        UpdatePause(true);
        UpdateUi(5);
    }

    public void ButtonTutorialYes ()
    {
        tc.TriggerTutorial();
        UpdatePause(false);
        FindObjectOfType<UIController>().StartCoroutine(FindObjectOfType<UIController>().WhiteFade(false, 0.5f));
        UpdateUi(0);
    }

    public void ButtonTutorialNo ()
    {
        UpdatePause(false);
        UpdateUi(0);
        FindObjectOfType<UIController>().StartCoroutine(FindObjectOfType<UIController>().WhiteFade(false, 0.5f));
        tc.TriggerNoTutorial();
    }

    //main pause
    public void ButtonYes () //exit confirm
    {
        SceneManager.LoadScene(0);
    }

    public void ButtonNo () //exit confirm
    {
        UpdateUi(1);
    }

    public void ButtonResume ()
    {
        UpdatePause(false);
        UpdateUi(0);
    }

    public void ButtonMenu()
    {
        UpdateUi(3);
        //Application.Quit();
    }

    public void ButtonCredits ()
    {
        UpdateUi(4);
    }
    
    public void ButtonOptions ()
    {
        UpdateUi(2);
    }

    public void ButtonBack ()
    {
        UpdateUi(1);
        //Debug.Log("SAVE");
        sc.SavePreferences();
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
        FMOD.Studio.VCA Master = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        Master.setVolume(slider.value * 0.01f);
        optionAssignments.soundValue.text = slider.value.ToString("F1") + "%";
    }

    public void ChangeMusic(Slider slider)
    {
        //Do music change here
        FMOD.Studio.VCA Music = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        Music.setVolume(slider.value * 0.01f);
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

    public void ChangeGraphics(Slider slider)
    {
        QualitySettings.SetQualityLevel((int)slider.value, true);

        switch ((int)slider.value)
        {
            case 0:
                optionAssignments.graphicsValue.text = "Low";
                break;
            case 1:
                optionAssignments.graphicsValue.text = "Medium";
                break;
            case 2:
                optionAssignments.graphicsValue.text = "High";
                break;
            default:
                Debug.LogWarning("This should not happen");
                break;
        }
    }

    public void ChangeInvertedControls ()
    {
        mc.SetInvertedControls();
    }
    #endregion
}