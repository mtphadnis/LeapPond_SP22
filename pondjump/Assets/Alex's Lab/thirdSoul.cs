using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class thirdSoul : MonoBehaviour
{
    private CharacterController controller;
    private FirstPersonControls firstPersonControls;
    private Rigidbody rigidBody;
    private GameObject mainCamera;
    private runeBehavior runeBehavior;

    [Header("Ground Detections")]
    [Tooltip("The hieght of the sphereCheck at the players bottom")]
    public float GroundedHieght;
    [Tooltip("The radius of the sphereCheck at the players bottom")]
    public float GroundRadius;
    //stores the set radius
    float groundRadiusStored;
    //the Ground layer
    LayerMask GroundLayers, RuneLayers, PlayerLayers;
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
    [Header("Health")]
    [Tooltip("Current Player Health")]
    [Range(0,1)]
    public float Damage;
    //whether the player is being hurt or not
    public bool inPain;
    Image HealthUI;
    [Tooltip("How much health percentage is lost per second while being damaged")]
    public float HealthLoss;
    [Tooltip("How much health percentage is gained per second while  not being damaged")]
    public float HealthGain;

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

    public bool Pause;

    [Space(10)]
    [Header("Grapple")]
    [Tooltip("If active [Secondary] will use grappling hood instead of Launch/Catch Runes")]
    public bool grappleActive;
    [Tooltip("Grappleing hook object")]
    public GameObject grapplingGun;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();

        GroundLayers = LayerMask.GetMask("Ground");
        RuneLayers = LayerMask.GetMask("Rune");
        PlayerLayers = LayerMask.GetMask("Player");
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

        //HealthUI = GameObject.Find("HealthUI").GetComponent<Image>();

        for(int i = 0; i < MaxLaunchCatchRunes; i++)
        {
            LCRuneSets.Add(null);
        }

        Cursor.lockState = CursorLockMode.Locked;

        //LaunchIndicatorCheck(0);
    }

    private void FixedUpdate()
    {
        WhatsUnder();
        Move();
        runeTimer++;
        Healing();
        Pausing();

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

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.tag == "Deadly" && Damage < 0.33)
        {
            Damage = 0.33f;
        }else if(other.transform.tag == "Deadly" && Damage < 1)
        {
            Damage += HealthLoss;
            inPain = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Deadly" && Damage > 0)
        {
            inPain = false;
        }
    }

    void Healing()
    {
        Damage += !inPain && Damage > 0 ? HealthGain : 0;
        //var tempColor = HealthUI.color;
        //tempColor.a = Damage;
        //HealthUI.color = tempColor;
        //HealthUI.fillAmount = Damage;
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
        Grounded = Physics.CheckSphere(spherePosition, GroundRadius, GroundLayers, QueryTriggerInteraction.Ignore) || (Physics.CheckSphere(spherePosition, GroundRadius, RuneLayers, QueryTriggerInteraction.Ignore) && crouching);

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
            if (startSwitch) { rigidBody.velocity = controller.velocity; startSwitch = false; }
            gravity = 0;
            offGroundTimer += Time.deltaTime;
            Grounded = false;
            SoulSwitch(false);
        }
        
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

    public void ToggleFire(InputAction.CallbackContext context)
    {
       
        if (context.performed) 
        { 
            grappleActive = !grappleActive;
            grapplingGun.GetComponent<GrapplingGun>().GrappleEnable(grappleActive); 
            
        }
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        //if(context.performed)
        //{
        //    scrollPosition += ((context.ReadValue<Vector2>().y) / 120);
        //    launchScroll = (int)(Math.Abs(scrollPosition%MaxLaunchCatchRunes));

        //    //LaunchIndicatorCheck(launchScroll);
            
        //    if(LCRuneSets[launchScroll] != null)
        //    {
        //        LaunchCatchTemp[0] = LCRuneSets[launchScroll];
        //        LaunchCatchTemp[1] = LCRuneSets[launchScroll].GetComponent<LaunchBehavior>().GetCatch();
        //    }
        //    else
        //    {
        //        LaunchCatchTemp[0] = null;
        //        LaunchCatchTemp[1] = null;
        //    }
            
        //}
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
        if (Physics.Raycast(mainCamera.GetComponent<Camera>().transform.position, mainCamera.GetComponent<Camera>().transform.rotation * Vector3.forward, out hit, RuneRange, ~PlayerLayers) && RuneRefresh <= runeTimer)
        {
            runeTimer = 0;
            if (type == "bounce" && grappleActive) 
            {
                BounceRunes.Add(Instantiate(bounceRunePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)));
                BounceRunes[BounceRunes.Count - 1].GetComponent<runeBehavior>().StickTo(hit.transform); 
            }
            
            else if (type == "launch" && grappleActive == false && LaunchCatchStorage[0] == null) 
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
            else if (type == "catch" && grappleActive == false && LaunchCatchStorage[1] == null) 
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
            }
            
        }
    }
    
    private void move_Rune(string type)
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.GetComponent<Camera>().transform.position, mainCamera.GetComponent<Camera>().transform.rotation * Vector3.forward, out hit, RuneRange, ~PlayerLayers) && RuneRefresh <= runeTimer)
        {
            runeTimer = 0;
            if (type == "bounce")
            { 
                BounceRunes[0].gameObject.transform.position = hit.point;
                BounceRunes[0].gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                GameObject placedRune = BounceRunes[0];
                BounceRunes.RemoveAt(0);
                BounceRunes.Add(placedRune);
                placedRune.GetComponent<runeBehavior>().StickTo(hit.transform);
            }
            else if(type == "launch")
            {
                LaunchCatchTemp[0].gameObject.transform.position = hit.point;
                LaunchCatchTemp[0].gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                LaunchCatchTemp[0].GetComponent<LaunchBehavior>().StickTo(hit.transform);
            }
            else if(type == "catch")
            {
                LaunchCatchTemp[1].gameObject.transform.position = hit.point;
                LaunchCatchTemp[1].gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                LaunchCatchTemp[1].GetComponent<runeBehavior>().StickTo(hit.transform);
            }
        }
    }

    //if primary is clicked then a bounceRune will be spawned
    public void Primary(InputAction.CallbackContext context)
    {
        
        if (grappleActive && context.performed && BounceRunes.Count < MaxBounceRunes) {spawn_Rune("bounce"); }
        else if (grappleActive && context.performed && BounceRunes.Count >= MaxBounceRunes) { move_Rune("bounce"); }
        else if (!grappleActive && context.performed && LCRuneSets[launchScroll] == null) { spawn_Rune("launch"); }
        else if (!grappleActive && context.performed && LCRuneSets[launchScroll] != null) 
        {
            LaunchCatchTemp[0] = LCRuneSets[launchScroll];
            LaunchCatchTemp[1] = LCRuneSets[launchScroll].GetComponent<LaunchBehavior>().GetCatch();
            move_Rune("launch"); 
        }
        
    }

    public void Secondary(InputAction.CallbackContext context)
    {
        if(!grappleActive && context.performed && LCRuneSets[launchScroll] == null) {spawn_Rune("catch"); }
        else if(!grappleActive && context.performed && LCRuneSets[launchScroll] != null)
        {
            LaunchCatchTemp[0] = LCRuneSets[launchScroll];
            LaunchCatchTemp[1] = LCRuneSets[launchScroll].GetComponent<LaunchBehavior>().GetCatch();
            move_Rune("catch"); 
        }
    }
    public void Pausing()
    {
        //float temp = firstPersonControls.Player.Paused.ReadValue<float>();
        //Debug.Log("Pause2: " + temp);
        //if(temp == 0)
        //{
        //    Pause = false;
        //}
        //else
        //{
        //    Pause = true;
        //}

    }

    public void Paused(InputAction.CallbackContext context)
    {
        Debug.Log("Pause: " + Pause);
        if (context.performed) { Pause = true; }
        else { Pause = false; }

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

}
