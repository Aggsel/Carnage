using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InteractionController : MonoBehaviour
{
    [Tooltip("Which layermasks the objects need to be able to be interacted with by the player")]
    [SerializeField] private LayerMask interactionLayermask = 0;
    //[SerializeField] private GameObject interactionObj = null;
    [SerializeField] private TextMeshProUGUI interactText = null;

    private KeyCode interactionKey = KeyCode.E;
    private bool interact = false;
    private UIController uc = null;
    private MovementController mc = null;
    private PauseController pc = null;

    //hardcoded im so sorry
    private int interactLayer = 11;

    private void Start ()
    {
        uc = FindObjectOfType<UIController>();
        mc = FindObjectOfType<MovementController>();
        pc = FindObjectOfType<PauseController>();
    }

    private void ReadKeybinds(KeyBindAsignments keys)
    {
        interactionKey = keys.action;
    }

    private void Awake()
    {
        PauseController.updateKeysFunction += ReadKeybinds;
    }

    private void OnDestroy()
    {
        PauseController.updateKeysFunction -= ReadKeybinds;
    }

    private void SetInteractObj (bool yes, string text)
    {
        interact = yes;
        interactText.text = text;
    }

    private IEnumerator InteractionDelay ()
    {
        mc.enabled = false;
        pc.enabled = false;
        uc.StartCoroutine(uc.WhiteFade(true, 1f));
        AudioManager.Instance.StopSound(ref AudioManager.Instance.hubMusic);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Level1");
    }

    private void LateUpdate ()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, 3.0f, interactionLayermask))
        {
            if ((interactionLayermask.value & (1 << interactLayer)) > 0)
            { 
                if (!interact)
                {
                    SetInteractObj(true, "Press " + interactionKey + " to interact");
                }

                if (Input.GetKeyDown(interactionKey))
                {
                    StartCoroutine(InteractionDelay());
                }
            }
        }
        else
        {
            SetInteractObj(false, "");
        }

    }
}