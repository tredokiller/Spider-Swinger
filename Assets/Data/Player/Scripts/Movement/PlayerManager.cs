using Data.Player.Scripts.Movement.AnimationController;
using Data.Player.Scripts.Movement.Controller;
using Data.Player.Scripts.Projectile_Movement;
using UnityEngine;

namespace Data.Player.Scripts.Movement
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private bool sitStateAtTheBeginning;
        [SerializeField] private bool spawnOnStartPosition;
        
        [SerializeField] private Transform startPlayerPosition;

        private PlayerController _controller;
        private ProjectileMovement _projectileMovement;
        private GameInput.PlayerActions _playerActions;


        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _projectileMovement = _controller.GetProjectileMovement();
            _playerActions = _controller.GetPlayerButtonActions();
        }

        private void Start()
        {
            ApplyStartSettings();
        }
        
        private void ApplyStartSettings()
        {
            if (spawnOnStartPosition)
            {
                SpawnOnStartPlace();
            }
        }

        public void SetEnableController(bool isEnabled)
        {
            _controller.enabled = isEnabled;
            _projectileMovement.enabled = isEnabled;
            if (isEnabled)
            {
                _playerActions.Enable();
                return;
            }
            
            _playerActions = _controller.GetPlayerButtonActions();
            _playerActions.Disable();
            
            
        }

        public void SpawnOnStartPlace()
        {
            _controller.ResetToDefault();
            
            Vector3 startPosition = startPlayerPosition.position;
            startPosition.y += PlayerController.PlayerHeight / 2;
            
            StopAllCoroutines();
            _controller.transform.position = startPosition;
            
            if (sitStateAtTheBeginning)
            {
                PlayerController.StopProjectileFlyToSitPoint.Invoke();
                PlayerAnimationController.StartImmediatelySitOnPoint.Invoke();
                PlayerController.StopProjectileFlyToSitPoint.Invoke();
            }
        }

        
    }
}
