using System;
using Data.Common.PauseMenu;
using UnityEngine;
using UnityEngine.Audio;

namespace Data.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private AudioMixer mainAudioMixer;

        [SerializeField] private float minVolumeDb = -80;
        [SerializeField] private float maxVolumeDb = 0;
        
        public bool IsSfxOn { get; private set; }
        
        public static Action OnSfxChanged;
        public static Action<string> OnSceneChanged;
        
        private static GameSettings _instance;
    
        public static GameSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameSettings>("Settings");
                }
                return _instance;
            }
        }
        
        private void AutoChangeCursorState(string scene)
        {
            if (scene == Scenes.Menu.ToString())
            {
                EnableCursor();
                return;
            }
            DisableCursor();
        }
        
        private void OnEnable()
        {
            SetSFXOn(true);
            
            OnSceneChanged += AutoChangeCursorState;
            OnSfxChanged += () =>  SetSFXOn(!IsSfxOn);

            MenuManager.OnMenuOpened += EnableCursor;
            MenuManager.OnMenuClosed += DisableCursor;
        }
        
        
        
        private void OnDisable()
        {
            OnSceneChanged -= AutoChangeCursorState;
            OnSfxChanged -= () =>  SetSFXOn(!IsSfxOn);
            
            MenuManager.OnMenuOpened -= EnableCursor;
            MenuManager.OnMenuClosed -= DisableCursor;
        }

        private void SetSFXOn(bool isOn)
        {
            if (isOn)
            {
                mainAudioMixer.SetFloat("Master" , maxVolumeDb);
                IsSfxOn = true;
                return;
            }
            mainAudioMixer.SetFloat("Master" , minVolumeDb);
            IsSfxOn = false;
        }
        
        private void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        
    }
}
