using Data.Player.Scripts.Movement.Controller;
using Data.Player.Scripts.Projectile_Movement;
using UnityEngine;
namespace Data.Buildings.SitPoint.Scripts
{
    public class SitPoint : MonoBehaviour
    {
        [SerializeField] private Transform pointToArrive;
        [SerializeField] private SitPointType pointType;
        
        private Vector3 positionToArriveWithPlayerHeight;

        private void Awake()
        {
            positionToArriveWithPlayerHeight = pointToArrive.transform.position;
            positionToArriveWithPlayerHeight.y += PlayerController.PlayerHeight / 2;

        }
        public Vector3 GetArrivePointPosition()
        {
            return positionToArriveWithPlayerHeight;
        }

        public SitPointType GetSitPointType()
        {
            return pointType;
        }
        
        public void OnBecameVisible()
        {
            ProjectileFlyingToPoint.AddPoint(this);
        }
        
        public void OnBecameInvisible()
        {
            ProjectileFlyingToPoint.RemovePoint(this);
        }
    }
}
