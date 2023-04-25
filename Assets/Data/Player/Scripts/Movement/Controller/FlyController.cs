using System.Collections;
using Data.Player.Scripts.Movement.Controller.Enums;
using UnityEngine;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        private void TryToStartSwinging()
        {
            if (!_isGrounded && !_isSwinging)
            {
                var hitOrNull = swingingRays.GetTheBestRayHit();
                
                if (hitOrNull == null)
                {
                    return;
                }

                _canSwing = false;
                
                lastRayHit = hitOrNull.Value;

                projectileMovement.FlipParabolaVertical(true);
                
                Vector3 finalPointDirection = _direction;
                
                projectileMovement.hitPoint.position = lastRayHit.point;

                _swingingRopeLenght = lastRayHit.distance;

                //float maxY = transform.position.y + _swingingRopeLenght;
                float maxY = lastRayHit.point.y;

                Vector3 maxPointPosition = lastRayHit.point + (finalPointDirection * _swingingRopeLenght);

                maxPointPosition = transform.position + (finalPointDirection * _swingingRopeLenght * 2 * ropeLenghtMultiplier);

                //Vector3 maxPointPosition = transform.position +  (finalPointDirection   * (_swingingRopeLenght - maxYDistance));

                if (_currentVerticalPositiveSpeed < 1)
                {
                    maxPointPosition.y = transform.position.y * flyYSpeedMultiplier * _currentHorizontalSpeed * 2.5f;
                }
                else
                {
                    maxPointPosition.y = transform.position.y * flyYSpeedMultiplier * _currentHorizontalSpeed * _currentVerticalPositiveSpeed; 
                }
                
                
                
                maxPointPosition.y = Mathf.Clamp(maxPointPosition.y, transform.position.y, maxY);

                Vector3 finalPointPosition = (maxPointPosition * flyXSpeedMultiplier * _currentHorizontalSpeed * _currentVerticalPositiveSpeed);
                
                
                //finalPointPosition.x = Mathf.Clamp(finalPointPosition.x, maxPointPosition.x, maxPointPosition.x);
                //finalPointPosition.z = Mathf.Clamp(finalPointPosition.z,  maxPointPosition.z, maxPointPosition.z);

                var startPointPosition = transform.position;
                finalPointPosition = maxPointPosition;
                

                projectileMovement.SetStartSpeed(new Vector2(_currentHorizontalSpeed , _playerVerticalVelocity));
                projectileMovement.SetHeight(_swingingRopeLenght * ropeHeightConst);
                
                
                _playerState = States.Swing;
                
                CalculateMainHand();
                projectileMovement.StartMoving(startPointPosition , finalPointPosition);
                
                StartSwingingAction?.Invoke();
                
                
                
                _isSwinging = true;
            }
        }

        private void TryToStopSwinging()
        {
            if (_isSwinging)
            {
                StopProjectileMovement?.Invoke();
                StopSwinging();
            }
        }
        
        private void StopSwinging()
        {
            if (_isSwinging)
            {
                var newSpeed = projectileMovement.GetFinalSpeed();

                _currentHorizontalSpeed = newSpeed.x * afterSwingingHorizontalSpeedMultiplier;
                _currentVerticalPositiveSpeed = MaxFlyVerticalSpeed * projectileMovement.GetFromZeroToOnePathValue();

                
                _isSwinging = false;
                
                _playerVerticalVelocity = newSpeed.y;
                
                StopSwingingAction?.Invoke();
            }
        }




        private void ControlSwingHeight()
        {
            shouldControlHeight = Physics.Raycast(groundCheckerPosition.position, Vector3.down, out var hitPoint,
                    2.5f, groundMask);

            if (shouldControlHeight)
            { 
                SetControlPositionY();
            }
            else
            {
                _canSetControlPositionY = true;
            }

            projectileMovement.UpdateTrueHeight(truePlayerY, shouldControlHeight);

            UnityEngine.Debug.DrawRay(groundCheckerPosition.position , Vector3.down * 1.5f);
        }



        private void RotateToSwingPoint()
        {
            float smoothAngle;
            if (_isSwinging)
            {
                Vector3 directionToTarget = (projectileMovement.hitPoint.position - transform.position);
               
                
                var targetAngle = Vector3.Angle(transform.up, directionToTarget);
                
                float dot = Vector3.Dot(transform.right, directionToTarget);
                
                if (dot > 0)
                {
                    targetAngle = -targetAngle;
                }

                smoothAngle = Mathf.LerpAngle(playerMesh.localRotation.eulerAngles.z, targetAngle, swingingSmoothTime);
            }
            else
            {
                smoothAngle = Mathf.LerpAngle(playerMesh.localRotation.eulerAngles.z, 0, swingingSmoothTime);
            }
            
            playerMesh.localRotation = Quaternion.Euler(0f, 0f, smoothAngle);

        }
        
        private void FlyMovement()
        {
            ControlSwingHeight();
            RotateToSwingPoint();

            if (_playerActions.Action.IsPressed() && _canSwing)
            {
                TryToStartSwinging();
            }
            
            if (_isSwinging == false)
            {

                if (_trickButtonPressed)
                {
                    turnFlySmoothValue = Mathf.Lerp(turnFlySmoothValue , turnTrickTime , 100 * Time.deltaTime);
                }
                else
                {
                    turnFlySmoothValue = Mathf.Lerp(turnFlySmoothValue , 1 , 0.7f * Time.deltaTime);
                }
                

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity,
                    turnSmoothTime);
            
                Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            
                if (_playerState != States.AirAcceleration)
                {
                    _playerHorizontalVelocity =Vector3.Lerp(_playerHorizontalVelocity , moveDir.normalized * _currentHorizontalSpeed , turnFlySmoothValue);
                }
                
                CheckWallJump();
                

                if (_currentHorizontalSpeed < MinFlyHorizontalSpeed)
                {
                    if (_inputPlayer.magnitude >= 0.1f)
                    {
                        _currentHorizontalSpeed = Mathf.Lerp(_currentHorizontalSpeed, MinFlyHorizontalSpeed, Time.deltaTime * multiplyFlySpeedTime);
                    }
                }
                else
                {
                    _currentHorizontalSpeed = Mathf.Lerp(_currentHorizontalSpeed, MinFlyHorizontalSpeed, Time.deltaTime * slowFlySpeedTime);
                }
                
                
                if (_playerState != States.Air)
                {
                    StartAirMovement?.Invoke();

                    _playerState = States.Air;
                }
                
                
            }
            
        }

        

        private void CheckWallJump()
        {
            {
                if (_wallJumpIsStarted)
                {
                    OnWallJump(); 
                }
            }
        }



        private void SetControlPositionY()
        {
            if (_canSetControlPositionY)
            {
                truePlayerY = transform.position.y;
                _canSetControlPositionY = false;
            }
        }

        private void TryToAirAccelerate()
        {
            if (IsAbleToAcceleration())
            {
                CalculateMainHand();
                StartAirAccelerating?.Invoke();
                StartCoroutine(AirAccelerationCoroutine());
            }
        }
        
        
        private bool IsAbleToAcceleration()
        {
            RaycastHit? ray = airAccelerationRays.GetTheBestRayHit();

            if (ray == null)
            {
                return false;
            }

            lastRayHit = ray.Value;

            if (ray.Value.distance < minRayDistanceToAcceleration)
            {
                return false;
            }

            if (_playerState != States.Air)
            {
                return false;
            }

            if (_isSitPointColliding)
            {
                return false;
            }

            return true;
        }

        private IEnumerator AirAccelerationCoroutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < airAccelerationTime)
            {
                
                _playerHorizontalVelocity += (_playerHorizontalVelocity.normalized * (airAccelerationHorizontalSpeed * airAccelerationSpeedMultiplier));
                _playerVerticalVelocity = airAccelerationVerticalSpeed;
                _playerState = States.AirAcceleration;
                
                elapsedTime += Time.deltaTime;
                yield return null;

            }

            _currentHorizontalSpeed = _playerHorizontalVelocity.magnitude * afterAirAccelerationMultiplier;
        }

        private void TryToCreateAirTrick()
        {
            if (_playerState == States.Air)
            {
                StartAirTrick?.Invoke();
            }
            _trickButtonPressed = true;
        }

        private void TryToStopAirTrick()
        {
            if (_playerState == States.Air)
            {
                StopAirTrick?.Invoke();
            }
            _trickButtonPressed = false;
        }
        
        
    }
    
    
    
}
