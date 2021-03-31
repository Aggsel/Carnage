using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///             V 2.0
///             Made by Carl in Carl branch lmao
///             2021-03-31
/// </summary>
public class MovementController : MonoBehaviour
{
    //Maybe add running, crouching, jumping?
    [Header("Movement: ")]
    [SerializeField] private float defaultSpeed = 0f;
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float jumpForce = 0f;

    [Header("Camera: ")]
    [SerializeField] private float minX = -90f;
    [SerializeField] private float maxX = 90f;
    [SerializeField] private float mouseSensitivity = 5f;

    [Header("Dash: ")]
    [SerializeField] private float dashLength = 5f;

    private CharacterController cc = null;

    private float speed = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float verticalVelocity = 0f;
    private float lastPress = 0;

    //TEST
    private Vector3 dashMovement = Vector3.zero;
    private Vector3 desiredDashPos = Vector3.zero;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        speed = defaultSpeed;
    }

    private void Update ()
    {
        Dash();
        Movement();
        CameraRotation();
    }

    /// <summary>
    /// The problem with using character controller is that it overwrites any other transformation
    /// This means that we cant just set he player position to something.
    /// Maybe a sign to use vectors instead
    /// </summary>
    #region Dash-Testing
    private void Dash ()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            float timeLastPress = Time.time - lastPress;

            if (timeLastPress <= 0.5f)
            {
                //test dashing with raycasting
                RaycastHit hit;
                Ray forwardRay = new Ray(transform.position, transform.forward);

                Debug.DrawRay(transform.position, transform.forward * dashLength, Color.red);

                if (!Physics.Raycast(forwardRay, out hit, dashLength))
                {
                    //this doesn't work as the position is entierly controlled by the charactercontroller
                    transform.position += transform.forward * dashLength;
                }
            }

            lastPress = Time.time;
        }
    }
    #endregion

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
        //set speed
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : defaultSpeed;

        //read movement direction from axis
        var horizontal = Input.GetAxis("Horizontal") * speed;
        var vertical = Input.GetAxis("Vertical") * speed;

        //add axis movement to correct direction
        Vector3 forwardMovement = transform.forward * vertical;
        Vector3 sideMovement = transform.right * horizontal;
        Vector3 upMovement = transform.up * verticalVelocity;

        //jump
        if(Input.GetKeyDown(KeyCode.Space) && cc.isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        //decrease verticalVel to fall down again
        if(verticalVelocity > 0)
        {
            verticalVelocity -= 10 * Time.deltaTime;
        }

        upMovement.y -= 9.81f; //gravity lol

        //dash clamp and reduction
        dashMovement.x = Mathf.Clamp(dashMovement.x, 0, 50);
        dashMovement.y = Mathf.Clamp(dashMovement.y, 0, 50);

        //apply movement on character controller
        Vector3 dir = forwardMovement  + sideMovement + upMovement + dashMovement;
        cc.Move(dir * Time.deltaTime);
    }
}
