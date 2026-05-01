using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		public float MoveSpeed = 4.0f;
		public float CrouchSpeed = 2.0f;

		public float RotationSpeed = 1.0f;
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		public float JumpHeight = 1.2f;
		public float Gravity = -15.0f;

		[Header("Crouch")]
		public float CrouchHeight = 1.0f;
		public float NormalHeight = 2.0f;
		public float CameraCrouchOffset = -0.5f;
		public float CrouchSmoothSpeed = 10f; // 👈 ความลื่น

		private bool _isCrouching;
		private float _currentHeight;
		private float _heightVelocity;

		private Vector3 _cameraOriginalPos;
		private Vector3 _cameraTargetPos;

		[Header("Grounded")]
		public bool Grounded = true;
		public float GroundedOffset = -0.14f;
		public float GroundedRadius = 0.5f;
		public LayerMask GroundLayers;

		[Header("Camera")]
		public GameObject CinemachineCameraTarget;
		public float TopClamp = 90.0f;
		public float BottomClamp = -90.0f;

		private float _cinemachineTargetPitch;

		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#endif

			NormalHeight = _controller.height;
			_currentHeight = NormalHeight;

			_cameraOriginalPos = CinemachineCameraTarget.transform.localPosition;
		}

		private void Update()
		{
			Crouch();
			GroundedCheck();
			JumpAndGravity();
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
			UpdateCrouchSmooth(); // 👈 สำคัญ
		}

		// ===================== CROUCH INPUT =====================
		private void Crouch()
		{
#if ENABLE_INPUT_SYSTEM
			_isCrouching = Keyboard.current.leftShiftKey.isPressed;
#else
			_isCrouching = Input.GetKey(KeyCode.LeftShift);
#endif
		}

		// ===================== SMOOTH SYSTEM =====================
		private void UpdateCrouchSmooth()
		{
			float targetHeight = _isCrouching ? CrouchHeight : NormalHeight;

			// Smooth height
			_currentHeight = Mathf.Lerp(_currentHeight, targetHeight, Time.deltaTime * CrouchSmoothSpeed);
			_controller.height = _currentHeight;

			// ปรับ center ให้ไม่ลอย
			_controller.center = new Vector3(0, _currentHeight / 2f, 0);

			// Smooth camera
			float targetCamY = _isCrouching ? _cameraOriginalPos.y + CameraCrouchOffset : _cameraOriginalPos.y;

			Vector3 camPos = CinemachineCameraTarget.transform.localPosition;
			camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * CrouchSmoothSpeed);
			CinemachineCameraTarget.transform.localPosition = camPos;
		}

		private void GroundedCheck()
		{
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			if (_input.look.sqrMagnitude >= _threshold)
			{
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				_cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);

				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0, 0);
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			float targetSpeed = _isCrouching ? CrouchSpeed : MoveSpeed;

			if (_input.move == Vector2.zero) targetSpeed = 0;

			float currentSpeed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

			if (currentSpeed < targetSpeed)
			{
				_speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
			}
			else
			{
				_speed = targetSpeed;
			}

			Vector3 inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + Vector3.up * _verticalVelocity * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				if (_verticalVelocity < 0)
					_verticalVelocity = -2f;

				if (_input.jump && !_isCrouching)
				{
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}
			}

			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}
	}
}