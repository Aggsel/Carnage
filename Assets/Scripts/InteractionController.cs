using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionController : MonoBehaviour
{
    [Tooltip("Which layermasks the objects need to be able to be interacted with by the player")]
    [SerializeField] private LayerMask interactionLayermask = 0;

    private KeyCode interactionKey = KeyCode.E;
    private bool interact = false;
    private bool item = false;

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

    private void LateUpdate ()
    {
        
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        //Debug.DrawRay(ray.origin, ray.direction * 1f, Color.green);

        if (Physics.Raycast(ray, out hit, 6.0f, interactionLayermask))
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.name == "Itemspawn")
            {
                Debug.Log("starting at item");
                item = true;
                interact = false;
            }
            else
            {
                item = false;
                interact = true;

                if (Input.GetKeyDown(interactionKey))
                {
                    //u can use loadasync perhaps
                    //Debug.Log("Start run");
                    SceneManager.LoadScene("Level1");
                }
            }
        }
        else
        {
            interact = false;
            item = false;
        }

    }

    private void OnGUI()
    {
        if(interact)
        {
            GUI.Label(new Rect((Screen.width / 2), (Screen.height / 2), 120, 50), "Press " + interactionKey.ToString() + " to interact");
        }

        if (item)
        {
            GUI.Label(new Rect((Screen.width / 2), (Screen.height / 2), 120, 50), "Item.");
        }
    }
}