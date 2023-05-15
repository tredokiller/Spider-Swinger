using System.Collections;
using Data.Common.Timer;
using Data.Player.Scripts.Movement.Controller.Enums;
using UnityEngine;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        private void StartBigJumpFromSitPointTimer()
        {
            _canDoBigJumpFromSitPoint = true;
            Timer.StartTimer(DurationToBigJump , () => _canDoBigJumpFromSitPoint = false);
        }
        
        private void SitMovement()
        {
            _playerState = States.Idle;
            TryToDoBigJump();
        }
        
        private void TryToDoBigJump()
        {
            if (_jumpButtonPressed && _isSitPointColliding)
            { 
                StartCoroutine(SitJumpCoroutine());
            }
        }
        
        private IEnumerator SitJumpCoroutine()
        {
            _isSitPointColliding = false;
            
            float elapsedTime = 0f;
            float maxTime = 0.1f;

            float currentHorizontalSpeed = SitBigJumpHorizontalSpeed;
            
            float verticalDirectionToApply = SitBigJumpVerticalSpeed;
            Vector3 horizontalDirectionToApply = _direction * currentHorizontalSpeed;

            float currentMultiplier = SitBigJumpHorizontalAfterFlyMultiplier;

            States stateOfJump = States.BigJumpFromSitPoint;

            if (_canDoBigJumpFromSitPoint == false)
            {
                verticalDirectionToApply = SitJumpVerticalSpeed;
                horizontalDirectionToApply = _direction * SitJumpHorizontalSpeed;

                currentHorizontalSpeed = SitJumpHorizontalSpeed;
                currentMultiplier = SitJumpHorizontalAfterFlyMultiplier;
                
                stateOfJump = States.JumpFromSitPoint;
            }
            
            while (elapsedTime < maxTime) 
            {
                _playerState = stateOfJump;
                _playerHorizontalVelocity = new Vector3(horizontalDirectionToApply.x, 0, horizontalDirectionToApply.z);
                _playerVerticalVelocity = verticalDirectionToApply;
                elapsedTime += Time.deltaTime;
                yield return null; 
            }

            
            _currentHorizontalSpeed = Mathf.Clamp(_playerHorizontalVelocity.magnitude, currentHorizontalSpeed, currentHorizontalSpeed) * currentMultiplier;
        }
    }
}
