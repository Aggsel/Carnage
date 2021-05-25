using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MovementController : MonoBehaviour
{
    //Variable structs good for organization in the editor
    #region Variable structs
    [Serializable]
    public struct MovementVariables
    {
        [Tooltip("If always run is off, the run speed of the player")]
        public float runSpeed;
        [Tooltip("The speed of which backwards movement is multiplied by from forward")]
        public float backSpeedMultiplier;
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
        [Tooltip("Camera field of view when dashing")]
        public float dashFov;
    }
    #endregion

    [Tooltip("Dont touch")]
    [SerializeField] private Transform cam = null;
    [Tooltip("Dont touch")]
    [SerializeField] private LayerMask wallLayermask = 0;
    [Tooltip("Dont touch")]
    [SerializeField] private VolumeProfile profile = null;
    [Tooltip("Dont touch")]
    [SerializeField] private Camera fovCamera = null;

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
    private float fallForce = 1.5f;
    public float charge = 3.0f;
    private Vector3 upMovement = Vector3.zero;
    private bool invertedControls = false;
    private float groundedTimer = 0.0f;
    private float vertical = 0.0f;
    private float horizontal = 0.0f;
    private Vector3 dir = Vector3.zero;
    private float edgeForce = 3.0f;
    private MotionBlur globalMotion = null;
    private float startFov = 0.0f;
    private float endFov = 0.0f;
    private float half = 0.0f;
    private CapsuleCollider cap = null;
    private Rigidbody rb = null;
    private AudioManager am = null;
    private bool hasLanded = false;

    private Vector3 spawnPoint = Vector3.zero;
    private bool spawnSet = false;

    private void Start ()
    {
        am = AudioManager.Instance;
        cc = GetComponent<CharacterController>();
        cap = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
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
        CheckPlayerPos(); //reseting player if outside map
        Movement();
        Dash();
        CameraRotation();

        //dash recharge
        if (charge < 3.0f)
        {
            Recharge();
        }
    }

    public void SetSpawnPoint(Vector3 pos) //for respawning if falling of the world
    {
        //Debug.Log("SpawnPos is: " + pos);
        spawnPoint = pos;
        spawnSet = true;
    }

    private void CheckPlayerPos ()
    {
        if(spawnSet)
        {
            if (transform.position.y < -5f)
            {
                Debug.LogWarning("Player had an unusial y-pos and is teleported back to spawn");
                cc.enabled = false;
                transform.position = spawnPoint;
                cc.enabled = true;
            }
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

        //read hdrp profile if null add it
        if (!profile.TryGet<MotionBlur>(out var motion))
        {
            Debug.LogWarning("THIS SHOULD NOT HAPPEN");
            motion = profile.Add<MotionBlur>(false);
        }

        #region Dash from input
        //dash
        if (Input.GetKeyDown(dash) && Time.time > nextDash && charge >= 1.0f && newDir.sqrMagnitude > 0.1f)
        {
            nextDash = Time.time + dashVar.dashRate;

            //effects: fov & motionblur
            motion.active = true;
            globalMotion = motion;

            //audio
            am.PlaySound(am.playerDash);

            //dash stuff
            if (!Physics.Raycast(ray, out hit, (dashVar.dashLength / 2)))
            {
                Vector3 pos = transform.position + (ray.direction * (dashVar.dashLength / 2));
                half = Vector3.Distance(transform.position, pos) * 0.5f;
                //half = Vector3.SqrMagnitude(transform.position - pos) * 0.5f;
                startFov = fovCamera.fieldOfView;
                endFov = fovCamera.fieldOfView + dashVar.dashFov;

                positioningList.Add(pos);
            }
            else
            {
                //if ray hit something, block dash. Also make sure no clipping occur using player radius
                Vector3 pos = new Vector3(hit.point.x - newDir.x * cc.radius, transform.position.y, hit.point.z - newDir.z * cc.radius);
                half = Vector3.Distance(transform.position, pos) * 0.5f;
                //half = Vector3.SqrMagnitude(transform.position - pos) * 0.5f;
                startFov = fovCamera.fieldOfView;
                endFov = fovCamera.fieldOfView + dashVar.dashFov;

                positioningList.Add(pos);
            }

            charge -= 1.0f;
            lastPress = Time.time;
        }
        #endregion
    }
    
    //in addition to normal movement i have an AdditionalPositioning function -
    //for teleporting or doing other stuff than moving in the normal translating (Movement) function
    private void AdditionalPositioning(Vector3 pos, MotionBlur motion)
    {
        float step = dashVar.dashSpeed * Time.deltaTime;
        float dist = Vector3.Distance(transform.position, pos);//Vector3.SqrMagnitude(transform.position - pos); //optimized

        if (dist > 0.1f)
        {
            //Debug.Log(dist + ", " + half);    
            transform.position = Vector3.MoveTowards(transform.position, pos, step);

            //fov
            if(dist >= half)
            {
                fovCamera.fieldOfView = Mathf.Lerp(fovCamera.fieldOfView, endFov, step * 0.25f);
            }
            else
            {
                fovCamera.fieldOfView = Mathf.Lerp(fovCamera.fieldOfView, startFov, step * 0.5f);
            }
        }
        else
        {
            //effects
            motion.active = false;
            positioningList.Remove(positioningList[0]);
            half = 0.0f;
            fovCamera.fieldOfView = startFov;
            cap.enabled = false;
            cc.enabled = true;
            rb.isKinematic = true;
        }
    }

    //main camera movement function
    private void CameraRotation()
    {
        //add input from mouseX and mouseY axis to variables
        mouseY += Input.GetAxis("Mouse X") * (cameraVar.mouseSensitivity * 0.1f);

        if (!invertedControls)
        {
            mouseX += Input.GetAxis("Mouse Y") * (cameraVar.mouseSensitivity * 0.1f);
        }
        else
        {
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
            rb.isKinematic = false;
            cap.enabled = true;
            verticalVelocity = 0.0f; //reset upforce
            AdditionalPositioning(positioningList[0], globalMotion);
            return;
        }

        //set speed
        //speed = movementVar.alwaysRun ? movementVar.runSpeed : (Input.GetKey(KeyCode.LeftShift) ? movementVar.runSpeed : movementVar.defaultSpeed);
        
        //new movement
        if(Input.GetKey(moveForward) || Input.GetKey(moveBack))
        {
            if (Input.GetKey(moveForward))
            {
                vertical = Mathf.Clamp(vertical + 1, -1.0f, 1.0f);
            }

            if (Input.GetKey(moveBack))
            {
                vertical = Mathf.Clamp(vertical - 1, -1.0f, 1.0f) * movementVar.backSpeedMultiplier;
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
            hasLanded = false;
            am.PlaySound(am.playerJump);
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

            #region wallriding glitch fix #2
            /*RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            Debug.DrawRay(transform.position, Vector3.down * 2.0f, Color.cyan);

            if(Physics.Raycast(ray, out hit, 2.0f, wallLayermask))
            {
                dot = Vector3.Dot(hit.normal, Vector3.up);

                if (dot < 1.0f)
                {
                    
                }
            }*/
            #endregion

            //main movement
            cc.Move(dir * Time.deltaTime);

            //slope jitter fix
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out hit, 2.0f, wallLayermask))
            {
                //land sound
                if(!cc.isGrounded && verticalVelocity < 0.0f && !hasLanded)
                {
                    hasLanded = true;
                    am.PlaySound(am.playerLand);
                    //Debug.Log("Land");
                }

                //jitter fix
                if(hit.normal != Vector3.up)
                {
                    //HACK
                    if(dir.x != 0 || dir.z != 0)
                    {
                        cc.Move(((dir / 2) + (Vector3.down * edgeForce)) * Time.deltaTime);
                    }
                }
            }

            //stuck in roof fix
            RaycastHit hit2;
            Vector3 poss = transform.position + new Vector3(0, cc.height * 0.5f, 0);
            Ray ray2 = new Ray(poss, Vector3.up);
            //Debug.DrawRay(poss, Vector3.up * 0.25f, Color.red);

            if (Physics.Raycast(ray2, out hit2, 0.35f, wallLayermask))
            {
                upMovement = Vector3.zero;
                verticalVelocity = 0.0f;
                dir = new Vector3(dir.x, -fallForce * 9.81f, dir.z);
            }
        }
    }

    public float Charge
    {
        get
        {
            return charge;
        }
    }
}