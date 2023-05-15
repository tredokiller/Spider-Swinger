using Data.Common.SceneLoader.Scripts;
using Data.Player.Scripts.Movement;
using Data.Player.Scripts.Movement.Controller;
using UnityEngine;
namespace Data.Common.Respawn
{
    public class RespawnZone : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private CanvasGroup canvasGroup;
        

        private const float TransitionDuration = 0.5f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerController.PlayerTag))
            {
                StartRespawn();
            }
        }

        private void StartRespawn()
        {
            playerManager.SetEnableController(false);
            Fade.FadeTo(canvasGroup, TransitionDuration , FinishRespawn);
        }

        private void FinishRespawn()
        {
            playerManager.SpawnOnStartPlace();
            playerManager.SetEnableController(true);
            Fade.FadeOut(canvasGroup, TransitionDuration);
        }
        


    }
}
