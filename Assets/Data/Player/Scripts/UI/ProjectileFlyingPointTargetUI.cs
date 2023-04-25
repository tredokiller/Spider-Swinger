using Data.Buildings.SitPoint;
using Data.Buildings.SitPoint.Scripts;
using Data.Player.Scripts.Projectile_Movement;
using UnityEngine;
using UnityEngine.UI;

namespace Data.Player.Scripts.UI
{
    public class ProjectileFlyingPointTargetUI : MonoBehaviour
    {
        [SerializeField] private Image targetSprite;
        [SerializeField] private ProjectileFlyingToPoint projectileFlyingToPoint;

        private Camera _camera;
        private SitPoint _sitPoint;

        private void Awake()
        {
            _camera = Camera.main;
        }
        
        
        void Update()
        {
            UpdateMainSitPoint();
            
            if (_sitPoint != null)
            {
                targetSprite.enabled = true;
                targetSprite.transform.position = _camera.WorldToScreenPoint(_sitPoint.transform.position);
            }
            else
            {
                targetSprite.enabled = false;
            }
        }
        
        private void UpdateMainSitPoint()
        {
            _sitPoint = projectileFlyingToPoint.GetMainSitPoint();
        }
    }
}
