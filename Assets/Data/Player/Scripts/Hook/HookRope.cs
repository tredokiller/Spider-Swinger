using Data.Player.Scripts.Movement.Controller;
using Data.Player.Scripts.Movement.Controller.Enums;
using Data.Player.Scripts.Projectile_Movement;
using UnityEngine;

namespace Data.Player.Scripts.Hook
{
    public class HookRope : MonoBehaviour {
        private Spring spring;
        private LineRenderer lr;
        private Vector3 currentGrapplePosition;

        private ProjectileMovement _projectileMovement;
        private PlayerController _controller;
        
        public int quality;
        public float damper;
        public float strength;
        public float velocity;
        public float waveCount;
        public float waveHeight;
        public AnimationCurve affectCurve;
    
        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _projectileMovement = _controller.GetProjectileMovement();
            lr = GetComponent<LineRenderer>();
            spring = new Spring();
            spring.SetTarget(0);
        }
    
        //Called after Update
        void LateUpdate() {
            DrawRope();
        }

        void DrawRope() {
            //If not grappling, don't draw rope
            if (_controller.GetPlayerCurrentState() != States.Swing && _controller.GetPlayerCurrentState() != States.AirAcceleration) { 
                currentGrapplePosition = _controller.rightHandTransform.position;
                spring.Reset();
                if (lr.positionCount > 0)
                    lr.positionCount = 0;
                return;
            }

            if (lr.positionCount == 0) {
                spring.SetVelocity(velocity);
                lr.positionCount = quality + 1;
            }
            
            
        
            spring.SetDamper(damper);
            spring.SetStrength(strength);
            spring.Update(Time.deltaTime);

            var grapplePoint = _controller.GetLastRayHit().point;
            var gunTipPosition = _controller.GetMainHandTransform().position;
            var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

            for (var i = 0; i < quality + 1; i++) {
                var delta = i / (float) quality;
                var offset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta));
            
                lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
            }
        }
    }
}