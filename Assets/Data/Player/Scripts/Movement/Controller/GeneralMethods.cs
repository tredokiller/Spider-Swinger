using Data.Common.Enums;
using UnityEngine;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        private void Awake()
        {
            _gameInput = new GameInput();
            _camera = Camera.main;

            _groundSpeed = GroundSpeed.Jog;
            
            _controller = GetComponent<CharacterController>();

            _playerActions = _gameInput.Player;
            
            _playerRadius = _controller.radius;
            
            BindInputToFunctions();
            BindActions();
        }
        
        private void OnEnable()
        {
            _gameInput.Enable();
        }
        
        void Update()
        {
            UpdateDirection();
            
            CheckGrounded();
            CheckWallRay();
            
            Movement();

            CheckIsStaying();

            ApplyGravity();
            
            PlayerRotate();
        }

        private void UpdateDirection()
        {
            Vector3 moveDir = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
            _direction = moveDir;
        }

        private void BindInputToFunctions()
        {
            _playerActions.Jump.started += context => OnJump();

            _playerActions.Jump.started += context => TryToAirAccelerate();
            
            _playerActions.Jump.canceled += context => CancelJump();
            
            _playerActions.Action.canceled += context => TryToStopSwinging();
            
            _playerActions.Run.started += context => _runButtonPressed = true;
            _playerActions.Run.canceled += context => _runButtonPressed = false;

            _playerActions.Action.started += context => _canSwing = true;

            _playerActions.AdditionalMovement.started += context => _walkButtonPressed = true;
            _playerActions.AdditionalMovement.canceled += context => _walkButtonPressed = false;

            _playerActions.Trick.started += context => TryToCreateAirTrick();
            _playerActions.Trick.canceled += context => TryToStopAirTrick();

        }

        private void BindActions()
        {
            StartProjectileMovement += () => _isProjectileMovement = true;
            StopProjectileMovement += () => _isProjectileMovement = false;
            
            StopProjectileMovement += StopSwinging;

            FinishProjectileFlyToSitPoint += () => _isSitPointColliding = true;
            FinishProjectileFlyToSitPoint += StartBigJumpFromSitPointTimer;
        }


        private void CheckGrounded()
        {
            bool newGroundedState = Physics.Raycast(groundCheckerPosition.position, Vector3.down, 0.2f , groundMask);

            if (newGroundedState && _isGrounded == false)
            {
                StartGroundMovement?.Invoke();
            }

            _isGrounded = newGroundedState;
        }

        private void CheckWallRay()
        {
            _wallCollider[0] = null;
            Physics.OverlapSphereNonAlloc(transform.position + Vector3.up, 5f, _wallCollider , 
                buildingsMask, QueryTriggerInteraction.Collide);

            if (_wallCollider[0] == null)
            {
                return;
            }
            bool hasCollision = false;
            foreach (Vector3 dir in _directions)
            {
                if (Physics.Raycast(transform.position, dir, out var tempRay, 1f, buildingsMask,
                        QueryTriggerInteraction.Collide))
                {
                    var trueDistanceToPlayer = (_wallRay.point - transform.position).sqrMagnitude;
                    var newDistanceToPlayer = (tempRay.point - transform.position).sqrMagnitude;

                    hasCollision = true;
                    if (tempRay.point != Vector3.zero)
                    {
                        if (_wallRay.point == Vector3.zero)
                        {
                            _wallRay = tempRay;
                        }
                        else
                        {
                            if (trueDistanceToPlayer >= newDistanceToPlayer)
                            {
                                _wallRay = tempRay;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (hasCollision)
            {
                if (_isGrounded && _inputPlayer.y < 0)
                {
                    _isWallColliding = false;
                    return;
                }

                if (_isWallColliding == false)
                {
                    StartWallClimbing?.Invoke();
                    StopAllCoroutines();

                    _isWallColliding = true;
                }
            }
            else
            {
                _isWallColliding = false;
            }
        }

        
        private void Move()
        {
            _playerVelocity = new Vector3(_playerHorizontalVelocity.x, _playerVerticalVelocity,
                _playerHorizontalVelocity.z);

            _controller.Move(_playerVelocity * Time.deltaTime);
        }

        private void Movement()
        {
            _inputPlayer = _playerActions.Movement.ReadValue<Vector2>();


            if (_isSitPointColliding)
            {
                SitMovement();
                return;
            }
            
            if (_isGrounded)
            {
                if (_isWallColliding)
                {
                    WallMovement();
                }
                else
                {
                    GroundMovement(); 
                }
            }

            if (_isWallColliding)
            {
                WallMovement();
            }
            else
            {
                if (!_isGrounded)
                {
                    FlyMovement();
                }
            }
            
            Move();
        }
        
        
        private void ApplyGravity()
        {
            if (_isGrounded)
            {
                _isSwinging = false;
                
                if (!_isWallColliding)
                {
                    _playerVerticalVelocity = 0f;
                }
            }
            
            if (_isGrounded && _jumpButtonPressed)
            {
                Jump();
            }
            

            if (_isSwinging)
            {
                _playerVerticalVelocity = 0f; 
            }

            else
            {
                _playerVerticalVelocity += WorldSettings.Gravity * Time.deltaTime;
            }
            _currentVerticalPositiveSpeed = Mathf.Abs(_playerVerticalVelocity);
        }


        private void PlayerRotate()
        {
            if (_isSitPointColliding)
            {
                _targetAngle = Mathf.Atan2(_inputPlayer.x, _inputPlayer.y) * Mathf.Rad2Deg +
                               _camera.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity,
                    turnSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            
            
            if (_inputPlayer.magnitude >= 0.1f)
            {
                if (_isGrounded == false)
                {
                    if (_trickButtonPressed)
                    {
                        return;
                    }
                }
                if (_isProjectileMovement == false)
                {
                    _targetAngle = Mathf.Atan2(_inputPlayer.x, _inputPlayer.y) * Mathf.Rad2Deg +
                                   _camera.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity,
                        turnSmoothTime);

                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }

                // playerCameraTarget.transform.rotation = Quaternion.Euler(0f, 0, 0f);
            }

            if (_isProjectileMovement)
            {
                Quaternion rotation = Quaternion.LookRotation(projectileMovement.finalPoint.position - transform.position);

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation.eulerAngles.y, ref _turnSmoothVelocity,
                    projectileTurnSmoothTime * Time.deltaTime);
                
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
        
        private void OnDisable()
        {
            _gameInput.Disable();
        }


        private void SetCustomSpeed(float speed)
        {
            _isCustomSpeed = true;
            _customSpeed = speed;
        }


        private void ResetCustomSpeed()
        {
            _isCustomSpeed = false;
        }
        
        private void CheckIsStaying()
        {
            if (_playerVelocity.magnitude <= 0.1f)
            {
                _isStaying = true;
                return;
            }
            _isStaying = false;
        }



        private void CalculateMainHand() //Calculating true hand for swing rope by direction to hit point
        {
            Vector3 directionToTarget = (lastRayHit.point - transform.position);

            float dot = Vector3.Dot(transform.right, directionToTarget);
                
            if (dot > 0)
            {
                mainHandTransform = rightHandTransform;
                _mainHandDirections = Directions.Right;
                return;
            }
         
            mainHandTransform = leftHandTransform;
            _mainHandDirections = Directions.Left;

        }
    }
}
