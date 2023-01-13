using PuzzleDungeon.Input;
using Tide;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : CharacterComponent, IInputReceiver
    {
        [Header("Movement")]
        [SerializeField] private Vector2 acceleration;
        [SerializeField] private Vector2 deceleration;
        [SerializeField] private Vector2 counterMovementAcceleration;
        [SerializeField] private Vector3 maxAbsoluteVelocity;
        
        [Header("Ground check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistanceOffset;
        [SerializeField] private float groundCheckRadiusOffset;
        
        [Header("Gravity/Jumping")]
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private bool    applyCoyoteTime;
        [SerializeField] private float   coyoteTime;
        [SerializeField] private float   jumpForce;
        [SerializeField] private float   maxJumpTime;
        [SerializeField] private float   inAirMovementAccelerationMultiplier;
        [Tooltip("How much velocity should player lose when landing")]
        [SerializeField] private Vector2 landingVelocityPenalty;
        
        [Header("Animator")]
        [SerializeField] private FloatAnimatorParameter speedAnimatorParameter;
        [SerializeField] private FloatAnimatorParameter speedXAnimatorParameter;
        [SerializeField] private FloatAnimatorParameter speedZAnimatorParameter;
        
        private Vector3                _velocity;
        private Vector3                _motionVector;
        private Rigidbody              _rigidbody;
        private CharacterController    _characterController;
        private bool                   _grounded;
        private bool                   _jumping;
        private float                  _jumpTime;
        private float                  _coyoteTimeTimer;
        private bool                   _countingCoyoteTime;
        
        public Vector2 P_RawMovementVector { get; private set; }
        
        public float P_CurrentSpeed => _rigidbody.velocity.magnitude;
        public float P_SpeedZ       => _rigidbody.velocity.z;
        public float P_SpeedX       => _rigidbody.velocity.x;
        
        public override void Initialize()
        {
            _rigidbody           = GetComponent<Rigidbody>();
            _characterController = GetComponent<CharacterController>();

            speedAnimatorParameter.Initialize(P_CharacterHub.P_Animator);
            speedXAnimatorParameter.Initialize(P_CharacterHub.P_Animator);
            speedZAnimatorParameter.Initialize(P_CharacterHub.P_Animator);
        }

        public void SetMovementVector(Vector2 value)
        {
            P_RawMovementVector = value;
        }

        public void AddVelocity(Vector3 velocity)
        {
            _velocity += velocity;
        }
        
        private void GroundCheck()
        {
            var center   = _characterController.bounds.center;
            var radius   = _characterController.radius                      + groundCheckRadiusOffset;
            var distance = (_characterController.height / 2) - (radius / 2) + groundCheckDistanceOffset;

            var touchingGround = Physics.SphereCast(center, radius, Vector3.down, out var hitInfo, distance, groundLayer);

            if (touchingGround && !_grounded)
            {
                OnCharacterBecameGrounded();
            }

            if (!touchingGround && _grounded)
            {
                OnCharacterStoppedBeingGrounded();
            }
        }

        private void OnCharacterBecameGrounded()
        {
            _velocity.y         = Mathf.Epsilon;
            _grounded           = true;
            _countingCoyoteTime = false;

            // Apply landing velocity penalty
            var directionX = Mathf.Sign(_velocity.x);
            var directionZ = Mathf.Sign(_velocity.z);
            
            _velocity.x += landingVelocityPenalty.x * -directionX;
            _velocity.z += landingVelocityPenalty.y * -directionZ;
            
            // Check if applying penalty didnt change velocity direction
            if (!Mathf.Approximately(directionX , Mathf.Sign(_velocity.x)))
            {
                _velocity.x = 0;
            }
            
            if (!Mathf.Approximately(directionZ , Mathf.Sign(_velocity.z)))
            {
                _velocity.z = 0;
            }
        }

        private void OnCharacterStoppedBeingGrounded()
        {
            if (!_jumping && applyCoyoteTime && !_countingCoyoteTime)
            {
                _coyoteTimeTimer    =  coyoteTime;
                _countingCoyoteTime =  true;
                _coyoteTimeTimer    -= Time.deltaTime;
                return;
            }

            if (_countingCoyoteTime)
            {
                _coyoteTimeTimer -= Time.deltaTime;
                if (_coyoteTimeTimer > 0)
                {
                    return;
                }
            }

            _countingCoyoteTime = false;
            _grounded           = false;
        }
        
        private float GetAccelerationValue(float acceleration)
        {
            return _grounded ? acceleration : acceleration * inAirMovementAccelerationMultiplier;
        }

        private void CalculateDeceleration()
        {
            if (Mathf.Abs(P_RawMovementVector.y) <= 0)
            {
                if (Mathf.Abs(_velocity.z) <= 0.1)
                {
                    _velocity.z = 0;
                }
                else
                {
                    var dir = (Mathf.Sign(_velocity.z) * -1);
                    _velocity.z += deceleration.y * Time.deltaTime * dir;  
                }
            }
            
            if (Mathf.Abs(P_RawMovementVector.x) <= 0)
            {
                if (Mathf.Abs(_velocity.x) <= 0.1)
                {
                    _velocity.x = 0;
                }
                else
                {
                    var dir = (Mathf.Sign(_velocity.x) * -1);
                    _velocity.x += deceleration.x * Time.deltaTime * dir;
                }
            }
        }

        private void CalculateCounterMovement()
        {
            // If player is changing direction, apply additional acceleration until velocity direction matches input direction
            var movementDirectionX = Mathf.Sign(P_RawMovementVector.x);
            var movementDirectionZ = Mathf.Sign(P_RawMovementVector.y);

            if (!Mathf.Approximately(movementDirectionX, _velocity.x))
            {
                _velocity.x += Time.deltaTime * GetAccelerationValue(counterMovementAcceleration.x) * P_RawMovementVector.x;
            }
            if (!Mathf.Approximately(movementDirectionZ, _velocity.z))
            {
                _velocity.z += Time.deltaTime * GetAccelerationValue(counterMovementAcceleration.y) * P_RawMovementVector.y;
            }
        }
        
        private void CalculateAcceleration()
        {
            // Regular acceleration
            _velocity.z += GetAccelerationValue(acceleration.y) * Time.deltaTime * P_RawMovementVector.y;
            _velocity.x += GetAccelerationValue(acceleration.x) * Time.deltaTime * P_RawMovementVector.x;
            
                        
            if (_grounded || _jumping) return;
            
            // If character is not grounded and not jumping apply gravity
            _velocity.y -= gravity * Time.deltaTime;
        }
    
        private void Move()
        {
            _velocity = Tools.ClampVector3(_velocity, maxAbsoluteVelocity * -1, maxAbsoluteVelocity);

            _motionVector = new Vector3(_velocity.x, _velocity.y, _velocity.z);
            _motionVector = _characterController.transform.rotation * _motionVector;
            
            _characterController.Move(_motionVector * Time.deltaTime);
        }
        
        private void CalculateJump()
        {
            if (!_jumping)
            {
                return;
            }

            if (_jumpTime >= maxJumpTime)
            {
                StopJumping();
                return;
            }

            _velocity.y += jumpForce * Time.deltaTime;
            _jumpTime   += Time.deltaTime;
        }
        
        private void StartJump()
        {
            if (!_grounded || _jumping)
            {
                return;
            }
            
            _jumping  = true;
            _jumpTime = 0f;
        }
        
        private void StopJumping()
        {
            if (!_jumping)
            {
                return;
            }

            _jumping = false;
        }

        #region CharacterComponent

        public override void ProcessUpdate()
        {
            GroundCheck();
            CalculateJump();
            CalculateAcceleration();
            CalculateDeceleration();
            CalculateCounterMovement();
            
            Move();
        }

        public override void UpdateAnimator()
        {
            speedAnimatorParameter.UpdateAnimator(P_CurrentSpeed);
            speedXAnimatorParameter.UpdateAnimator(P_SpeedX);
            speedZAnimatorParameter.UpdateAnimator(P_SpeedZ);
        }

        #endregion

        #region IInputReceiver

        public void ReceiveInputUpdate(InputManager input)
        {
            SetMovementVector(input.P_MovementVector2.P_CurrentValue);
        }

        public void ListenToInputEvents(InputManager input)
        {
            input.P_JumpButton.E_ButtonPress   += StartJump;
            input.P_JumpButton.E_ButtonRelease += StopJumping;
        }

        public void StopListeningToInputEvents(InputManager input)
        {
            input.P_JumpButton.E_ButtonPress   -= StartJump;
            input.P_JumpButton.E_ButtonRelease -= StopJumping;
        }

        #endregion
        
        private void OnDrawGizmos()
        {
            var defaultMatrix = Gizmos.matrix;
            
            #region Movement direction
            
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color  = Color.yellow;
            Gizmos.DrawCube(new Vector3(0f, 0.05f, 0.75f), new Vector3(0.05f, 0.05f, 1.5f));

            #endregion
            
            #region Ground Check

            if (_characterController != null)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(_characterController.bounds.center, _characterController.transform.rotation, _characterController.transform.lossyScale);

                if (_grounded)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                
                var radius   = _characterController.radius                    + groundCheckRadiusOffset;
                var distance = (_characterController.height /2) - (radius /2) + groundCheckDistanceOffset;
                
                Gizmos.DrawSphere(new Vector3(0f, -distance, 0f), radius); 
            }

            #endregion
        }
    }
}