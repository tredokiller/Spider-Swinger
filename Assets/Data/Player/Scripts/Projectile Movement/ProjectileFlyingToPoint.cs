using System;
using System.Collections.Generic;
using System.Linq;
using Data.Buildings.SitPoint;
using Data.Buildings.SitPoint.Scripts;
using Data.Player.Scripts.Movement.Controller;
using UnityEngine;


namespace Data.Player.Scripts.Projectile_Movement
{
    [RequireComponent(typeof(PlayerController))]
    public class ProjectileFlyingToPoint : MonoBehaviour
    {
        private PlayerController _playerController;
        private GameInput.PlayerActions _playerActions; 
            
        private ProjectileMovement _projectileMovement;

        [SerializeField] private float flySpeed = 35f;
        [SerializeField] private float smoothRotationTime = 10f;
        [SerializeField] private LayerMask obstacleLayers;

        private const float MinDistanceToFly = 15f;
        private const float MaxDistanceToFly = 50f;

        private static List<SitPoint> _points = new List<SitPoint>();
        private SitPoint _mainPoint;
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerActions = _playerController.GetPlayerButtonActions();
            
            _projectileMovement = _playerController.GetProjectileMovement();
        }


        private void OnEnable()
        {
            _playerActions.Zip.started += context => TryToFlyToPoint();
        }

        private void OnDisable()
        {
            _playerActions.Zip.started -= context => TryToFlyToPoint();
        }

        private void Update()
        {
            _mainPoint = CalculateTheBestArrivePoint();
        }

        private void TryToFlyToPoint()
        {
            if (CanFlyToPoint())
            {
                var mainPointType = _mainPoint.GetSitPointType();
                
                if (mainPointType == SitPointType.RollPoint)
                {
                    PlayerController.StartFlyToSitPointRoll.Invoke();
                }
                else
                {
                    PlayerController.StartFlyToSitPointDefault.Invoke();
                }
                PlayerController.StartProjectileFlyToSitPoint?.Invoke();
                
                _projectileMovement.SetStartSpeed(new Vector2(flySpeed, flySpeed) , true);
                _projectileMovement.StartLinearMoving(transform.position, _mainPoint.GetArrivePointPosition() , OnProjectileMovementToPointCompleted);
            }   
        }

        private void OnProjectileMovementToPointCompleted()
        {
            PlayerController.FinishProjectileFlyToSitPoint.Invoke();
        }

        private SitPoint CalculateTheBestArrivePoint()
        {
            if (_points.Count > 0)
            {
                List<SitPoint> preferedPoints= new List<SitPoint>();
                List<float> preferedPointDistances = new List<float>();
                foreach (var point in _points)
                {
                    float distance = Vector3.Distance(point.transform.position, transform.position);

                    if (distance > MaxDistanceToFly || distance < MinDistanceToFly)
                    {
                        continue;
                    }
                    
                    Vector3 directionBetweenPlayerAndPoint = (point.transform.position - transform.position).normalized;

                    bool hasAnyObstacles = Physics.Raycast(transform.position, directionBetweenPlayerAndPoint, distance , obstacleLayers);

                    if (hasAnyObstacles)
                    {
                        continue;
                    }
                    
                    preferedPointDistances.Add(distance);
                    preferedPoints.Add(point);
                }
                if (preferedPoints.Count > 0)
                {
                    int indexOfBestPoint =  preferedPointDistances.IndexOf(preferedPointDistances.Min());

                    SitPoint bestPoint = preferedPoints[indexOfBestPoint];

                    return bestPoint;
                }
                return null;
            }
            return null;
        }

        
        public SitPoint GetMainSitPoint()
        {
            return _mainPoint;
        }

        private bool CanFlyToPoint()
        {
            if (_playerController.GetIsProjectileMovement())
            {
                return false; 
            }
            if (_mainPoint == null)
            {
                return false;
            }
            return true;
        }
        

        public static void AddPoint(SitPoint point)
        {
            _points.Add(point);
        }
        
        public static void RemovePoint(SitPoint point)
        {
            _points.Remove(point);
        }
    }
}
