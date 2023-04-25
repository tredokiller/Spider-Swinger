using System;
using Cinemachine;
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

        [Header("Player")]
        [SerializeField] private Transform playerMesh;

        [Header("Rotation")]
        [SerializeField] private float rotationToSwingSpeed = 10f;
        private bool _rotateToSwingDirection;
        private const float RotationToSwingMultiplier = 0.3f;
        
        
        private CameraMode _currentMode;
        
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
        }

        private void OnDisable()
        {
            PlayerAnimationController.StartFastFalling -= () => SwitchCameraMode(CameraMode.FastFalling);
            PlayerController.StartSwingingAction -= () => SwitchCameraMode(CameraMode.Swing);
            PlayerController.StartWallClimbing -= () => SwitchCameraMode(CameraMode.Wall);
            PlayerController.StartGroundMovement -= () => SwitchCameraMode(CameraMode.Ground);
            PlayerController.StartAirMovement -= () => SwitchCameraMode(CameraMode.Air);
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
                swingCameraMode.m_Lens.Dutch = Mathf.LerpAngle(swingCameraMode.m_Lens.Dutch, angleToMove * RotationToSwingMultiplier, rotationToSwingSpeed * Time.deltaTime);
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
            
            groundCameraMode.gameObject.SetActive(false);
            swingCameraMode.gameObject.SetActive(false);
            wallCameraMode.gameObject.SetActive(false);
            airCameraMode.gameObject.SetActive(false);
            fastFallingCameraMode.gameObject.SetActive(false);
            
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
                    
            }
        }
    }
}
