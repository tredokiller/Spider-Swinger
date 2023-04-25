using Data.Player.Scripts.Movement.Controller;
using Data.Player.Scripts.Projectile_Movement;
using TMPro;
using UnityEngine;

namespace Data.Player.Debug
{
    public class PlayerStatsDebug : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI horizontalSpeedTextObj;
        [SerializeField] private TextMeshProUGUI verticalSpeedTextObj;
        [SerializeField] private TextMeshProUGUI swingingClampedAnimationTextObj;

        [SerializeField] private TextMeshProUGUI currentPlayerStateTextObj;
        
        [SerializeField] private TextMeshProUGUI distanceToSwingPointTextObj;
        
        
        [SerializeField] private PlayerController controller;
        private ProjectileMovement _projectileMovement;

        private const string HorizontalSpeedText = "Horizontal Speed: ";
        private const string VerticalSpeedText = "Vertical Speed: ";
        
        private const string SwingPointDistanceText = "Distance To Swing Point: ";

        private const string SwingingClampedAnimationText = "Swinging Clamped Animation Value: ";
        
        private const string CurrentPlayerStateText = "Current State: ";


        private void Start()
        {
            _projectileMovement = controller.GetProjectileMovement();
        }

        private void Update()
        {
            horizontalSpeedTextObj.text = HorizontalSpeedText + controller.GetPlayerCurrentHorizontalSpeed();
            verticalSpeedTextObj.text = VerticalSpeedText + controller.GetPlayerCurrentVerticalSpeed();
            swingingClampedAnimationTextObj.text = SwingingClampedAnimationText + _projectileMovement.GetFromZeroToOnePathValue();
            currentPlayerStateTextObj.text = CurrentPlayerStateText + controller.GetPlayerCurrentState();

            distanceToSwingPointTextObj.text = SwingPointDistanceText + controller.GetPlayerSwingingRopeLenght();
        }
        
    }
    
    
}
