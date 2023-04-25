using Data.Player.Scripts.Movement.Controller;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace Data.Player.Scripts.Movement.AnimationController
{
    public class IKManager : MonoBehaviour
    {
        [SerializeField] private Rig rig;
        [SerializeField] private float smoothWeightLerpTime = 0.04f;
        
        
        private PlayerController _controller;


        private bool _rigIsEnabled;
        
        
        private void Awake()
        {
           // DOTween.Init(false, true, LogBehaviour.Default);
           PlayerController.StartSwingingAction += () => SetRigEnable(0);
           PlayerController.StopProjectileMovement += () => SetRigEnable(1);
           SetRigEnable(1);
        }
        
        
        public void SetRigEnable(int isEnable)
        {
            _rigIsEnabled = (isEnable != 0);
            CheckAndApplyWeightTween();
        }


        private void CheckAndApplyWeightTween()
        {
            if (_rigIsEnabled)
            {
                DOTween.To(() => rig.weight, x => rig.weight = x, 1f, smoothWeightLerpTime);
            }
            else
            {
                DOTween.To(() => rig.weight, x => rig.weight = x, 0f, smoothWeightLerpTime);
            }
        }
    }
}