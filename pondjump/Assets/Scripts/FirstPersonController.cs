﻿using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;
		[Tooltip("The height the run can jump you")]
		public float RuneJumpHeight = 3.5f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("Rune Casting")]
		public GameObject launchRunePrefab;
		public GameObject bounceRunePrefab;
		public GameObject catchRunePrefab;
		private Vector3 decalDir = new Vector3(0, 1, 0);
		public float runeTimerStart;
		public float runeTimerFin = 15;
		private float runeTimer;
		private bool catchNeeded;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private Rigidbody _rigidbody;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		//Animator
		public GameObject frogArm;
		Animator animator;

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_rigidbody = GetComponent<Rigidbody>();

			animator = frogArm.GetComponent<Animator>();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			runeTimer = runeTimerStart;
		}

        private void FixedUpdate()
        {
			LaunchCheck();
			BounceCheck();
		}

        private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
			
		}

		private void LateUpdate()
		{
			CameraRotation();

		}

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("LaunchRune"))
			{
				Launch(other.gameObject);
			} 
			else if (other.CompareTag("BounceRune"))
			{
				Debug.Log("Bouncing");
				Bounce(other.gameObject);
			}
		}



		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * Time.deltaTime;
				_rotationVelocity = _input.look.x * RotationSpeed * Time.deltaTime;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void spawn_Rune(GameObject type)
		{
			RaycastHit hit;
			if (Physics.Raycast(_mainCamera.GetComponent<Camera>().transform.position, _mainCamera.GetComponent<Camera>().transform.rotation * Vector3.forward, out hit))
			{
				Instantiate(type,
					hit.point /*+ Vector3.Scale(hit.normal, launchRunePrefab.transform.localScale) / 2*/,
					Quaternion.FromToRotation(decalDir, hit.normal));
				//Debug.Log(Quaternion.FromToRotation(decalDir, hit.normal));
				//animator.SetBool("isFiring", false);
			}
		}

		private void Launch(GameObject rune)
		{
			Debug.Log("bang");
		}

		private void LaunchCheck()
        {
			//checks if button is pressed & the timer has passed(power has recharged)
			if (_input.launch && runeTimer >= runeTimerFin)
			{
				//Checks if animation is playing and if not plays it
				if (!animator.GetBool("isFiring"))
				{
					//Debug.Log("isfiring");
					animator.SetBool("isFiring", true);
				}

				//resets timer and instanciates the rune
				if (!catchNeeded)
				{
					runeTimer = runeTimerStart;
					spawn_Rune(launchRunePrefab);
					catchNeeded = true;
				}
				else
                {
					runeTimer = runeTimerStart;
					spawn_Rune(catchRunePrefab);
					catchNeeded = false;
				}
			}
			//if timer is not done adds to current timer
			else if (runeTimer < runeTimerFin)
			{
				animator.SetBool("isFiring", false);
				runeTimer++;
			}
		}

		private void Bounce(GameObject rune)
        {
			_verticalVelocity = Mathf.Sqrt(RuneJumpHeight * -2f * Gravity);
			Debug.Log("pow");
				
		}

		private void BounceCheck()
		{
			//checks if button is pressed & the timer has passed(power has recharged)
			if (_input.bounce && runeTimer >= runeTimerFin)
			{
				//Checks if animation is playing and if not plays it
				if (!animator.GetBool("isFiring"))
				{
					//Debug.Log("isfiring");
					animator.SetBool("isFiring", true);
				}

				//resets timer and instanciates the rune
					runeTimer = runeTimerStart;
					spawn_Rune(bounceRunePrefab);
			}
			//if timer is not done adds to current timer
			else if (runeTimer < runeTimerFin)
			{
				animator.SetBool("isFiring", false);
				runeTimer++;
			}
		}

		private void Move()
		{

			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			//if (!Grounded) inputMagnitude = 0;



			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			

			// move the player horizontal and vertical
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

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

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}