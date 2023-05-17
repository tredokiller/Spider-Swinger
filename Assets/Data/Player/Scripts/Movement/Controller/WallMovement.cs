using System.Collections;
using Data.Common.Enums;
using Data.Player.Scripts.Movement.Controller.Enums;
using UnityEngine;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        private void WallMovement()
        {
            {
                if (_runButtonPressed)
                {
                    _playerState = States.WallRun;
                }
                else
                {
                    _playerState = States.WallClimb;
                }
                
                if (_isSwinging)
                {
                    StopProjectileMovement?.Invoke();
                }

                RotationToWall();
                InvertWallAnimations();
                
                bool directionMovement = Physics.Raycast(transform.position + new Vector3(0, _inputPlayer.y * 0.7f, 0),
                    (_inputPlayer.y * _direction).normalized, out var hit, 3f, buildingsMask);
                
                
                if (!Physics.Raycast(transform.position + Vector3.up * 0.7f , -_wallRay.normal , out var climbOnWallHit, 2f , buildingsMask))
                {
                    if (Physics.Raycast(climbOnWallHit.point + (climbOnWallHit.normal * (_playerRadius * -3f))+ (Vector3.up * (0.6f * PlayerHeight)) , Vector3.down  * 1.3f, out var vaultToPlaceHit, PlayerHeight))
                    {
                        UnityEngine.Debug.DrawRay(climbOnWallHit.point + (climbOnWallHit.normal * (_playerRadius * -3f)) + (Vector3.up * (0.6f * PlayerHeight)), Vector3.down * 1.3f, Color.green);
                        
                        _wallJumpDirection = Directions.Up;
                        OnWallJump();
                    }
                }
                
                else if (directionMovement)
                {
                    DirectionWallMove(hit);
                }
                else
                {
                    StrafeWallMove();
                }
            }
        }

        private void InvertWallAnimations()
        {
            ShouldInvertWallAnimations = false;
                
            if (_wallRay.normal.x < 0 || _wallRay.normal.z > 0)
            {
                ShouldInvertWallAnimations = true;
            }
        }
        
        private void StayOnWall()
        {
            _playerHorizontalVelocity.x = 0;
            _playerVerticalVelocity = 0;
            _playerHorizontalVelocity.z = 0;
        }

        private void MoveOnWall(Vector3 direction)
        {
            float speed = WallClimbingSpeed;
            
            if (_playerState == States.WallRun)
            {
                speed = WallRunSpeed;
            }
            
            _playerHorizontalVelocity.x = Mathf.Lerp(_playerHorizontalVelocity.x , direction.x * speed , smoothWallDirectionLerpTime  * Time.deltaTime);
            _playerVerticalVelocity = Mathf.Lerp( _playerVerticalVelocity, direction.y * speed, smoothWallDirectionLerpTime * Time.deltaTime);
            _playerHorizontalVelocity.z = Mathf.Lerp(_playerHorizontalVelocity.z , direction.z * speed , smoothWallDirectionLerpTime  * Time.deltaTime);
        }

        private void RotationToWall()
        {
            Vector3 directionToTarget = _wallRay.point - transform.position;

            // Calculate the rotation that would be used by LookAt
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            playerMesh.rotation = lookRotation;
        }
        
        private void DirectionWallMove(RaycastHit hit)
        {
            Vector3 direction = Vector3.zero;

            _wallJumpDirection = Directions.Up;

            if (_inputPlayer.y > 0)
            {
                direction = -(_wallRay.point - hit.point).normalized;
            }

            if (_inputPlayer.y < 0)
            {
                StayOnWall();
                return;
            }

            UnityEngine.Debug.DrawLine(transform.position + Vector3.up,
                transform.position + Vector3.up + direction * 2f, Color.green);
            
            {
                if (_inputPlayer != Vector2.zero)
                {
                    MoveOnWall(direction);
                }
                else
                {
                    StayOnWall();
                }
            }
        }


        private void StrafeWallMove()
        {
            _wallJumpDirection = Directions.Right;
            
            if (_jumpButtonPressed && _inputPlayer != Vector2.zero)
            {
                OnWallJump();
                return;
            }

            Vector3 crossProd = Vector3.Cross(_wallRay.normal, transform.up);
            
            if ((new Vector3(0, 0,  _direction.z) -crossProd).magnitude >
                (new Vector3(0, 0, _direction.z) + crossProd).magnitude)
            {
                crossProd = -crossProd;
            }
            

            if ((new Vector3(_direction.x, 0, 0) - crossProd).magnitude >
                (new Vector3(_direction.x, 0, 0) + crossProd).magnitude)
            {
                crossProd = -crossProd;
            }
            
            {
                if (_inputPlayer != Vector2.zero)
                {
                    MoveOnWall(crossProd);
                }

                else
                {
                    StayOnWall();
                } 
            }
            
            UnityEngine.Debug.DrawLine(_wallRay.point, _wallRay.point + crossProd);
        }
        
        private void OnWallJump()
        {
            StartCoroutine(WallJumpCoroutine());
        }

        private IEnumerator WallJumpCoroutine()
        {
            _wallJumpIsStarted = true;
            
            float elapsedTime = 0f;
            float maxTime = 0.1f;

            _playerState = States.JumpFromWall;
            
            
            float verticalDirectionToApply = 0;
            Vector3 horizontalDirectionToApply = Vector3.zero;

            switch (_wallJumpDirection)
            {
                case Directions.Up:
                    verticalDirectionToApply = wallJumpUpForce;
                    maxTime = 0.2f;
                    break;
                
                case Directions.Right:
                    verticalDirectionToApply = 6f;
                    horizontalDirectionToApply = _wallRay.normal * wallJumpSideForce;
                    break;
            }
            
            while (elapsedTime < maxTime)
            {
                _playerHorizontalVelocity = new Vector3(horizontalDirectionToApply.x, 0, horizontalDirectionToApply.z);
                _playerVerticalVelocity = verticalDirectionToApply;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            _currentHorizontalSpeed = Mathf.Max(_currentHorizontalSpeed, MinWallJumpSideForce);
            _wallJumpIsStarted = false;
        }

        
        
        
    }
    
}