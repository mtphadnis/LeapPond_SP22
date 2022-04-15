using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class thirdSoul : MonoBehaviour
{
    private CharacterController controller;
    private FirstPersonControls firstPersonControls;
    private Rigidbody rigidBody;
    private GameObject mainCamera;
    private runeBehavior runeBehavior;
    public AudioClip runeCast;
    public AudioSource source;
    public AudioClip catchCast;
    public AudioClip incorrectcast;
    public AudioClip grappleshoot;
    public AudioClip bounce;
    public ChangeSceneButton mousey;


    [Header("Ground Detections")]
    [Tooltip("The hieght of the sphereCheck at the players bottom")]
    public float GroundedHieght;
    [Tooltip("The radius of the sphereCheck at the players bottom")]
    public float GroundRadius;
    //stores the set radius
    float groundRadiusStored;
    [Tooltip("Layers the player can place runes on and layers that will enable the player controller")]
    public LayerMask RuneAble, GroundAble, playerLayer;
    [Tooltip("how long in seconds the player needs to be off the ground until the player is !Grounded")]
    public float offGroundTimerEnd;
    //the fluid timer that increases until offGroundTimerEnd
    public float offGroundTimer;
    //basically whether the player should be a RigidBody or CharacterController
    bool Grounded;
    public Collider[] FeetTouching;
    bool startSwitch;

    [Space(10)]
    [Header("Debug Controls")]
    [Tooltip("Vector3 position to reset too")]
    public Vector3 ResetPoint;
    [Tooltip("A Public Vector3")]
    public Vector3 PublicVector3;
    [Tooltip("cube prefab")]
    public GameObject Cube;

    [Space(10)]
    [Header("Jump Factors")]
    [Tooltip("How high the player jumps")]
    public float JumpHeight;
    bool jumping;

    [Space(10)]
    [Header("Movement Factors")]
    [Tooltip("How fast the player moves")]
    public float movementSpeed;
    [Tooltip("How fast the player moves while sprinting")]
    public float sprintSpeed;
    [Tooltip("Multiplyer for air strafe speed in the final total")]
    public float AirStrafeSpeed;

    public float AirStrafeClamp;
    [Tooltip("The rate by which all speed increases progressively")]
    public float SpeedChangeRate;
    [Tooltip("Gravity for the character controller component")]
    public float TargetGravity;
    float gravity;
    //Player vertical speed
    float vertSpeed;
    //the actual player speed
    float speed;
    //whether shift and ctrl are being held
    bool sprinting, crouching;
    Vector3 platformPositionStorage;

    [Space(10)]
    [Header("Camera/Mouse Controls")]
    [Tooltip("A multiplier affecting the rate of pitch and pan")]
    public float mouseSensitivity;
    //the live rotation of the camera and player
    float xRotation;
    [Tooltip("How many updates before a rune can be used again")]
    public float RuneRefresh;
    //the timer for RuneRefresh
    float runeTimer;
    float scrollPosition;
    int launchScroll;

    [Space(10)]
    [Header("Runes")]
    [Tooltip("How many Bounce Runes can be placed before they start reusing placed runes")]
    public float MaxBounceRunes;
    [Tooltip("How many pairs of Launch/Catch runes can be placed before they start reusing placed runes")]
    public float MaxLaunchCatchRunes;
    [Tooltip("The multiplier for the addForce on Launch triggers")]
    public Vector3 LaunchStrength;
    public List<GameObject> BounceRunes;
    public float RuneRange;

    public GameObject[] LaunchCatchStorage;
    public GameObject[] LaunchCatchTemp;
    public List<GameObject> LCRuneSets;

    public GameObject launchRunePrefab;
    public GameObject bounceRunePrefab;
    public GameObject catchRunePrefab;

    public GameObject[] LaunchIconActive;
    public GameObject[] LaunchIconsPlaced;
    public GameObject[] LaunchIconsPlacedBG;
    public Slider mousesense;


    [Space(10)]
    [Header("Grapple")]
    [Tooltip("Grappleing hook object")]
    public GameObject grapplingGun;
    float reduction;


    public bool Pause;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
        runeBehavior = GetComponent<runeBehavior>();

        firstPersonControls = new FirstPersonControls();
        firstPersonControls.Player.Enable();

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    
    private void Start()
    {
        SoulSwitch(true);
        groundRadiusStored = GroundRadius;

        for(int i = 0; i < MaxLaunchCatchRunes; i++)
        {
            LCRuneSets.Add(null);
        }

        Cursor.lockState = CursorLockMode.Locked;

        LaunchIndicatorCheck(0);
        

        mousey = GameObject.Find("SceneManager").GetComponent<ChangeSceneButton>();
    }

    private void FixedUpdate()
    {
        WhatsUnder();
        Move();
        runeTimer++;

        //Debug.Log("RigidBody: " + rigidBody.velocity + " Controller: " + controller.velocity);
        //Debug.Log("Rigid Velocity: " + rigidBody.velocity.magnitude + " Controller Velocity: " + controller.velocity.magnitude);
    }


    private void LateUpdate()
    {
        if(offGroundTimer > 0.5)
        {
            jumping = false;
            GroundRadius = groundRadiusStored;
        }
        controller.enabled = Grounded;
        rigidBody.isKinematic = Grounded;
    }

    //Will switch between physics controllers
    //Character Controller = True
    //RigidBody = False
    private void SoulSwitch(bool state)
    {
        controller.enabled = state;
        rigidBody.isKinematic = state;
    }

    //Creates a Sphere at the bottom of the player that detects collisions and the layers collided with
    //It then delays the reaction to this collision
    private void WhatsUnder()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedHieght, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundRadius, GroundAble, QueryTriggerInteraction.Ignore);

        if (Grounded && rigidBody.velocity.y == 0 && !Rising() && !jumping)
        {
            gravity = Mathf.Lerp(gravity, TargetGravity, 0.01f);
            offGroundTimer = 0;
            controller.Move(new Vector3(0, gravity, 0));
            Grounded = true;
            SoulSwitch(true);
        }
        else if(!Grounded && offGroundTimer < offGroundTimerEnd)
        {
            gravity = Mathf.Lerp(gravity, TargetGravity, 0.01f);
            offGroundTimer += Time.deltaTime;
            controller.Move(new Vector3(0, gravity, 0));
            Grounded = true;
            startSwitch = true;
        }
        else if(!Grounded && offGroundTimer >= offGroundTimerEnd)
        {
            if (startSwitch && !jumping) { rigidBody.velocity = controller.velocity; startSwitch = false; }
            gravity = 0;
            offGroundTimer += Time.deltaTime;
            Grounded = false;
            SoulSwitch(false);
        }
        
    }
    public void GrapplePhysicsStart()
    {
       rigidBody.velocity = rigidBody.velocity / 3;
       rigidBody.useGravity = false;
        
    }

    public void GrapplePhysicsEnd()
    {
        rigidBody.useGravity = true;
        Debug.Log("End");
    }

    //Debugging position setter
    public void ResetPos(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            Instantiate(Cube, transform.position, Quaternion.FromToRotation(Vector3.up, Vector3.up));
            transform.position = ResetPoint; 
            
        }
    }

    //Detects if Ctrl is being held
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.started){crouching = true;}
        else if (context.canceled){crouching = false;}
    }

    //Detects if Shift is being held
    public void Sprint(InputAction.CallbackContext context)
    {
        if(context.started){sprinting = true;}
        else if(context.canceled){sprinting = false;}
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            scrollPosition += ((context.ReadValue<Vector2>().y) / 120);
            launchScroll = (int)(Math.Abs(scrollPosition%MaxLaunchCatchRunes));

            LaunchIndicatorCheck(launchScroll);
            
            if(LCRuneSets[launchScroll] != null)
            {
                LaunchCatchTemp[0] = LCRuneSets[launchScroll];
                LaunchCatchTemp[1] = LCRuneSets[launchScroll].GetComponent<LaunchBehavior>().GetCatch();
            }
            else
            {
                LaunchCatchTemp[0] = null;
                LaunchCatchTemp[1] = null;
            }
            
        }
    }

    //if the player is rising at all
    bool Rising()
    {
        return (rigidBody.velocity.y > 0 || controller.velocity.y > 0);
    }

    //Calculates the target speed the player and then moves them in the various contexts
    private void Move()
    {
        //Checks if the player is sprinting and returns the respective value
        float targetSpeed = sprinting ? sprintSpeed : movementSpeed;

        //If there is no input the players targetSpeed is 0
        if (firstPersonControls.Player.Move.ReadValue<Vector2>() == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(firstPersonControls.Player.Move.ReadValue<Vector2>().x, 0.0f, firstPersonControls.Player.Move.ReadValue<Vector2>().y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (firstPersonControls.Player.Move.ReadValue<Vector2>() != Vector2.zero)
        {
            // move
            inputDirection = transform.right * firstPersonControls.Player.Move.ReadValue<Vector2>().x + transform.forward * firstPersonControls.Player.Move.ReadValue<Vector2>().y;
        }

        // move the player horizontal and vertical
        if (controller.enabled) {controller.SimpleMove(inputDirection.normalized * (speed * Time.deltaTime));}
        //if (controller.enabled) { controller.SimpleMove(inputDirection.normalized * (speed * Time.deltaTime));}
        else if (!controller.enabled) { rigidBody.AddForce(Vector3.ClampMagnitude(inputDirection.normalized * (speed * Time.deltaTime * AirStrafeSpeed), AirStrafeClamp));}
    }

    //When Space is pressed the player switches to RigidBody and is Forced though the air 
    //This disables the ground check until space is released
    public void Jump(InputAction.CallbackContext context)
    {
        
        if (context.performed && Grounded)
        {
            jumping = true;
            Grounded = false;
            offGroundTimer = offGroundTimerEnd;
            SoulSwitch(false);
            GroundRadius = 0;

            rigidBody.AddForce(new Vector3(controller.velocity.x, JumpHeight, controller.velocity.z), ForceMode.Impulse);
            source.PlayOneShot(bounce);

        }
        else if(context.canceled)
        {
            jumping = false;
            GroundRadius = groundRadiusStored;
        }
    }

    //Rotates the player horizontally and the camera vertically in accordance with the mouse
    public void Look(InputAction.CallbackContext context)
    {
        mousesense = mousey.mousey;
        mouseSensitivity = mousey.valSlide;
        float mouseX = context.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = context.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

      


    }

    //Checks the surface being aimed at and instanciates a rune on it if valid
    private void spawn_Rune(string type)
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.GetComponent<Camera>().transform.position, mainCamera.GetComponent<Camera>().transform.rotation * Vector3.forward, out hit, RuneRange, ~playerLayer) && RuneRefresh <= runeTimer)
        {
            Debug.Log("Object: " + hit.transform.name + " Layer: " + hit.transform.gameObject.layer + " Runeable?: " + (hit.transform.gameObject.layer == RuneAble) + " Runeable: " + RuneAble);
            runeTimer = 0;
            if (type == "bounce" && (RuneAble & (1 << hit.transform.gameObject.layer)) != 0) 
            {
                BounceRunes.Add(Instantiate(bounceRunePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)));
                BounceRunes[BounceRunes.Count - 1].GetComponent<runeBehavior>().StickTo(hit.transform); 
            }
            else if (type == "launch" && LaunchCatchStorage[0] == null && (RuneAble & (1 << hit.transform.gameObject.layer)) != 0) 
            {
                LaunchCatchStorage[0] = Instantiate(launchRunePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                LaunchCatchStorage[0].GetComponent<LaunchBehavior>().StickTo(hit.transform);
                if (LaunchCatchStorage[1] != null)
                {
                    LCRuneSets[launchScroll] = LaunchCatchStorage[0];
                    LaunchCatchStorage[0].GetComponent<LaunchBehavior>().NewCatch(LaunchCatchStorage[1]);
                    Array.Clear(LaunchCatchStorage,0,2);
                    LaunchIconsPlaced[launchScroll].SetActive(true);
                }
            }
            else if (type == "catch" && LaunchCatchStorage[1] == null && (RuneAble & (1 << hit.transform.gameObject.layer)) != 0) 
            {
                LaunchCatchStorage[1] = Instantiate(catchRunePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                LaunchCatchStorage[1].GetComponent<runeBehavior>().StickTo(hit.transform);
                if (LaunchCatchStorage[0] != null)
                {
                    LCRuneSets[launchScroll] = LaunchCatchStorage[0];
                    LaunchCatchStorage[0].GetComponent<LaunchBehavior>().NewCatch(LaunchCatchStorage[1]);
                    Array.Clear(LaunchCatchStorage, 0, 2);
                    LaunchIconsPlaced[launchScroll].SetActive(true);
                }

                source.PlayOneShot(catchCast);
                
            }
            else { GameObject.Find("CrossHairBase").GetComponent<Image>().color = new Color32(255, 0, 0, 255); }


        }
        else { GameObject.Find("CrossHairBase").GetComponent<Image>().color = new Color32(255, 0, 0, 255); }
    }
    
    private void move_Rune(string type)
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.GetComponent<Camera>().transform.position, mainCamera.GetComponent<Camera>().transform.rotation * Vector3.forward, out hit, RuneRange, ~playerLayer) && RuneRefresh <= runeTimer)
        {
            Debug.Log("Object: " + hit.transform.name + " Layer: " + hit.transform.gameObject.layer + " Runeable?: " + (hit.transform.gameObject.layer == RuneAble) + " Runeable: " + RuneAble);
            runeTimer = 0;
            if (type == "bounce" && (RuneAble & (1 << hit.transform.gameObject.layer)) != 0)
            { 
                BounceRunes[0].gameObject.transform.position = hit.point;
                BounceRunes[0].gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                GameObject placedRune = BounceRunes[0];
                BounceRunes.RemoveAt(0);
                BounceRunes.Add(placedRune);
                placedRune.GetComponent<runeBehavior>().StickTo(hit.transform);
            }
            else if(type == "launch" && (RuneAble & (1 << hit.transform.gameObject.layer)) != 0)
            {
                LaunchCatchTemp[0].gameObject.transform.position = hit.point;
                LaunchCatchTemp[0].gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                LaunchCatchTemp[0].GetComponent<LaunchBehavior>().StickTo(hit.transform);

            }
            else if(type == "catch" && (RuneAble & (1 << hit.transform.gameObject.layer)) != 0)
            {
                LaunchCatchTemp[1].gameObject.transform.position = hit.point;
                LaunchCatchTemp[1].gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                LaunchCatchTemp[1].GetComponent<runeBehavior>().StickTo(hit.transform);


                source.PlayOneShot(catchCast);
            }
            else { GameObject.Find("CrossHairBase").GetComponent<Image>().color = new Color32(255, 0, 0, 255); }
        }
        else { GameObject.Find("CrossHairBase").GetComponent<Image>().color = new Color32(255, 0, 0, 255); }
    }

    //if primary is clicked then a bounceRune will be spawned
    public void Primary(InputAction.CallbackContext context)
    {

        
        if (context.performed && LCRuneSets[launchScroll] == null) { spawn_Rune("launch");}
        else if (context.performed && LCRuneSets[launchScroll] != null)
        {
            LaunchCatchTemp[0] = LCRuneSets[launchScroll];
            LaunchCatchTemp[1] = LCRuneSets[launchScroll].GetComponent<LaunchBehavior>().GetCatch();
            move_Rune("launch");
        }
        source.PlayOneShot(runeCast);

        if (context.canceled)
        {
            GameObject.Find("CrossHairBase").GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        }
    }

    public void Secondary(InputAction.CallbackContext context)
    {
        if (context.performed && LCRuneSets[launchScroll] == null) { spawn_Rune("catch");}
        else if (context.performed && LCRuneSets[launchScroll] != null)
        {
            LaunchCatchTemp[0] = LCRuneSets[launchScroll];
            LaunchCatchTemp[1] = LCRuneSets[launchScroll].GetComponent<LaunchBehavior>().GetCatch();
            move_Rune("catch");
        }
        source.PlayOneShot(runeCast);

        if (context.canceled)
        {
            GameObject.Find("CrossHairBase").GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        }
    }

    public void Update()
    {
        if (pausemenu.paused)
            return;

        
    }

    void LaunchIndicatorCheck(float active)
    {
        
        LaunchIconActive[0].SetActive(0 == active && 1 <= MaxLaunchCatchRunes);
        LaunchIconActive[1].SetActive(1 == active && 2 <= MaxLaunchCatchRunes);
        LaunchIconActive[2].SetActive(2 == active && 3 <= MaxLaunchCatchRunes);
        LaunchIconActive[3].SetActive(3 == active && 4 <= MaxLaunchCatchRunes);
        LaunchIconActive[4].SetActive(4 == active && 5 <= MaxLaunchCatchRunes);
        LaunchIconActive[5].SetActive(5 == active && 6 <= MaxLaunchCatchRunes);
        LaunchIconActive[6].SetActive(6 == active && 7 <= MaxLaunchCatchRunes);
        LaunchIconActive[7].SetActive(7 == active && 8 <= MaxLaunchCatchRunes);
        LaunchIconActive[8].SetActive(8 == active && 9 <= MaxLaunchCatchRunes);
        LaunchIconActive[9].SetActive(9 == active && 10 <= MaxLaunchCatchRunes);
        
        
        LaunchIconsPlacedBG[0].SetActive(1 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[1].SetActive(2 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[2].SetActive(3 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[3].SetActive(4 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[4].SetActive(5 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[5].SetActive(6 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[6].SetActive(7 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[7].SetActive(8 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[8].SetActive(9 <= MaxLaunchCatchRunes);
        LaunchIconsPlacedBG[9].SetActive(10 <= MaxLaunchCatchRunes);
        
        
        LaunchIconsPlaced[0].SetActive(1 <= MaxLaunchCatchRunes && LCRuneSets[0] != null);
        
        LaunchIconsPlaced[1].SetActive(2 <= MaxLaunchCatchRunes && LCRuneSets[1] != null);
        LaunchIconsPlaced[2].SetActive(3 <= MaxLaunchCatchRunes && LCRuneSets[2] != null);
        LaunchIconsPlaced[3].SetActive(4 <= MaxLaunchCatchRunes && LCRuneSets[3] != null);
        LaunchIconsPlaced[4].SetActive(5 <= MaxLaunchCatchRunes && LCRuneSets[4] != null);
        LaunchIconsPlaced[5].SetActive(6 <= MaxLaunchCatchRunes && LCRuneSets[5] != null);
        LaunchIconsPlaced[6].SetActive(7 <= MaxLaunchCatchRunes && LCRuneSets[6] != null);
        LaunchIconsPlaced[7].SetActive(8 <= MaxLaunchCatchRunes && LCRuneSets[7] != null);
        LaunchIconsPlaced[8].SetActive(9 <= MaxLaunchCatchRunes && LCRuneSets[8] != null);
        LaunchIconsPlaced[9].SetActive(10 <= MaxLaunchCatchRunes && LCRuneSets[9] != null);
        
    }

    public void LaunchStart()
    {
        jumping = true;
        Grounded = false;
        offGroundTimer = offGroundTimerEnd;
        SoulSwitch(false);
        GroundRadius = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedHieght, transform.position.z), GroundRadius);
    }

    public void PlayerSounds()
    {

    }





}

