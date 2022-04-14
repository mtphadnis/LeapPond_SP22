using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class inputs : MonoBehaviour
{
    private Rigidbody rigidBody;
    private PlayerInput playerInput;
    private FirstPersonControls firstPersonControls;

    public GameObject CinemachineCameraTarget, Ground;

    public bool Grounded;
    public float speed;

    public Vector3 vectorTester;
    public Vector3 currentGravity;

    public float GroundedOffset, GroundedRadius, GroundedHieght;
    public LayerMask GroundLayers;

    public float mouseSensitivity;

    float xRotation = 0f;

    RaycastHit smartFoot;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        firstPersonControls = new FirstPersonControls();
        firstPersonControls.Player.Enable();
        firstPersonControls.Player.Jump.performed += Jump;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        WhatsUnder();
        ObeyGravity();

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

    void ObeyGravity()
    {
        if (Grounded == false)
        {
            //normal gravity, active when not grounded.
            currentGravity = Physics.gravity;
        }
        else if (Grounded == true)
        {
            /*Not normal gravity. Instead of going down, we go in the
            direction perpendicular to the angle of where we're standing. 
            This means whatever surface we're grounded on will be 
            effectively the same as standing on a perfectly horizontal 
            surface. Ergo, no sliding will occur. */
            currentGravity = -(smartFoot.normal) * Physics.gravity.magnitude;
        }
        rigidBody.AddForce(currentGravity, ForceMode.Force);

        Debug.Log(currentGravity);
        Debug.DrawRay(transform.position + Vector3.up, currentGravity * 5, Color.cyan);
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


        Vector3 groundMod = Vector3.RotateTowards(new Vector3(inputVector.x, 0, inputVector.y), smartFoot.normal.normalized, 1f, 0f);
        rigidBody.AddRelativeForce(direction * speed, ForceMode.Force);

        //Debug.Log("smartFoot: " + smartFoot.normal + "direction: " + direction + "groundMod: " + groundMod);
        Debug.DrawRay(transform.position + Vector3.up, groundMod * 5, Color.blue);
        Debug.DrawRay(transform.position + Vector3.up, direction * 5, Color.black);
        //Debug.DrawRay
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && Grounded)
        {
            rigidBody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}
