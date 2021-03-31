using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///             V 1.0
///             Made by Carl in Carl branch lmao
///             2021-03-31
/// </summary>
public class MovementController : MonoBehaviour
{
    //Maybe add running, crouching, jumping?
    [Header("Movement: ")]
    [SerializeField] private float defaultSpeed = 0f;

    [Header("Camera: ")]
    [SerializeField] private float minX = -90f;
    [SerializeField] private float maxX = 90f;
    [SerializeField] private float mouseSensitivity = 5f;

    private CharacterController cc = null;

    private float speed = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        speed = defaultSpeed;
    }

    private void Update ()
    {
        Movement();
        CameraRotation();
    }

    //main camera movement function
    private void CameraRotation ()
    {
        //add input from mouseX and mouseY axis to variables
        mouseY += Input.GetAxis("Mouse X") * (mouseSensitivity * 0.1f);
        mouseX += Input.GetAxis("Mouse Y") * (mouseSensitivity * 0.1f);

        //Make sure to clamp ROTATION around mouseX to restrict looking up
        //apply rotation to camera AND player to move in the direction player is looking
        mouseX = Mathf.Clamp(mouseX, minX, maxX);
        Camera.main.transform.eulerAngles = new Vector3(-mouseX, mouseY, 0);
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }

    //main movement function
    private void Movement ()
    {
        //read movement direction from axis
        var horizontal = Input.GetAxis("Horizontal") * speed;
        var vertical = Input.GetAxis("Vertical") * speed;

        //add axis movement to correct direction
        Vector3 forwardMovement = transform.forward * vertical;
        Vector3 sideMovement = transform.right * horizontal;
        Vector3 upMovement = Vector3.zero; //gravity and later perhaps a jump?

        upMovement.y -= 9.81f; //gravity lol

        //apply movement on character controller
        Vector3 dir = forwardMovement  + sideMovement + upMovement;
        cc.Move(dir * Time.deltaTime);
    }
}
