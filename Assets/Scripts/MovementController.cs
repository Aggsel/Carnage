using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///             V 5.0
///             Made by Carl in Carl branch lmao
///             2021-04-02
/// </summary>
public class MovementController : MonoBehaviour
{
    //Variable structs good for organization in the editor
    #region Variable structs
    [Serializable]
    public struct MovementVariables
    {
        [Tooltip("Should the player always run or run with the run key")]
        public bool alwaysRun;
        [Tooltip("If always run is off, the default walk speed of the player")]
        public float defaultSpeed;
        [Tooltip("If always run is off, the run speed of the player")]
        public float runSpeed;
        [Tooltip("The jumpheight of the player")]
        public float jumpForce;
        [Tooltip("The acceleration of the players speed when falling")]
        public float fallAcceleration;
    }

    [Serializable]
    public struct CameraVariables
    {
        [Tooltip("Max x value that the camera can rotate (looking up)")]
        public float minX;
        [Tooltip("Min x value that the camera can rotate (looking down)")]
        public float maxX;
        [Tooltip("The speed of which the camera is moved")]
        public float mouseSensitivity;
    }

    [Serializable]
    public struct DashVariables
    {
        [Tooltip("Dude, the length of the dash..")]
        public float dashLength;
        [Tooltip("How fast the dash is")]
        public float dashSpeed;
        [Tooltip("Cooldown of the dash, higher number means a longer wait between dashes")]
        public float dashRate;
    }
    #endregion

    [SerializeField] private MovementVariables movementVar = new MovementVariables();
    [SerializeField] private CameraVariables cameraVar = new CameraVariables();
    [SerializeField] private DashVariables dashVar = new DashVariables();

    private CharacterController cc = null;

    private float speed = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float verticalVelocity = 0f;
    private float lastPress = 0f;
    private float nextDash = 0f;
    private List<Vector3> positioningList = new List<Vector3>();
    private Vector3 dir = Vector3.zero;
    private float fallForce = 1.5f;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        speed = movementVar.runSpeed;

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

        Debug.DrawRay(ray.origin, ray.direction * (dashVar.dashLength / 2), Color.green);

        #region RightClick-Dash (for testing only)
        //dash with RightClick
        if (Input.GetMouseButtonDown(1) && Time.time > nextDash)
        {
            nextDash = Time.time + dashVar.dashRate;

            if (!Physics.Raycast(ray, out hit, (dashVar.dashLength / 2)))
            {
                positioningList.Add(transform.position + (ray.direction * (dashVar.dashLength / 2)));
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
        float step = dashVar.dashSpeed * Time.deltaTime;
        float dist = Vector3.Distance(transform.position, pos);

        if (dist > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, step);
        }
        else
        {
            positioningList.Remove(positioningList[0]);
            cc.enabled = true;
        }
    }

    //main camera movement function
    private void CameraRotation()
    {
        //add input from mouseX and mouseY axis to variables
        mouseY += Input.GetAxis("Mouse X") * (cameraVar.mouseSensitivity * 0.1f);
        mouseX += Input.GetAxis("Mouse Y") * (cameraVar.mouseSensitivity * 0.1f);

        //Make sure to clamp ROTATION around mouseX to restrict looking up
        //apply rotation to camera AND player to move in the direction player is looking
        mouseX = Mathf.Clamp(mouseX, cameraVar.minX, cameraVar.maxX);
        Camera.main.transform.eulerAngles = new Vector3(-mouseX, mouseY, 0);
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }

    //main movement function
    private void Movement ()
    {
        //set speed
        //if alwaysRun == true, speed = runSpeed if not do whats right of the colon
        speed = movementVar.alwaysRun ? movementVar.runSpeed : (Input.GetKey(KeyCode.LeftShift) ? movementVar.runSpeed : movementVar.defaultSpeed);

        //read movement direction from axis
        var horizontal = Input.GetAxis("Horizontal") * speed;
        var vertical = Input.GetAxis("Vertical") * speed;

        //add axis movement to correct direction
        Vector3 forwardMovement = transform.forward * vertical;
        Vector3 sideMovement = transform.right * horizontal;
        Vector3 upMovement = transform.up * verticalVelocity;

        #region jumping
        //jump key
        if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded)
        {
            verticalVelocity = movementVar.jumpForce;
        }

        //fall down again
        if (verticalVelocity > 0)
        {
            verticalVelocity -= 25 * Time.deltaTime;
        }

        if (fallForce > 0 && cc.isGrounded)
        {
            fallForce = 0.1f;
        }

        if(verticalVelocity < 0)
        {
            fallForce += (Mathf.Pow(movementVar.fallAcceleration, 2f) * Time.deltaTime);
        }

        upMovement.y -= (fallForce * 9.81f);
        #endregion

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
        dir = forwardMovement + sideMovement;

        if (cc.enabled)
        {
            dir = Vector3.ClampMagnitude(dir, speed);
            dir += upMovement;
            cc.Move(dir * Time.deltaTime);
        }
    }

    //DEBUG velocity & FPS
    private void OnGUI ()
    {
        //GUI.Label(new Rect(10, 30, 100, 50), fallForce.ToString());
        GUI.Label(new Rect(10, 10, 100, 50), cc.velocity.magnitude.ToString());
        GUI.Label(new Rect(Screen.width - 40, 10, 70, 50), (1.0f / Time.smoothDeltaTime).ToString("F2"));
    }
}
