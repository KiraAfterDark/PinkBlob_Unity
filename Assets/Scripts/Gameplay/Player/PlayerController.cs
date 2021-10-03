using System;
using System.Collections;
using Cinemachine;
using PinkBlob.Gameplay.Ability;
using PinkBlob.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace PinkBlob.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public event Action OnNewAbility;
        
        // Required Components
        private CharacterController characterController;

        // Global
        private new Camera camera;
        
        // Input
        private PlayerInput playerInput;

        private Vector2 movementInput;

        public Vector2 MovementInput => movementInput;

        private PinkBlobPhysics PinkBlobPhysics => GameplayController.Instance.Physics;

        [Title("Properties")]

        [SerializeField]
        private AbilityType defaultAbility = AbilityType.Normal;

        [Title("Visuals")]

        [Required]
        [SerializeField]
        private new Renderer renderer = default;

        // Movement
        
        public Vector3 Velocity => velocity;
        private Vector3 velocity;
        
        [Title("Movement")]
        
        [Min(0)]
        [SerializeField]
        private float maxInputSpeed = 5f;

        [Min(0)]
        [SerializeField]
        private float maxCrouchInputSpeed = 2f;

        [Min(0)]
        [SerializeField]
        private float maxFlyInputSpeed = 3f;

        public float MaxSpeed
        {
            get
            {
                if (isFlying)
                {
                    return maxFlyInputSpeed * ability.MaxInputSpeedMod;
                }

                if (isCrouched)
                {
                    return maxCrouchInputSpeed * ability.MaxInputSpeedMod;
                }

                return maxInputSpeed * ability.MaxInputSpeedMod;
            }
        }

        [SerializeField]
        private float minSpeed = 0.1f;

        [Min(0)] 
        [SerializeField]
        private float groundAccel = 50f;

        [Min(0)] 
        [SerializeField]
        private float airAccel = 30f;

        public float Accel => !isGrounded ? airAccel : groundAccel;

        [Min(0)]
        [SerializeField]
        private float groundDecel = 25f;
        
        [Min(0)]
        [SerializeField]
        private float airDecel = 10f;

        public float Decel => !isGrounded ? airDecel : groundDecel;

        // Jumping + Flying
        
        public bool IsFlying => isFlying;
        private bool isFlying = false;
        
        [Title("Jump")]
        
        [Min(0)]
        [SerializeField]
        private float jumpSpeed = 100f;

        [Min(0)]
        [SerializeField]
        private int numberOfJumps = 5;
        
        public int RemainingJumps => numberOfJumps - jumps;
        private int jumps = 0;

        [Tooltip("Jumps don't happen if vertical speed is above this.")]
        [Min(0)]
        [SerializeField]
        private float maxJumpSpeed = 5f;

        public bool IsCrouched => isCrouched;
        private bool isCrouched = false;

        [Title("Rotation")]
        
        [Min(0)]
        [SerializeField]
        private float baseRotationSpeed = 10f;

        private float RotationSpeed => baseRotationSpeed * ability.RotationSpeedMod;

        // Slide
        private bool isSlide = false;

        [Title("Slide")]
        
        [Min(0)]
        [SerializeField]
        private float slideSpeed = 20f;

        [Min(0)]
        [SerializeField]
        private float slideTimer = 2f;

        [Title("Colliders")]

        [Required]
        [SerializeField]
        private Collider flyCollider = default;
        
        [Required]
        [SerializeField]
        private Collider crouchCollider = default;

        [Title("Camera")]
        
        [Required]
        [SerializeField]
        private CinemachineFreeLook baseCamera = default;

        [Title("Ground Check")]

        [Min(0)]
        [SerializeField]
        private float groundCheckHeight = 0.2f;

        [Min(0)]
        [SerializeField]
        private float groundCheckRadius = 0.5f;

        [SerializeField]
        private Vector3 groundGroundCheckOffset = Vector3.zero;

        [SerializeField]
        private Vector3 airGroundCheckOffset = Vector3.zero;
        
        private bool isGrounded = false;

        public bool IsGrounded => isGrounded;

        [Title("Positions")]

        [Required]
        [SerializeField]
        private Transform suckLocation = default;

        public Transform SuckLocation => suckLocation;

        // Ability
        private PlayerAbility ability;
        
        public PlayerAbility Ability => ability;

        [Title("Animation")]

        [Required]
        [SerializeField]
        private Animator animator = default;

        [SerializeField]
        private string crouchParam = "Crouch";

        [SerializeField]
        private string flyParam = "Fly";

        #region Unity Events
        
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            flyCollider.enabled = false;
            crouchCollider.enabled = false;
            
            characterController.detectCollisions = true;
            
            InitInput();
        }

        private void Start()
        {
            camera = Camera.main;

            jumps = 0;
            ability = this.GetAbility(defaultAbility);
        }

        private void OnEnable()
        {
            playerInput.Gameplay.Enable();
        }

        private void OnDisable()
        {
            playerInput.Gameplay.Disable();
        }

        private void Update()
        {
            CheckGround();
            UpdateInput();
            UpdateMovement();
            UpdateGravity();

            ability.Update();
        }

        private void FixedUpdate()
        {
            ability.FixedUpdate();
        }

        #endregion
        
        #region Input

        private void InitInput()
        {
            playerInput = new PlayerInput();

            playerInput.Gameplay.Jump.performed += ctx => Jump();
            playerInput.Gameplay.CancelAbility.performed += ctx => CancelAbility();

            playerInput.Gameplay.Action.started += ctx => 
                                                   {
                                                       if (isCrouched)
                                                       {
                                                           StartSlide();
                                                       }
                                                       else
                                                       {
                                                           ability?.OnStartAction();
                                                       }
                                                   };
            
            playerInput.Gameplay.Action.canceled += ctx =>
                                                    {
                                                        if (!isCrouched)
                                                        {
                                                            ability?.OnCancelAction();
                                                        }
                                                    };
            
            playerInput.Gameplay.Action.performed += ctx =>
                                                     {
                                                         if (!isCrouched)
                                                         {
                                                             ability?.OnPerformedAction();
                                                         }
                                                     };
            
            playerInput.Gameplay.Crouch.started += ctx => OnStartCrouch();
            playerInput.Gameplay.Crouch.canceled += ctx => OnCancelCrouch();
        }

        private void UpdateInput()
        {
            movementInput = IsMovementInputLock() ? Vector2.zero : playerInput.Gameplay.Movement.ReadValue<Vector2>();

            var cameraRotation = playerInput.Gameplay.CameraRotation.ReadValue<Vector2>();

            baseCamera.m_XAxis.m_InputAxisValue = cameraRotation.x;
            baseCamera.m_YAxis.m_InputAxisValue = cameraRotation.y;
        }

        private bool IsMovementInputLock()
        {
            return ability.MovementInputLock || isSlide;
        }

        #endregion
        
        #region Movement

        private void UpdateMovement()
        {
            Vector3 forward = camera.transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Vector3 right = camera.transform.right;
            right.y = 0;
            right.Normalize();

            float vert = velocity.y;
            
            if (movementInput.magnitude != 0)
            {
                velocity.y = 0;
                Vector3 inputVelocity = forward * (movementInput.y * Accel * Time.deltaTime)
                                      + right * (movementInput.x * Accel * Time.deltaTime);

                Vector3 vel = velocity + inputVelocity;
                
                if (vel.magnitude > MaxSpeed)
                {
                    velocity = vel.normalized * MaxSpeed;
                }
                else
                {
                    velocity = vel;
                }
                
                if (velocity.magnitude != 0)
                {
                    UpdateRotation(velocity.normalized);
                }
            }
            else
            {
                velocity = velocity.normalized * (velocity.magnitude - (Decel * Time.deltaTime));
                if (velocity.magnitude < 0)
                {
                    velocity = Vector3.zero;
                }
            }

            if (velocity.magnitude <= minSpeed)
            {
                velocity = Vector3.zero;
            }

            velocity.y = vert;

            characterController.Move(velocity * Time.deltaTime);
        }

        private void UpdateRotation(Vector3 aimDirection)
        {
            Vector3 forward = aimDirection;
            forward.y = 0;
            forward.Normalize();

            transform.forward = Vector3.RotateTowards(transform.forward, forward, RotationSpeed * Time.deltaTime, 0);
        }

        private Vector3 GetFriction(Vector3 velocity)
        {
            if (velocity.magnitude > 0.1f && isGrounded)
            {
                return PinkBlobPhysics.GroundFrictionCoef * Time.deltaTime * -velocity.normalized;
            }

            if (velocity.magnitude > 0.1f && !isGrounded)
            {
                return PinkBlobPhysics.AirFrictionCoef * Time.deltaTime * -velocity.normalized;
            }

            return Vector3.zero;
        }

        private void Jump()
        {
            if (jumps < numberOfJumps && velocity.y < maxJumpSpeed && (isGrounded || !ability.FlyingLock) && !isCrouched)
            {
                velocity += jumpSpeed * Vector3.up;
                jumps++;

                if (!isGrounded)
                {
                    SwitchToFlying();
                }
            }
        }

        private void UpdateGravity()
        {
            if (!isGrounded)
            {
                velocity += PinkBlobPhysics.Gravity * Time.deltaTime * Vector3.up;
            }
        }

        #endregion

        private void CheckGround()
        {
            bool oldCheck = isGrounded;
            Vector3 offset = isGrounded ? groundGroundCheckOffset : airGroundCheckOffset;
            Vector3 origin = transform.position + offset;
            
            isGrounded = Physics.CheckBox(origin, new Vector3(groundCheckRadius, groundCheckHeight, groundCheckRadius), 
                                          transform.rotation, PinkBlobPhysics.GroundMask)
                      && Physics.CheckCapsule(origin + Vector3.up * groundCheckHeight, origin - Vector3.up * groundCheckHeight,
                                              groundCheckRadius, PinkBlobPhysics.GroundMask);

            if (!oldCheck && isGrounded)
            {
                LandOnGround();
            }
        }

        private void SwitchToFlying()
        {
            isFlying = true;
            
            flyCollider.enabled = true;
            characterController.detectCollisions = false;
            crouchCollider.enabled = false;
        }

        private void LandOnGround()
        {
            isFlying = false;
            
            velocity.y = 0;
            jumps = 0;
            
            flyCollider.enabled = false;
            characterController.detectCollisions = true;
            crouchCollider.enabled = false;
        }

        private void OnStartCrouch()
        {
            if (isCrouched || isFlying)
            {
                return;
            }

            isCrouched = true;
            
            flyCollider.enabled = false;
            characterController.detectCollisions = false;
            crouchCollider.enabled = true;

            animator.SetBool(crouchParam, true);
            
            ability?.OnPerformedCrouch();
        }
        
        private void OnCancelCrouch()
        {
            if (!isCrouched || isSlide)
            {
                return;
            }

            isCrouched = false;
            
            flyCollider.enabled = false;
            characterController.detectCollisions = true;
            crouchCollider.enabled = false;

            animator.SetBool(crouchParam, false);
        }

        public void SetAbility(AbilityType abilityType)
        {
            ability = this.GetAbility(abilityType);
            renderer.material = ability.Material;
            
            OnNewAbility?.Invoke();
        }

        private void CancelAbility()
        {
            SetAbility(AbilityType.Normal);
        }

        private void StartSlide()
        {
            if (isSlide || !isCrouched)
            {
                return;
            }

            StartCoroutine(PerformSlide());
        }

        private IEnumerator PerformSlide()
        {
            isSlide = true;
            Vector3 slideVelocity = transform.forward * slideSpeed;
            velocity = slideVelocity;
            
            yield return new WaitForSeconds(slideTimer);

            isSlide = false;
            if (!playerInput.Gameplay.Crouch.inProgress)
            {
                isCrouched = false;

                flyCollider.enabled = false;
                characterController.detectCollisions = true;
                crouchCollider.enabled = false;

                animator.SetBool(crouchParam, false);
            }
        }

        #region Debugging
        
        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawCube(transform.position, new Vector3(groundCheckRadius, groundCheckHeight, groundCheckRadius) * 2);
            
            ability?.OnDrawGizmos();
        }

        #endregion
    }
}
