using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Data.Common.SceneLoader.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Settings.Scenes scene;
        
        private CanvasGroup _canvasGroup;
        private Image _color;
        
        [SerializeField] private float transitionDuration;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _color = GetComponent<Image>();
            
            _canvasGroup.enabled = true;
            _canvasGroup.alpha = 1;

            _color.enabled = true;
        }

        private void Start()
        {
            PlayStartingSceneTransition();
        }

        private void PlayStartingSceneTransition()
        {
            Fade.FadeOut(_canvasGroup, transitionDuration);
        }
        
        public void PlayToAnotherSceneTransition()
        {
            Fade.FadeTo(_canvasGroup, transitionDuration , LoadTheScene);
        }

        private void LoadTheScene()
        {
            SceneManager.LoadScene(scene.ToString());
            Settings.GameSettings.OnSceneChanged.Invoke(scene.ToString());
        }
    }
}
