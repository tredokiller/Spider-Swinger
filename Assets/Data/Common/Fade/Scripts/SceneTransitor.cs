using UnityEngine;

namespace Data.Common.Fade.Scripts
{
    public class SceneTransitor : MonoBehaviour
    {
        public string scene;
        public Color loadToColor = Color.white;
        
        public void PlayTransitionToAnotherScene()
        {
            Initiate.Fade(scene, loadToColor, 1.0f);
        }
    }
}
