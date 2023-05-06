using Cinemachine;
using Data.Common.PauseMenu;
using Data.Player_Camera.Scripts.Enums;
using Data.Player.Scripts.Movement.AnimationController;
using Data.Player.Scripts.Movement.Controller;
using UnityEngine;

namespace Data.Player_Camera.Scripts
{
    public class CameraSettings : MonoBehaviour
    {
        [Header("IK")] 
        [SerializeField] private Transform headTarget;

        [Header("Camera Modes")] 
        [SerializeField] private CinemachineFreeLook groundCameraMode;
        [SerializeField] private CinemachineFreeLook swingCameraMode;
        [SerializeField] private CinemachineFreeLook wallCameraMode;
        [SerializeField] private CinemachineFreeLook airCameraMode;
        [SerializeField] private CinemachineFreeLook fastFallingCameraMode;
        [SerializeField] private CinemachineFreeLook sitOnPointCameraMode;

        [Header("Player")] 
        [SerializeField] private Transform playerMesh;

        [Header("Rotation")] 
        [SerializeField] private float rotationToSwingSpeed = 10f;
        
        private bool _rotateToSwingDirection;
        private const float RotationToSwingMultiplier = 0.3f;
        
        private CameraMode _currentMode;

        private bool _canChangeCameraMode = true;

        private void Awake()
        {
            _rotateToSwingDirection = false;
            SwitchCameraMode(CameraMode.Ground);
        }

        public Transform GetHeadTarget()
        {
            return headTarget;
        }


        private void OnEnable()
        {
            PlayerAnimationController.StartFastFalling += () => SwitchCameraMode(CameraMode.FastFalling);

            PlayerController.StartSwingingAction += () => SwitchCameraMode(CameraMode.Swing);

            PlayerController.StartSwingingAction += () => _rotateToSwingDirection = true;
            PlayerController.StopSwingingAction += () => _rotateToSwingDirection = false;

            PlayerController.StartWallClimbing += () => SwitchCameraMode(CameraMode.Wall);
            PlayerController.StartGroundMovement += () => SwitchCameraMode(CameraMode.Ground);
            PlayerController.StartAirMovement += () => SwitchCameraMode(CameraMode.Air);
            PlayerController.StopProjectileFlyToSitPoint += () => SwitchCameraMode(CameraMode.Sit);

            
            MenuManager.OnMenuOpened += () => SetCanChangeCameraMode(false);
            MenuManager.OnMenuOpened += () => SetOnCameraModes(false);
            
            MenuManager.OnMenuClosed += () => SetCanChangeCameraMode(true);
            MenuManager.OnMenuClosed += () => SwitchCameraMode(_currentMode);
        }

        private void OnDisable()
        {
            PlayerAnimationController.StartFastFalling -= () => SwitchCameraMode(CameraMode.FastFalling);
            PlayerController.StartSwingingAction -= () => SwitchCameraMode(CameraMode.Swing);
            PlayerController.StartWallClimbing -= () => SwitchCameraMode(CameraMode.Wall);
            PlayerController.StartGroundMovement -= () => SwitchCameraMode(CameraMode.Ground);
            PlayerController.StartAirMovement -= () => SwitchCameraMode(CameraMode.Air);
            
            MenuManager.OnMenuOpened -= () => SetOnCameraModes(false);
            MenuManager.OnMenuOpened -= () => SetCanChangeCameraMode(false);
            
            MenuManager.OnMenuClosed -= () => SetCanChangeCameraMode(true);
            MenuManager.OnMenuClosed -= () => SwitchCameraMode(_currentMode);
        }


        private void LateUpdate()
        {
            RotateCameraToSwingDirection();
        }

        private void RotateCameraToSwingDirection()
        {
            if (_rotateToSwingDirection)
            {
                float angleToMove = playerMesh.localRotation.eulerAngles.z;

                if (angleToMove > 180f)
                {
                    angleToMove -= 360;
                }

                swingCameraMode.m_Lens.Dutch = Mathf.LerpAngle(swingCameraMode.m_Lens.Dutch,
                    angleToMove * RotationToSwingMultiplier, rotationToSwingSpeed * Time.deltaTime);
            }
            else
            {
                swingCameraMode.m_Lens.Dutch =
                    Mathf.LerpAngle(swingCameraMode.m_Lens.Dutch, 0, rotationToSwingSpeed * Time.deltaTime);
            }
        }

        private void SwitchCameraMode(CameraMode newMode)
        {
            _currentMode = newMode;

            SetOnCameraModes(false);
            
            if (_canChangeCameraMode == false)
            {
                return;
            }

            switch (_currentMode)
            {
                case CameraMode.Ground:
                    groundCameraMode.gameObject.SetActive(true);
                    break;

                case CameraMode.Swing:
                    swingCameraMode.gameObject.SetActive(true);
                    break;

                case CameraMode.Wall:
                    wallCameraMode.gameObject.SetActive(true);
                    break;

                case CameraMode.Air:
                    airCameraMode.gameObject.SetActive(true);
                    break;

                case CameraMode.FastFalling:
                    fastFallingCameraMode.gameObject.SetActive(true);
                    break;

                case CameraMode.Sit:
                    sitOnPointCameraMode.gameObject.SetActive(true);
                    break;

            }
        }

        private void SetOnCameraModes(bool isActive)
        {
            groundCameraMode.gameObject.SetActive(isActive);
            swingCameraMode.gameObject.SetActive(isActive);
            wallCameraMode.gameObject.SetActive(isActive);
            airCameraMode.gameObject.SetActive(isActive);
            fastFallingCameraMode.gameObject.SetActive(isActive);
            sitOnPointCameraMode.gameObject.SetActive(isActive);
        }

        private void SetCanChangeCameraMode(bool canChange)
        {
            _canChangeCameraMode = canChange;
        }
    }
}
