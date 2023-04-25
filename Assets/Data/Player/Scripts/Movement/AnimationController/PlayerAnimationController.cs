using System;
using Data.Common.Enums;
using Data.Player.Scripts.Movement.Controller;
using Data.Player.Scripts.Movement.Controller.Enums;
using Data.Player.Scripts.Projectile_Movement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data.Player.Scripts.Movement.AnimationController
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(IKManager))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private PlayerController _playerController;
        private Animator _animator;
        private ProjectileMovement _projectileMovement;
        private IKManager _ikManager;

        private Camera _camera;
        
        private Vector3 _playerNonZeroDirection;
        

        [SerializeField] private int swingingAnimationsCount = 3;
        [SerializeField] private int airAccelerationAnimationsCount = 2;
        [SerializeField] private float lowDistanceToGround = 5f;


        [SerializeField] private float minSpeedToFastFalling = 7f;

        private int swingingAnimationValue;

        private bool _isLowDistanceToGround;
        private bool _isFastFallingDown;
        private bool _isAirTrick;

        private bool _canActivateTriggers;
        
        private static readonly int LeftHand = Animator.StringToHash("LeftHand");
        private static readonly int RightHand = Animator.StringToHash("RightHand");
        
        private static readonly int State = Animator.StringToHash("State");
        
        private static readonly int CustomSpeedValue = Animator.StringToHash("CustomSpeedValue");
        private static readonly int SwingingAnimation = Animator.StringToHash("SwingingAnimation");
        
        private static readonly int IsLowDistanceToGround = Animator.StringToHash("IsLowDistanceToGround");
        private static readonly int IsFastFallingDown = Animator.StringToHash("IsFastFallingDown");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsStaying = Animator.StringToHash("IsStaying");
        private static readonly int IsAirTrick = Animator.StringToHash("IsAirTrick");

        private static readonly int PositionX = Animator.StringToHash("PositionX");
        private static readonly int PositionY = Animator.StringToHash("PositionY");
        
        private static readonly int WallRunning = Animator.StringToHash("WallRunning");
        private static readonly int AirAcceleration = Animator.StringToHash("AirAcceleration");
        private static readonly int Swinging = Animator.StringToHash("Swinging");
        private static readonly int JumpFromBuilding = Animator.StringToHash("JumpFromBuilding");
        private static readonly int AirTrick = Animator.StringToHash("AirTrick");
        private static readonly int SitOnPointDefault = Animator.StringToHash("SitOnPointDefault");
        private static readonly int SitOnPointRoll = Animator.StringToHash("SitOnPointRoll");
        
        [Header("Actions")]
        public static Action StartFastFalling;

        private static readonly int FastFallingDown = Animator.StringToHash("FastFallingDown");
        
        private void Awake()
        {
            _canActivateTriggers = true;
            
            _playerController = GetComponent<PlayerController>();
            _animator = GetComponent<Animator>();
            _ikManager = GetComponent<IKManager>();
            _projectileMovement = _playerController.GetProjectileMovement();

            _camera = Camera.main;
            
            PlayerController.StartSwingingAction += ActivateSwingingTriggers;

            PlayerController.StopProjectileMovement += () => RandomizeAnimations(swingingAnimationsCount);
            
            PlayerController.StartFlyToSitPointDefault += () => ActivateSitOnPointDefaultTrigger(SitOnPointDefault);
            PlayerController.StartFlyToSitPointRoll += () => ActivateSitOnPointDefaultTrigger(SitOnPointRoll);
            
            PlayerController.FinishProjectileFlyToSitPoint += ResetCanActivateTriggers;
            
            PlayerController.StartAirAccelerating += ActivateAirAccelerationTrigger;

            PlayerController.StartWallClimbing += ActivateWallRunTrigger;

            PlayerController.StartAirTrick += ActivateAirTrickTrigger;
            PlayerController.StopAirTrick += ResetAirTrickTrigger;

            StartFastFalling += ActivateFastFallingTrigger;

            ProjectileJump.StartJumpingFromBuilding += ActivateJumpFromBuildingTrigger;
        }


        private void SetAnimatorVariables()
        {
           _animator.SetInteger(State , (int)_playerController.GetPlayerCurrentState());
           _animator.SetInteger(SwingingAnimation , swingingAnimationValue);
           
           _animator.SetBool(IsLowDistanceToGround , _isLowDistanceToGround);
           _animator.SetBool(IsGrounded , _playerController.GetIsGrounded()); 
           _animator.SetBool(IsStaying , _playerController.GetIsStaying());
           _animator.SetBool(IsFastFallingDown , _isFastFallingDown);
           _animator.SetBool(IsAirTrick , _isAirTrick);
           
           _animator.SetFloat(CustomSpeedValue , _projectileMovement.GetFromZeroToOnePathValue());
           
           var playerCurrentState = _playerController.GetPlayerCurrentState();
           if (playerCurrentState == States.WallClimb || playerCurrentState == States.WallRun)
           {
               Vector3 direction = _playerController.GetPlayerVelocity().normalized;

               float horizontalValue = 0f;
               
               if (direction != Vector3.zero)
               {
                   _playerNonZeroDirection = direction;
                   horizontalValue = _playerNonZeroDirection.x + _playerNonZeroDirection.z;
                   
                   if (_playerController.ShouldInvertWallAnimations)
                   {
                       horizontalValue = -horizontalValue;
                   }
               }
               else
               {
                   var wallVector = _playerController.GetPlayerWallRayNormalized();
                   horizontalValue = -(_camera.transform.rotation * wallVector).x;

                   if (wallVector.x != 0)
                   {
                       horizontalValue = -horizontalValue;
                   }
                   
               }

               
               _animator.SetFloat(PositionX , horizontalValue);
               _animator.SetFloat(PositionY , direction.y);
           }
           
           if (_playerController.GetPlayerCurrentState() == States.Air)
           {
               Vector2 inputPlayer = _playerController.GetInputPlayer();
               
               _animator.SetFloat(PositionX, inputPlayer.x);
               _animator.SetFloat(PositionY, inputPlayer.y);
               
               if (_playerController.GetPlayerCurrentHorizontalSpeed() < minSpeedToFastFalling && _isLowDistanceToGround == false && _playerController.GetPlayerCurrentVerticalSpeed() <= 0)
               {
                   if (_isFastFallingDown == false)
                   {
                       if (_canActivateTriggers)
                       {
                           _isFastFallingDown = true;
                           StartFastFalling?.Invoke();
                       }
                   }
                   return;
               }
               _isFastFallingDown = false;
           }
        }

        private void Update()
        {
            SetAnimations();
            SetAnimatorVariables();
        }

        private void RandomizeAnimations(int count)
        {
            int previousValue = swingingAnimationValue;
            swingingAnimationValue = Random.Range(1, count+ 1);

            if (previousValue == swingingAnimationValue)
            {
                RandomizeAnimations(count);
            }
        }

        private void ActivateSwingingTriggers()
        {
            UpdateMainHandTrigger();
            _animator.SetTrigger(Swinging);
        }


        private void ActivateSitOnPointDefaultTrigger(int sitPointTriggerId)
        {
            _animator.SetTrigger(sitPointTriggerId);
            _canActivateTriggers = false;
        }

        private void ActivateJumpFromBuildingTrigger()
        {
            _animator.SetTrigger(JumpFromBuilding);
        }

        private void ActivateAirTrickTrigger()
        {
            _animator.SetTrigger(AirTrick);
            _ikManager.SetRigEnable(0);
            _isAirTrick = true;
        }
        
        private void ResetAirTrickTrigger()
        {
            _animator.ResetTrigger(AirTrick);
            _ikManager.SetRigEnable(1);
            _isAirTrick = false;
        }

        private void ActivateFastFallingTrigger()
        {
            if (_canActivateTriggers)
            {
                _animator.SetTrigger(FastFallingDown);
            }
        }

        private void UpdateMainHandTrigger()
        {
            var direction = _playerController.GetMainHandDirection();
            
            _animator.ResetTrigger(LeftHand);
            _animator.ResetTrigger(RightHand);
            
            if (direction == Directions.Left)
            {
                _animator.SetTrigger(LeftHand);
            }
            else
            {
                _animator.SetTrigger(RightHand);
            }
            
        }

        private void ActivateWallRunTrigger()
        {
            _animator.SetTrigger(WallRunning);
        }

        private void ActivateAirAccelerationTrigger()
        {
            RandomizeAnimations(airAccelerationAnimationsCount);
            UpdateMainHandTrigger();
            _animator.SetTrigger(AirAcceleration);
        }
        
    
        private void SetAnimations()
        {
            if (_playerController.GetPlayerCurrentState() == States.Air)
            {
                CheckLowDistanceToGroundDistance();
            }
        }

        private void CheckLowDistanceToGroundDistance()
        {
            _isLowDistanceToGround = Physics.Raycast(_playerController.GetGroundCheckerPosition(), Vector3.down,
                lowDistanceToGround, _playerController.GetGroundMask());
        }

        private void ResetCanActivateTriggers()
        {
            _canActivateTriggers = true;
        }
        
    }
}
