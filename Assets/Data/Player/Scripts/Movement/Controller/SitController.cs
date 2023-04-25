using System.Collections;
using Data.Player.Scripts.Movement.Controller.Enums;
using DG.Tweening;
using UnityEngine;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        private void StartBigJumpFromSitPointTimer()
        {
            float timer = 0f;

            _canDoBigJumpFromSitPoint = true;
            
            DOTween.To(() => timer, x => timer = x, 1f, DurationToBigJump)
                .SetEase(Ease.Linear)
                .OnComplete(() => _canDoBigJumpFromSitPoint = false);
        }
        
        private void SitMovement()
        {
            TryToDoBigJump();
        }
        
        private void TryToDoBigJump()
        {
            {
                if (_jumpButtonPressed && _sitJumpFromSitPointStarted == false)
                {
                    StartCoroutine(SitBigJumpCoroutine());
                }
            }
        }
        
        private IEnumerator SitBigJumpCoroutine()
        {
            _isSitPointColliding = false;
            
            float elapsedTime = 0f;
            float maxTime = 0.1f;
            
            _sitJumpFromSitPointStarted = true;

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
            _sitJumpFromSitPointStarted = false;
        }
    }
}
