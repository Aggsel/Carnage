using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        [Tooltip("Additional time or leeway the player has to jump after stepping of a platform")]
        public float jumpLeeway;
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
        [Tooltip("How fast the dash charges increases / recharges")]
        public float dashRechargeRate;
    }
    #endregion

    [Tooltip("Dont touch")]
    [SerializeField] private Transform cam = null;
    [Tooltip("Dont touch")]
    [SerializeField] private LayerMask wallLayermask = 0;

    [SerializeField] private MovementVariables movementVar = new MovementVariables();
    [SerializeField] private CameraVariables cameraVar = new CameraVariables();
    [SerializeField] private DashVariables dashVar = new DashVariables();

    [HideInInspector]
    public CharacterController cc = null; //change to get function

    //keys
    private KeyCode moveForward;
    private KeyCode moveBack;
    private KeyCode moveRight;
    private KeyCode moveLeft;
    private KeyCode dash;
    private KeyCode jump;

    //private
    private float speed = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float verticalVelocity = 0f;
    private float lastPress = 0f;
    private float nextDash = 0f;
    private List<Vector3> positioningList = new List<Vector3>();
    private Vector3 dir = Vector3.zero;
    private float fallForce = 1.5f;
    private float charge = 3.0f;
    private Vector3 upMovement = Vector3.zero;
    private bool invertedControls = false;
    private float groundedTimer = 0.0f;
    private float vertical = 0.0f;
    private float horizontal = 0.0f;

    private void Start ()
    {
        cc = GetComponent<CharacterController>();
        speed = movementVar.runSpeed;
    }

    #region options related
    //Options
    public void SetSensitivity (float value)
    {
        cameraVar.mouseSensitivity = value;
    }

    public float GetSensitivity ()
    {
        return cameraVar.mouseSensitivity;
    }

    public void SetInvertedControls ()
    {
        invertedControls = !invertedControls;
    }

    //update keybinds
    private void ReadKeybinds (KeyBindAsignments keys)
    {
        moveForward = keys.moveForward;
        moveBack = keys.moveBack;
        moveRight = keys.moveRight;
        moveLeft = keys.moveLeft;
        dash = keys.dash;
        jump = keys.jump;
    }

    private void Awake ()
    {
        PauseController.updateKeysFunction += ReadKeybinds;
    }

    private void OnDestroy ()
    {
        PauseController.updateKeysFunction -= ReadKeybinds;
    }
    #endregion

    //script
    private void Update ()
    {
        Movement();
        Dash();
        CameraRotation();

        //dash recharge
        if (charge < 3.0f)
        {
            Recharge();
        }
    }

    private void Recharge ()
    {
        //put visual here

        charge += (dashVar.dashRechargeRate * 0.1f) * Time.deltaTime;
        charge = Mathf.Clamp(charge, 0.0f, 3.0f);
    }

    private void Dash ()
    {
        //test dashing with raycasting
        RaycastHit hit;
        Vector3 newDir = new Vector3(dir.x, 0, dir.z).normalized;
        Ray ray = new Ray(transform.position, newDir);

        //Debug.DrawRay(ray.origin, ray.direction * (dashVar.dashLength / 2), Color.green);

        #region Dash from input
        //dash
        if (Input.GetKeyDown(dash) && Time.time > nextDash && charge >= 1.0f && newDir.sqrMagnitude > 0.1f)
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

            charge -= 1.0f;
            lastPress = Time.time;
        }
        #endregion
    }
    
    //in addition to normal movement i have an AdditionalPositioning function -
    //for teleporting or doing other stuff than moving in the normal translating (Movement) function
    private void AdditionalPositioning(Vector3 pos)
    {
        float step = dashVar.dashSpeed * Time.deltaTime;
        float dist = Vector3.SqrMagnitude(transform.position - pos); //optimized
        //float dist = Vector3.Distance(transform.position, pos);

        if (dist > 0.1f)
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
        if(!invertedControls)
        {
            mouseY += Input.GetAxis("Mouse X") * (cameraVar.mouseSensitivity * 0.1f);
            mouseX += Input.GetAxis("Mouse Y") * (cameraVar.mouseSensitivity * 0.1f);
        }
        else
        {
            mouseY -= Input.GetAxis("Mouse X") * (cameraVar.mouseSensitivity * 0.1f);
            mouseX -= Input.GetAxis("Mouse Y") * (cameraVar.mouseSensitivity * 0.1f);
        }

        //Make sure to clamp ROTATION around mouseX to restrict looking up
        //apply rotation to camera AND player to move in the direction player is looking
        mouseX = Mathf.Clamp(mouseX, cameraVar.minX, cameraVar.maxX);
        cam.eulerAngles = new Vector3(-mouseX, mouseY, 0);
        transform.eulerAngles = new Vector3(0, cam.eulerAngles.y, 0);
    }

    //main movement function
    private void Movement ()
    {
        //in addition to cc.Move, we add additional transformation after it, good for teleportation
        if (positioningList.Count != 0)
        {
            if (positioningList.Count > 1)
            {
                Debug.LogError("Something is wrong, there should not be more than one");
            }

            cc.enabled = false;
            verticalVelocity = 0.0f; //reset upforce
            AdditionalPositioning(positioningList[0]);
            return;
        }

        //set speed
        speed = movementVar.alwaysRun ? movementVar.runSpeed : (Input.GetKey(KeyCode.LeftShift) ? movementVar.runSpeed : movementVar.defaultSpeed);

        //read movement direction from axis
        //var horizontal = Input.GetAxis("Horizontal") * speed;
        //var vertical = Input.GetAxis("Vertical") * speed;

        //new movement
        if(Input.GetKey(moveForward) || Input.GetKey(moveBack))
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

        //add axis movement to correct direction
        Vector3 forwardMovement = transform.forward * vertical * speed;
        Vector3 sideMovement = transform.right * horizontal * speed;
        upMovement = transform.up * verticalVelocity;

        #region jumping
        //jump leeway
        if(cc.isGrounded && groundedTimer < movementVar.jumpLeeway)
        {
            groundedTimer += Time.deltaTime;
        }

        if(!cc.isGrounded && groundedTimer > 0.0f)
        {
            groundedTimer -= Time.deltaTime;
        }

        //jump key
        if (Input.GetKeyDown(jump) && groundedTimer > 0.0f)
        {
            verticalVelocity = movementVar.jumpForce;
        }

        //fall down again
        if (verticalVelocity >= 0)
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

        //apply movement on character controller
        dir = forwardMovement + sideMovement;

        if (cc.enabled)
        {
            dir = Vector3.ClampMagnitude(dir, speed);
            dir += upMovement;

            //fix wallriding glitch
            RaycastHit hit;
            Vector3 newDir = new Vector3(dir.x, 0, dir.z).normalized;
            Ray ray = new Ray(transform.position, newDir);

            Debug.DrawRay(ray.origin, ray.direction * 1.0f, Color.green);

            if (Physics.Raycast(ray, out hit, 1.0f, wallLayermask))
            {
                if(dir.z > 0.0f || dir.z < 0.0f)
                {
                    //Debug.Log("Stop wallgrinding bitch on z");
                    dir = new Vector3(dir.x, dir.y, 0.0f);
                }
                
                if(dir.x > 0.0f || dir.x < 0.0f)
                {
                    //Debug.Log("Stop wallgrinding bitch on x");
                    dir = new Vector3(0.0f, dir.y, dir.z);
                }
            }

            cc.Move(dir * Time.deltaTime);
        }
    }

    //DEBUG velocity & FPS
    private void OnGUI ()
    {
        GUI.Label(new Rect(10, 30, 100, 50), charge.ToString("F2"));
        GUI.Label(new Rect(10, 10, 100, 50), cc.velocity.magnitude.ToString());
        GUI.Label(new Rect(Screen.width - 40, 10, 70, 50), (1.0f / Time.smoothDeltaTime).ToString("F2"));
    }
}
