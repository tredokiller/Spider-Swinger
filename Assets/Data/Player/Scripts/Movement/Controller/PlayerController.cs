using System;
using Data.Common.Enums;
using Data.Player.Scripts.Movement.Controller.Enums;
using Data.Player.Scripts.Projectile_Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform playerMesh;
        [SerializeField] private Transform playerCameraTarget;

        [Header("GroundProperties")] 
        [SerializeField] private float smoothSpeedUp = 0.05f;
        
        private bool _jumpButtonPressed;
        private bool _runButtonPressed;
        private bool _walkButtonPressed;
        
        private bool _canJump;
        
        private bool _isRunning;
        private bool _isWalking;

        private readonly float _walkSpeed = 2.8f;
        private readonly float _jogSpeed = 5f;
        private readonly float _runSpeed = 7f;
        
        private GroundSpeed _groundSpeed;
        
        private float _currentHorizontalSpeed;
        private float _currentVerticalPositiveSpeed;
        
        private readonly float jumpHeight = 20f;
        
        [Header("Rotation")]
        [SerializeField] private float turnSmoothTime = 2;
        [SerializeField] private float projectileTurnSmoothTime = 1;
        [SerializeField] private float swingingSmoothTime;
        
        private float _turnSmoothVelocity;
        private float _targetAngle;
        
        [Header("WallProperties")] 
        [SerializeField] private LayerMask buildingsMask;

        private bool _wallJumpIsStarted;
        public bool ShouldInvertWallAnimations { get; private set; }
        
        [SerializeField] private Transform wallCheckPosition;
        
        [SerializeField] private float wallJumpUpForce;
        [SerializeField] private float wallJumpSideForce;

        [SerializeField] private float smoothWallDirectionLerpTime;

        private Directions _wallJumpDirection;

        [SerializeField , Range(0, 1)] private float wallJumpUpTime;
        
        private RaycastHit _wallRay;

        private const float WallClimbingSpeed = 10f;
        private const float WallRunSpeed = 15f;
        
        private bool _isWallColliding;
        
        [Header("FlyProperties")]
        public bool shouldControlHeight;
        
        private RaycastHit lastRayHit;

        private const float ToSwingCullDown = 0.8f;
        [SerializeField] private float flyYSpeedMultiplier;
        [SerializeField] private float flyXSpeedMultiplier;

        private float turnFlySmoothValue;

        [SerializeField] private float afterSwingingHorizontalSpeedMultiplier;
        
        [SerializeField] private float truePlayerY;
        
        [SerializeField] private float ropeLenghtMultiplier;
        
        private bool _isSwinging;
        private bool _canSetControlPositionY;
        
        private bool _canSwing;
        private bool _canFlyJump;
        private bool _canSpeedUp;
        
        private Vector3 _flyDirection;
        
        private const float MinFlyHorizontalSpeed = 1f;
        public const float MaxFlyHorizontalSpeed = 30f;

        public const float MaxFlyVerticalSpeed = 12f;

        [FormerlySerializedAs("slowSpeedTime")] [SerializeField] private float slowFlySpeedTime = 0.8f;
        [FormerlySerializedAs("multiplySpeedTime")] [SerializeField] private float multiplyFlySpeedTime = 0.8f;


        [SerializeField] private float ropeHeightConst = 0.4f;

        [SerializeField] private RayList swingingRays;
        [SerializeField] private RayList airAccelerationRays;
        
        [SerializeField] private ProjectileMovement projectileMovement;
        
        [Header("AirTricks")] 
        [SerializeField] private float turnTrickTime;

        private bool _trickButtonPressed;

        [Header("AirAcceleration")]
        [SerializeField] private float minRayDistanceToAcceleration;

        [SerializeField , Range(0, 1)] private float airAccelerationTime;

        [SerializeField, Range(0, 1)] private float afterAirAccelerationMultiplier;
        
        [SerializeField] private float airAccelerationHorizontalSpeed;
        [SerializeField] private float airAccelerationVerticalSpeed;
        
        [SerializeField] private float airAccelerationSpeedMultiplier;
        
        [Header("Overall")]
        public const string PlayerTag = "Player";
        
        private bool _isProjectileMovement;
        
        public Transform leftHandTransform;
        public Transform rightHandTransform;

        private Transform mainHandTransform;
        private Directions _mainHandDirections;
        
        private bool _isCustomSpeed;
        private float _customSpeed;
        
        private States _playerState;

        private float _playerRadius;
        public const float PlayerHeight = 2.73f;

        private Vector3 _playerHorizontalVelocity;
        private Vector3 _playerVelocity;
        private float _playerVerticalVelocity;

        private readonly Vector3[] _directions = {Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        private Vector3 _direction;

        private Vector2 _inputPlayer;
        
        private bool _isGrounded;
        private bool _isStaying;

        private Camera _camera;
        
        [SerializeField] private Transform groundCheckerPosition;
        
        [SerializeField] private InputManager inputManager;
        private GameInput.PlayerActions _playerActions;
        
        private CharacterController _controller;
        
        [SerializeField] private LayerMask groundMask;

        [Header("SwingingRopeLenght")]
        private float _swingingRopeLenght;
        private readonly Collider[] _wallCollider = new Collider[1];
        
        [Header("SitPoint")] 
        private bool _isSitPointColliding;
        private bool _canDoBigJumpFromSitPoint;

        private const float DurationToBigJump = 1.5f;
        
        private const float SitBigJumpVerticalSpeed = 15f;
        private const float SitJumpVerticalSpeed = 6f;
        
        private const float SitBigJumpHorizontalSpeed = 15f;
        private const float SitJumpHorizontalSpeed = 5f;
        
        private const float SitJumpHorizontalAfterFlyMultiplier = 1.5f;
        private const float SitBigJumpHorizontalAfterFlyMultiplier = 2.5f;
        
        [Header("Actions")] 
        public static Action StartSwingingAction;
        public static Action StopSwingingAction;

        public static Action StartProjectileMovement;
        public static Action StopProjectileMovement;

        public static Action StartProjectileFlyToSitPoint;
        public static Action StopProjectileFlyToSitPoint;

        public static Action StartFlyToSitPointDefault;
        public static Action StartFlyToSitPointRoll;
        
        public static Action StartWallClimbing;
        public static Action StartAirAccelerating;

        public static Action StartAirMovement;
        public static Action StartGroundMovement;

        public static Action StartAirTrick;
        public static Action StopAirTrick;

        public Vector3 GetPlayerVelocity()
        {
            return _playerVelocity;
        }
        
        public GameInput.PlayerActions GetPlayerButtonActions()
        {
            return _playerActions;
        }
        
        public float GetPlayerCurrentHorizontalSpeed()
        {
            return _currentHorizontalSpeed;
        }

        public Vector3 GetPlayerWallRayNormalized()
        {
            return _wallRay.normal;
        }
        
        public float GetPlayerCurrentVerticalSpeed()
        {
            return _playerVerticalVelocity;
        }

        public float GetPlayerSwingingRopeLenght()
        {
            return _swingingRopeLenght;
        }

        public ProjectileMovement GetProjectileMovement()
        {
            return projectileMovement;
        }
        
        public GroundSpeed GetGroundSpeed()
        {
            return _groundSpeed;
        }

        public States GetPlayerCurrentState()
        {
            return _playerState;
        }

        public bool GetIsStaying()
        {
            return _isStaying;
        }
        
        public bool GetIsGrounded()
        {
            return _isGrounded;
        }

        public Vector3 GetGroundCheckerPosition()
        {
            return groundCheckerPosition.position;
        } 

        public LayerMask GetGroundMask()
        {
            return groundMask;
        }

        public Transform GetMainHandTransform()
        {
            return mainHandTransform;
        }

        public Directions GetMainHandDirection()
        {
            return _mainHandDirections;
        }
        
        public RaycastHit GetLastRayHit()
        {
            return lastRayHit;
        }

        public Vector3 GetPlayerHorizontalVelocity()
        {
            return _playerHorizontalVelocity;
        }

        public Vector2 GetInputPlayer()
        {
            return _inputPlayer;
        }

        public bool GetIsProjectileMovement()
        {
            return _isProjectileMovement;
        }
        
    }
}
