using Data.Common.Enums;
using Data.Player.Scripts.Movement.Controller.Enums;
using UnityEngine;

namespace Data.Player.Scripts.Movement.Controller
{
    partial class PlayerController : MonoBehaviour
    {
        private void SetGroundSpeed()
        {
            if (_walkButtonPressed)
            {
                _groundSpeed = GroundSpeed.Walk;
            }
            else if (_runButtonPressed)
            {
                _groundSpeed = GroundSpeed.Run;
            }
            else
            {
                _groundSpeed = GroundSpeed.Jog;
            }

            if (_inputPlayer.magnitude <= 0.1f)
            {
                _currentHorizontalSpeed = 0;
                _playerState = States.Idle;
            }
            else
            {
                switch (_groundSpeed)
                {
                    case GroundSpeed.Jog:
                        _playerState = States.Jog;
                        _currentHorizontalSpeed = _jogSpeed;
                        break;

                    case GroundSpeed.Walk:
                        _currentHorizontalSpeed = _walkSpeed;
                        _playerState = States.Walk;
                        break;

                    case GroundSpeed.Run:
                        _currentHorizontalSpeed = _runSpeed;
                        _playerState = States.Run;
                        break;
                }
            }
        }
           
        private void GroundMovement()
        {
            SetGroundSpeed();
            
            if (_isCustomSpeed)
            {
                _playerHorizontalVelocity = _direction.normalized * _customSpeed;
            }
            
            else if (_inputPlayer.magnitude >= 0.1f)
            {
                _playerHorizontalVelocity = Vector3.Lerp(_playerHorizontalVelocity,
                    _direction.normalized * _currentHorizontalSpeed, smoothSpeedUp);
            }
            else
            {
                _playerHorizontalVelocity = Vector3.zero;
            }
        }
        
        private void OnJump()
        {
            _jumpButtonPressed = true;
        }


        private void CancelJump()
        {
            _jumpButtonPressed = false;
        }
        
        private void Jump()
        {
            _playerState = States.Jump;
            _playerVerticalVelocity = Mathf.Sqrt(jumpHeight * -1f * WorldSettings.Gravity);
        }
        
    }
}
