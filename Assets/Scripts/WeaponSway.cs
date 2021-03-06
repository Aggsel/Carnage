using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///             v 1.0
///             Made by Carl
///             2021-04-02
/// </summary>

public class WeaponSway : MonoBehaviour
{
    [Header("Movement sway: ")]
    [SerializeField] private float movementAmount = 0.2f;
    [SerializeField] private float movementSpeed = 2f;

    [Header("Rotation sway: ")]
    [SerializeField] private float rotationAmount = 0.5f;
    [SerializeField] private float rotationSpeed = 2f;

    private Vector3 startPos = Vector3.zero;
    private Vector3 desiredPos = Vector3.zero;

    private Vector3 desiredRot = Vector3.zero;
    private Vector3 startRot = Vector3.zero;

    private Vector3 right = Vector3.zero;
    private Vector3 forward = Vector3.zero;
    private Vector3 up = Vector3.zero;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private MovementController mc;
    private float vertical = 0.0f;
    private float horizontal = 0.0f;

    //keys
    private KeyCode moveForward;
    private KeyCode moveBack;
    private KeyCode moveRight;
    private KeyCode moveLeft;
    private KeyCode dash;
    private KeyCode jump;

    private void Start ()
    {
        if(GetComponentInParent<MovementController>() != null)
        {
            mc = GetComponentInParent<MovementController>();
        }
        else
        {
            Debug.LogWarning("Could not find player movementController.cs");
        }

        startPos = transform.localPosition;
        desiredPos = startPos;

        startRot = transform.localEulerAngles;
        desiredRot = startRot;
    }

    private void LateUpdate ()
    {
        MovementSway();
        CameraSway();
    }

    //weaponsway from camera rotation (changes rotation)
    private void CameraSway ()
    {
        mouseY = Input.GetAxis("Mouse X") * Mathf.Log10(mc.GetSensitivity() * 0.25f) * (rotationAmount * 0.1f);
        mouseX = Input.GetAxis("Mouse Y") * Mathf.Log10(mc.GetSensitivity() * 0.25f) * (rotationAmount * 0.1f);

        desiredRot = new Vector3(mouseX, mouseY, right.x * -100f);
        Quaternion dest = Quaternion.Euler(startRot + desiredRot);

        //old
        //float step = Mathf.Log(mc.GetSensitivity(), mc.GetSensitivity() * Mathf.Pow(1.5f, 3.0f)) * 0.5f * rotationSpeed * Time.deltaTime;
        float step = (1.0f - Mathf.Log10(mc.GetSensitivity()) * 2.0f * rotationSpeed * Time.deltaTime) * 0.1f;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, dest, step);
    }

    #region read keybind
    //update keybinds
    private void ReadKeybinds(KeyBindAsignments keys)
    {
        moveForward = keys.moveForward;
        moveBack = keys.moveBack;
        moveRight = keys.moveRight;
        moveLeft = keys.moveLeft;
        dash = keys.dash;
        jump = keys.jump;
    }

    private void Awake()
    {
        PauseController.updateKeysFunction += ReadKeybinds;
    }

    private void OnDestroy()
    {
        PauseController.updateKeysFunction -= ReadKeybinds;
    }
    #endregion

    //weaponsway from movement (changes position)
    private void MovementSway ()
    {
        //var horizontal = Input.GetAxis("Horizontal");
        //var vertical = Input.GetAxis("Vertical");

        if (Input.GetKey(moveForward) || Input.GetKey(moveBack))
        {
            if (Input.GetKey(moveForward))
            {
                vertical = Mathf.Clamp(vertical + 1, -1.0f, 1.0f);
            }

            if (Input.GetKey(moveBack))
            {
                vertical = Mathf.Clamp(vertical - 1, -1.0f, 1.0f);
            }
        }
        else
        {
            vertical = 0.0f;
        }

        if (Input.GetKey(moveRight) || Input.GetKey(moveLeft))
        {
            if (Input.GetKey(moveRight))
            {
                horizontal = Mathf.Clamp(horizontal + 1, -1.0f, 1.0f);
            }

            if (Input.GetKey(moveLeft))
            {
                horizontal = Mathf.Clamp(horizontal - 1, -1.0f, 1.0f);
            }
        }
        else
        {
            horizontal = 0.0f;
        }

        right = horizontal * Vector3.right * (movementAmount * 0.1f);
        forward = vertical * Vector3.forward * (movementAmount * 0.1f);
        up = mc.cc.velocity.y * Vector3.up * (movementAmount * 0.02f);

        desiredPos = right + forward + up;

        float step = movementSpeed * Time.deltaTime;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, desiredPos + startPos, step);
    }
}