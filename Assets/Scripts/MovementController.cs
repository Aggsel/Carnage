using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///             V 4.0
///             Made by Carl in Carl branch lmao
///             2021-03-31
/// </summary>
public class MovementController : MonoBehaviour
{
    //Added tooltips for designers lol
    [Header("Movement: ")]
    [Tooltip("Should the player always run or run with the run key")]
    [SerializeField] private bool alwaysRun = false;
    [Tooltip("If always run is off, the default walk speed of the player")]
    [SerializeField] private float defaultSpeed = 0f;
    [Tooltip("If always run is off, the run speed of the player")]
    [SerializeField] private float runSpeed = 0f;
    [Tooltip("The jumpheight of the player")]
    [SerializeField] private float jumpForce = 0f;

    [Header("Camera: ")]
    [SerializeField] private float minX = -90f;
    [SerializeField] private float maxX = 90f;
    [SerializeField] private float mouseSensitivity = 5f;

    [Header("Dash: ")]
    [Tooltip("Dude, the length of the dash..")]
    [SerializeField] private float dashLength = 5f;
    [Tooltip("Cooldown of the dash, higher number means a longer wait between dashes")]
    [SerializeField] private float dashRate = 2f;

    private CharacterController cc = null;

    private float speed = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float verticalVelocity = 0f;
    private float lastPress = 0f;
    private float nextDash = 0f;
    private List<Vector3> positioningList = new List<Vector3>();
    private Vector3 dir = Vector3.zero;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        speed = runSpeed;

        //DEBUG lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update ()
    {
        Dash();
        Movement();
        CameraRotation();
    }

    private void Dash ()
    {
        //test dashing with raycasting
        RaycastHit hit;
        Vector3 newDir = new Vector3(dir.x, 0, dir.z).normalized;
        Ray ray = new Ray(transform.position, newDir);

        Debug.DrawRay(ray.origin, ray.direction * (dashLength / 2), Color.green);

        #region RightClick-Dash (for testing only)
        //dash with RightClick
        if (Input.GetMouseButtonDown(1) && Time.time > nextDash)
        {
            nextDash = Time.time + dashRate;

            if (!Physics.Raycast(ray, out hit, (dashLength / 2)))
            {
                positioningList.Add(transform.position + (ray.direction * (dashLength / 2)));
            }
            else
            {
                //if ray hit something, block dash. Also make sure no clipping occur using player radius
                positioningList.Add(new Vector3(hit.point.x - newDir.x * cc.radius, transform.position.y, hit.point.z - newDir.z * cc.radius));
            }

            lastPress = Time.time;
        }
        #endregion

        #region dubbleclickShift-Dash
        //dash with dubblepress of SHIFT
        /*if (Time.time > nextDash)
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
        }*/
        #endregion
    }
    
    //in addition to normal movement i have an AdditionalPositioning function -
    //for teleporting or doing other stuff than moving in the normal translating (Movement) function
    private void AdditionalPositioning(Vector3 pos)
    {
        float step = dashLength * Time.deltaTime;
        float dist = Vector3.Distance(transform.position, pos);

        if (dist > 1f)
        {
            Debug.Log("move");
            transform.position = Vector3.MoveTowards(transform.position, pos, step);
        }
        else
        {
            Debug.Log("stop");
            positioningList.Remove(positioningList[0]);
            cc.enabled = true;
        }
    }

    //main camera movement function
    private void CameraRotation()
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
        //if alwaysRun == true, speed = runSpeed if not do whats right of the colon
        speed = alwaysRun ? runSpeed : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : defaultSpeed);

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
            verticalVelocity -= 25f * Time.deltaTime;
        }

        /*
        //accelerated movement when falling
        float fallTime = 0f;
        
        if(verticalVelocity > 0)
        {
            fallTime += Time.deltaTime;
            verticalVelocity -= (fallTime * fallTime * 9.81f) / 2; //accelerated movement v*t + (a*t^2) / 2
            Debug.Log(verticalVelocity);
            //verticalVelocity = Mathf.Clamp(verticalVelocity, -50, 50);
        }
        else
        {
            fallTime = 0;
        }
        */

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
        }

        //apply movement on character controller
        dir = forwardMovement + sideMovement + upMovement;

        if (cc.enabled)
        {
            cc.Move(dir * Time.deltaTime);
        }
    }
}
