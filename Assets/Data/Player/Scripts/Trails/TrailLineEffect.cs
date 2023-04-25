using Data.Player.Scripts.Movement.Controller;
using UnityEngine;

namespace Data.Player.Scripts.Trails
{
    public class TrailLineEffect : MonoBehaviour
    {
        private TrailRenderer _trail;
        
        
        private readonly float _widthMultiplier = 0.1f;
        private readonly float _time = 0.1f;
        
        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            
            PlayerController.StartAirMovement += EnableTrail;
            PlayerController.StartGroundMovement += DisableTrail;
            
            ApplyTrailParameters();
        }
        

        private void ApplyTrailParameters()
        {
            _trail.widthMultiplier = _widthMultiplier;
            _trail.time = _time;
        }

        private void EnableTrail()
        {
            _trail.emitting = true;
        }

        private void DisableTrail()
        {
            _trail.emitting = false; 
        }
    }
}
