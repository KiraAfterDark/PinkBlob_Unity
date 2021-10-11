using Cinemachine;
using PinkBlob.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        // Defaults
        private new Rigidbody rigidbody;
        private Camera mainCamera;
        
        // Input
        private PlayerInput playerInput;
        
        // Movement
        private Vector2 movementInput = Vector2.zero;
        private Vector2 rotationInput = Vector2.zero;

        [TitleGroup("Hookups")]
        [Required]
        [SerializeField]
        private Transform visuals = default;
        
        [TitleGroup("Movement")]
        [Min(0)]
        [SerializeField]
        private float movementSpeed = 10f;

        [TitleGroup("Movement")]
        [Min(0)]
        [SerializeField]
        private float jumpForce = 10f;

        private bool onGround = false;

        private Vector3 prevMove;

        [TitleGroup("Movement")]
        [Min(0)]
        [SerializeField]
        private float rotationSpeed = 50f;

        private Vector3 prevMovement;

        [TitleGroup("Air Movement")]
        [Min(0)]
        [SerializeField]
        private float inAirAccel = 50f;

        [TitleGroup("Air Movement")]
        [Min(0)]
        [SerializeField]
        private float maxAirSpeed = 8f;

        [TitleGroup("Camera")]
        [Required]
        [SerializeField]
        private CinemachineFreeLook cinemachine = default;

        [TitleGroup("Camera")]
        [Min(0)]
        [SerializeField]
        private Vector2 cameraSensitivity = Vector2.one;
        
        [TitleGroup("Sucking")]
        [Min(0)]
        [SerializeField]
        private float suckingMovementSpeed = 4f;

        #region Awake
        
        private void Awake()
        {
            InitDefaults();
            InitInput();
        }

        private void InitDefaults()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void InitInput()
        {
            playerInput = new PlayerInput();
            
            playerInput.Gameplay.Jump.performed += ctx => Jump();
        }
        
        #endregion
        
        #region Enable

        private void OnEnable()
        {
            playerInput.Gameplay.Enable();
        }

        private void OnDisable()
        {
            playerInput.Gameplay.Disable();
        }
        
        #endregion
        
        #region start

        private void Start()
        {
            mainCamera = Camera.main;
        }
        
        #endregion
        
        #region Update

        private void Update()
        {
            ReadInput();
        }

        private void ReadInput()
        {
            movementInput = playerInput.Gameplay.Movement.ReadValue<Vector2>();
            rotationInput = playerInput.Gameplay.Rotation.ReadValue<Vector2>();
        }
        
        #endregion
        
        #region FixedUpdate

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            Transform cameraTransform = mainCamera.transform;
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            forward.Normalize();

            right.y = 0;
            right.Normalize();

            Vector3 forwardMove = movementInput.y * movementSpeed * forward;
            Vector3 rightMove = movementInput.x * movementSpeed * right;
            Vector3 cameraInputVector = forwardMove + rightMove;
            
            if (cameraInputVector.magnitude > 0)
            {
                UpdateRotation(cameraInputVector.normalized);
            }
            
            if (cameraInputVector.magnitude > 0)
            {
                UpdateRotation(cameraInputVector.normalized);
            }
            
            rigidbody.MovePosition(transform.position + cameraInputVector * Time.fixedDeltaTime);
            prevMove = cameraInputVector;
        }

        private void UpdateRotation(Vector3 dir)
        {
            dir.y = 0;
            dir.Normalize();

            Vector3 newLook = Vector3.RotateTowards(visuals.forward, dir, rotationSpeed * Time.fixedDeltaTime, 0);

            visuals.forward = newLook;
        }

        #endregion
        
        #region LateUpdate

        private void LateUpdate()
        {
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            cinemachine.m_XAxis.m_InputAxisValue = rotationInput.x * cameraSensitivity.x;
            cinemachine.m_YAxis.m_InputAxisValue = rotationInput.y * cameraSensitivity.y;
        }
        
        #endregion
        
        #region InputEvents

        private void Jump()
        {
            if (onGround)
            {
                rigidbody.AddForce(jumpForce * Vector3.up);
            }
        }
        
        #endregion
        
        #region Collision
        
        // Collisions
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Ground"))
            {
                onGround = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.CompareTag("Ground"))
            {
                onGround = false;
            }
        }
        
        #endregion
    }
}
