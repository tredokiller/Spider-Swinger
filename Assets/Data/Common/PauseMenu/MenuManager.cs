using System;
using Data.Common.UITransitions;
using Data.Player.Scripts.Movement;
using Data.Settings;
using JetBrains.Annotations;
using UnityEngine;

namespace Data.Common.PauseMenu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField, CanBeNull] private OneShotTransition toVisibleTransition;
        [SerializeField, CanBeNull] private OneShotTransition toInvisibleTransition;

        [SerializeField, CanBeNull] private InputManager inputManager;
        [SerializeField] private GameObject menuElements;

        public static Action OnMenuOpened;
        public static Action OnMenuClosed;
        
        private GameInput.UIActions _uiActions;

        private bool _menuIsActive;

        private void Awake()
        {
            _menuIsActive = false;
        }


        private void Start()
        {
            if (inputManager != null)
            {
                _uiActions = inputManager.GetUIActions();
                _uiActions.ESC.started += context => SetActiveMenu();
            }
            menuElements.SetActive(false);
        }
        

        public void SetActiveMenu()
        {
            _menuIsActive = !_menuIsActive;
            menuElements.SetActive(_menuIsActive);

            if (_menuIsActive)
            {
                OnMenuOpened.Invoke();
            }
            else
            {
                OnMenuClosed.Invoke();
            }
            
            PlayTransitions();
        }

        private void PlayTransitions()
        {
            if (_menuIsActive)
            {
                if (toVisibleTransition != null)
                {
                    toVisibleTransition.MakeTransitionFrom();
                }
                return;
            }
            if (toInvisibleTransition != null)
            {
                toInvisibleTransition.MakeTransitionTo();
            }
        }

    }
}
