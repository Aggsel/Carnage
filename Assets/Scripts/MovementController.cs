using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///             V 3.0
///             Made by Carl in Carl branch lmao
///             2021-03-31
/// </summary>
public class MovementController : MonoBehaviour
{
    //Maybe add running, crouching, jumping?
    [Header("Movement: ")]
    [SerializeField] private bool alwaysRun = false;
    [SerializeField] private float defaultSpeed = 0f;
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float jumpForce = 0f;

    [Header("Camera: ")]
    [SerializeField] private float minX = -90f;
    [SerializeField] private float maxX = 90f;
    [SerializeField] private float mouseSensitivity = 5f;

    [Header("Dash: ")]
    [SerializeField] private float dashLength = 5f;
    [SerializeField] private float dashRate = 2f;

    private CharacterController cc = null;

    private float speed = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float verticalVelocity = 0f;
    private float lastPress = 0f;
    private float nextDash = 0f;

    //TEST
    private List<Vector3> positioningList = new List<Vector3>();
    private Vector3 dir = Vector3.zero;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        speed = runSpeed;
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
    private void Dash ()
    {
        //test dashing with raycasting
        RaycastHit hit;
        Ray ray = new Ray(transform.position, dir.normalized);

        Debug.DrawRay(ray.origin, ray.direction * dashLength, Color.green);

        #region C-Dash
        //dash with C
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            //nothing in the way of the dash
            if (!Physics.Raycast(forwardRay, out hit, dashLength))
            {
                positioningList.Add(transform.position + (Camera.main.transform.forward * dashLength));
            }
            else
            {
                positioningList.Add(new Vector3(hit.point.x - cc.radius, hit.point.y + cc.height, hit.point.z - cc.radius));
            }
        }*/
        #endregion

        //dash with dubblepress of SHIFT
        if (Time.time > nextDash)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                float timeLastPress = Time.time - lastPress;

                if (timeLastPress <= 0.5f)
                {
                    nextDash = Time.time + dashRate;

                    if (!Physics.Raycast(ray, out hit, dashLength))
                    {
                        positioningList.Add(transform.position + (ray.direction * dashLength));
                    }
                    else
                    {
                        //TODO: we can always fix the instant dash by using smoothmovement instead
                        //TODO: here we have to check the rotation of the raycast whether we are looking up or down
                        positioningList.Add(new Vector3(hit.point.x - cc.radius, transform.position.y, hit.point.z - cc.radius));
                    }
                }
                
                lastPress = Time.time;
            }
        }
    }

    private void AdditionalPositioning (Vector3 pos)
    {
        transform.position = pos;
        positioningList.Remove(positioningList[0]);
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
        //set speed
        speed = alwaysRun ? runSpeed : Input.GetKey(KeyCode.LeftShift) ? runSpeed : defaultSpeed;

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

        //in addition to cc.Move, we add additional transformation after it, good for teleportation
        if (positioningList.Count != 0)
        {
            if (positioningList.Count > 1)
            {
                Debug.LogError("Something is wrong, there should not be more than one");
            }

            cc.enabled = false;
            AdditionalPositioning(positioningList[0]);
            cc.enabled = true;
        }

        //apply movement on character controller
        dir = forwardMovement + sideMovement + upMovement;

        if (cc.enabled)
        {
            cc.Move(dir * Time.deltaTime);
        }
    }
}
