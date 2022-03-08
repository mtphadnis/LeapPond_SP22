using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class twoSoulController : MonoBehaviour
{
    private CharacterController controller;
    private Rigidbody rigidBody;
    private PlayerInput playerInput;
    private FirstPersonControls firstPersonControls;
    public GameObject CinemachineCameraTarget;

    public bool Grounded;
    public float speed;

    public Vector3 vectorTester;

    public float GroundedOffset, GroundedRadius, GroundedHieght;
    public LayerMask GroundLayers;

    public float mouseSensitivity;

    float xRotation = 0f;

    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    RaycastHit smartFoot;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        firstPersonControls = new FirstPersonControls();
        firstPersonControls.Player.Enable();
        //firstPersonControls.Player.Jump.performed += Jump;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void FixedUpdate()
    {
        WhatsUnder();
        
        if (Grounded)
        {
            Move();
        }

    }


    private void WhatsUnder()
    {

        Grounded = Physics.Raycast(transform.position - (Vector3.down / 5), Vector3.down, out smartFoot, GroundedHieght, GroundLayers, QueryTriggerInteraction.Ignore);

        Debug.DrawRay(transform.position - (Vector3.down / 5), Vector3.down, Color.red);
        Debug.DrawRay(smartFoot.point, smartFoot.normal * 5, Color.green);

    }

    public void Look(InputAction.CallbackContext context)
    {

        float mouseX = context.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = context.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

    }

    private void Move()
    {
        Vector2 inputVector = firstPersonControls.Player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputVector.x, 0, inputVector.y);

        rigidBody.AddRelativeForce(direction * speed, ForceMode.Force);
        //controller.Move(direction.normalized);

        Debug.Log(direction);
        Debug.DrawRay(transform.position + Vector3.up, direction * 5, Color.black);
        
    }
    /*
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    */
    /*
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && Grounded)
        {
            rigidBody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
    */
}
