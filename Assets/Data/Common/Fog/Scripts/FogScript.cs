using Data.Common.Fade.Scripts;
using Data.Player.Scripts.Movement.Controller;
using UnityEngine;
namespace Data.Common.Fog.Scripts
{
    public class FogScript : MonoBehaviour
    {
        [SerializeField] private SceneTransitor sceneTransitor;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerController.PlayerTag))
            {
                sceneTransitor.PlayTransitionToAnotherScene();
            }
            
        }
    }
}
