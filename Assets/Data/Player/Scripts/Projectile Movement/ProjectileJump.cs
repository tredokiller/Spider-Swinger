using System;
using Data.Common.Enums;
using Data.Player.Scripts.Movement.Controller;
using UnityEngine;

namespace Data.Player.Scripts.Projectile_Movement
{
    public class ProjectileJump : MonoBehaviour
    {
        private PlayerController _controller;
        private ProjectileMovement _projectileMovement;
        
        private bool _isGrounded;
        private bool _canJump = true;
        private bool _isJumping;
        
        [SerializeField] private float maxJumpDistance = 6;
        [SerializeField] private float distanceFromPlayerToJumpRay = 4;
        [SerializeField] private float jumpHeight = 5;
        [SerializeField] private float rayLength = 10;
        [SerializeField] private float jumpSpeed = 5;

        [SerializeField] private LayerMask collisionLayers;


        public static Action StartJumpingFromBuilding;
        
        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            
            PlayerController.StopProjectileMovement += ResetCanJump;
            
            _projectileMovement = _controller.GetProjectileMovement();
        }

        private void Update()
        {
            CheckJumpRay();
            CheckJump();
        }
        

        private void CheckJumpRay()
        {
            if (_controller.GetIsGrounded() && _controller.GetGroundSpeed() == GroundSpeed.Run)
            {
                var direction =  _controller.GetPlayerHorizontalVelocity().normalized;
                UnityEngine.Debug.DrawRay(_controller.transform.position + direction* distanceFromPlayerToJumpRay , Vector3.down * rayLength);
                if (!Physics.Raycast(_controller.transform.position + direction * 
                        distanceFromPlayerToJumpRay , Vector3.down, rayLength , collisionLayers))
                {
                    _isJumping = true;
                }
                    
                
            }
        }
        

        private void CheckJump()
        {
            if (_isJumping &&  _canJump)
            {
                JumpFromHighPlace();
            }
        }

        private void ResetCanJump()
        {
            _canJump = true;
            _isJumping = false;
        }

        private void JumpFromHighPlace()
        {
            StartJumpingFromBuilding?.Invoke();
            
            _canJump = false;
            
            _projectileMovement.FlipParabolaVertical(false);
            
            var direction =  _controller.GetPlayerHorizontalVelocity().normalized;

            Vector3 startPoint = transform.position;
            Vector3 finalPoint = transform.position + (direction * maxJumpDistance);
            
            _projectileMovement.SetHeight(jumpHeight);
            _projectileMovement.SetStartSpeed(new Vector2(jumpSpeed, jumpSpeed));
            
            _projectileMovement.StartMoving(startPoint, finalPoint);
        }
        
        
    }
}
