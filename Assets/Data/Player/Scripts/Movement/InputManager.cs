using Data.Common.PauseMenu;
using UnityEngine;

namespace Data.Player.Scripts.Movement
{
    public class InputManager : MonoBehaviour
    {
        private GameInput _gameInput;
        
        private GameInput.PlayerActions _playerActions;
        private GameInput.UIActions _uIActions;

        private void Awake()
        {
            _gameInput = new GameInput();

            _playerActions = _gameInput.Player;
            _uIActions = _gameInput.UI;
        }

        private void OnEnable()
        {
            _gameInput.Enable();

            MenuManager.OnMenuOpened += () => _playerActions.Disable();
            MenuManager.OnMenuClosed += () => _playerActions.Enable();
        }
        
        private void OnDisable()
        {
            _gameInput.Disable();
            
            MenuManager.OnMenuOpened -= () => _playerActions.Disable();
            MenuManager.OnMenuClosed -= () => _playerActions.Enable();
        }

        public GameInput.PlayerActions GetPlayerActions()
        {
            return _playerActions;
        }
        
        public GameInput.UIActions GetUIActions()
        {
            return _uIActions;
        }
    }
}
