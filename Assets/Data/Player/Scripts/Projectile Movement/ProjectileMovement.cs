using System.Collections;
using Data.Player.Scripts.Movement.Controller;
using JetBrains.Annotations;
using UnityEngine;


namespace Data.Player.Scripts.Projectile_Movement
{
    public class ProjectileMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float step = 0.1f;
        [SerializeField] private float height = 5;

        public delegate void LinearCoroutineCallback();
        public delegate void CoroutineCallback();

        [SerializeField] private float horizontalSpeedLerpTime = 1f;
        [SerializeField] private float verticalSpeedLerpTime = 1f;

        [SerializeField] private float verticalSpeedLerpTimeMultiplier; //For increase more vertical speed
        
        [SerializeField] private float speedMultiplier;

        private Vector2 _startedSpeed;

        private float _finalVerticalSpeed;

        private bool _speedIsConst;
    
        private float _startTime;

        private float _angle;

        private float _v0;

        private float _currentTime;
        private float _time;

        private Vector3 flipParabolaVector = Vector3.down;

        [SerializeField] private bool heightControl;
    
    
        private float _trueObjectY;
    
    
        private Vector3 _targetPosition;

        public Transform startPoint;
        public Transform finalPoint;
        public Transform hitPoint;
        

        private float _maxDistance;
        
        [SerializeField] private Transform movableObject;



        [Header("Debug")]
        [SerializeField] private LineRenderer _debugLine;
    

        private bool _canMove = false;

        [Header("Direction")] 
        private Vector3 _movementDirection = Vector3.zero;
        private Vector3 _movementGroundDirection = Vector3.zero;


        private void Awake()
        {
            PlayerController.StopProjectileMovement += StopMoving;
        }

        private float QuadraticEquation(float a, float b, float c, float sign)
        {
            return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        }
    

        private void CalculatePathWithHeight()
        {
            float xt = _targetPosition.x;
            float yt = _targetPosition.y;

            float g = -WorldSettings.Gravity;

            float b = Mathf.Sqrt(2 * g * height);
        
            float a = (-0.5f * g);
            float c = flipParabolaVector.magnitude * yt;
        
            float tPlus = QuadraticEquation(a, b, c, 1);
            float tMin = QuadraticEquation(a, b, c, -1);

            _time = tPlus > tMin ? tPlus : tMin;

            _angle = Mathf.Atan(b * _time / xt);

            _v0 = b / Mathf.Sin(_angle);
        }
    
        private void DrawPath(Vector3 direction , float v0 , float angle , float time , float step)
        {
            step = Mathf.Max(0.01f, step);
            _debugLine.positionCount = (int)(time / step) + 2;

            int count = 0;


            for (float i = 0; i < time; i+= step)
            {
                float x = v0 * i * Mathf.Cos(angle);
                float y = v0 * i * Mathf.Sin(angle) - 0.5f * -WorldSettings.Gravity * Mathf.Pow(i, 2);
            
                _debugLine.SetPosition(count , startPoint.position + direction * x + flipParabolaVector * y);
            
                count++;
            }

            float xfinal = v0 * time * Mathf.Cos(angle);
            float yfinal = v0 * time * Mathf.Sin(angle) - 0.5f * -WorldSettings.Gravity * Mathf.Pow(time, 2);
            _debugLine.SetPosition(count , startPoint.position + direction * xfinal + flipParabolaVector * yfinal);
        

        }
    
        private void Update()
        {
            _movementDirection = finalPoint.position - startPoint.position;
            _movementGroundDirection = new Vector3(_movementDirection.x, 0, _movementDirection.z);
        
            _targetPosition = new Vector3(_movementGroundDirection.magnitude , _movementDirection.y , 0);
            
            height = Mathf.Max(0.01f, height);

            if (_canMove)
            {
                PlayerController.StartProjectileMovement?.Invoke();
                
                _canMove = false;
                StopAllCoroutines();
                StartCoroutine(Coroutine_Movement());
            }
        }

        IEnumerator Coroutine_Movement()
        {
            _currentTime = 0;
            _finalVerticalSpeed = 0;
            
            CalculatePathWithHeight();
        
            while (_currentTime < _time)
            {
                DrawPath(_movementGroundDirection.normalized , _v0 , _angle, _time , step);
                
                float x = _v0 * _currentTime * Mathf.Cos(_angle);
                float y = (_v0 * _currentTime * Mathf.Sin(_angle) - (1f / 2f) * -WorldSettings.Gravity * Mathf.Pow(_currentTime, 2));
            
                Vector3 defaultPathCalculation = startPoint.position + _movementGroundDirection.normalized * x + flipParabolaVector * y;

                if (heightControl)
                {
                    if (defaultPathCalculation.y >= movableObject.transform.position.y)
                    {
                        movableObject.transform.position = defaultPathCalculation;  
                    }
                    else
                    {
                        movableObject.transform.position =
                            new Vector3(startPoint.position.x + _movementGroundDirection.normalized.x * x, _trueObjectY,
                                startPoint.position.z + _movementGroundDirection.normalized.z * x);
                    }
                }

                else
                {
                    if (_trueObjectY <  movableObject.transform.position.y)
                    {
                        movableObject.transform.position = defaultPathCalculation;
                    }
                    else
                    {
                        movableObject.transform.position =
                            new Vector3(startPoint.position.x + _movementGroundDirection.normalized.x * x, _trueObjectY,
                                startPoint.position.z + _movementGroundDirection.normalized.z * x);
                    }
                }
                
                if (_speedIsConst == false)
                {
                    float speedX = (_startedSpeed.x + (_startedSpeed.y * verticalSpeedLerpTimeMultiplier));
                    
                    if (flipParabolaVector != Vector3.up)
                    {
                        _finalVerticalSpeed = Mathf.Lerp(_finalVerticalSpeed, PlayerController.MaxFlyVerticalSpeed,
                            Time.deltaTime * verticalSpeedLerpTime * speedX);
                    }
                    
                    if (_currentTime <= _time / 2)
                    {
                        _startedSpeed.x = Mathf.Lerp(_startedSpeed.x, PlayerController.MaxFlyHorizontalSpeed,
                            Time.deltaTime * horizontalSpeedLerpTime * speedX);
                    }
                }

                _currentTime += Time.deltaTime * speedMultiplier * _startedSpeed.magnitude;
                
                yield return null;
            }
        
            PlayerController.StopProjectileMovement?.Invoke();
            
        }

        IEnumerator LinearCoroutine_Movement(LinearCoroutineCallback callback = null)
        {
            _currentTime = 0;

            _time = _startedSpeed.x;

            while (_currentTime < _time)
            {
                movableObject.transform.position =
                    Vector3.Lerp(startPoint.position, finalPoint.position, _currentTime / _startedSpeed.x);
                _currentTime += Time.deltaTime * _startedSpeed.magnitude;
                yield return null;
            }

            movableObject.position = finalPoint.position;
            
            if (callback != null)
            {
                callback.Invoke();
            }
            
            PlayerController.StopProjectileMovement?.Invoke();
        }
        
        public float GetFromZeroToOnePathValue()
        {
            float clampedValue = (_currentTime - 0) / (_time - 0);
            
            return clampedValue;
        }

        public void SetHeight(float newHeight)
        { 
            height = newHeight;
        }
        
        public void SetStartSpeed(Vector2 speed , bool isConst = false)
        {
            _speedIsConst = isConst;
            
            if (speed.y < 0)
            {
                speed.y *= -1f;
            }
            _startedSpeed = speed;
        }
        
    
        public void StartMoving(Vector3 startPointPosition , Vector3 finalPointPosition , Vector3 hitPointPosition = default)
        {
            startPoint.position = startPointPosition;
            finalPoint.position = finalPointPosition;

            if (hitPointPosition != default)
            {
                hitPoint.position = hitPointPosition;
            }
            
            _canMove = true;
            
            _maxDistance = Vector3.Distance(startPoint.position, finalPoint.position);

        }

        public void StartLinearMoving(Vector3 startPointPosition , Vector3 finalPointPosition , [CanBeNull] LinearCoroutineCallback callback = null)
        {
            startPoint.position = startPointPosition;
            finalPoint.position = finalPointPosition;
            
            StopAllCoroutines();
            
            PlayerController.StartProjectileMovement?.Invoke();
            StartCoroutine(LinearCoroutine_Movement(callback));
            
        }


        public void UpdateTrueHeight(float trueHeight , bool shouldHeightControl)
        {
            _trueObjectY = trueHeight;
            heightControl = shouldHeightControl;
        }


        private void StopMoving()
        {
            _canMove = false;
            StopAllCoroutines();
        }


        public Vector2 GetFinalSpeed()
        {
            return new Vector2(_startedSpeed.x , _finalVerticalSpeed);
        }


        public void FlipParabolaVertical(bool flip)
        {
            if (flip)
            {
                flipParabolaVector = Vector3.down;
                return;
            }
            flipParabolaVector = Vector3.up;
        }
    }
}
