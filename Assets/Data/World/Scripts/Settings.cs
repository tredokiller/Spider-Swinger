using UnityEngine;

namespace Data.Game.Scripts
{
    public class Settings : MonoBehaviour
    {
        private void Awake()
        {
            SetCursor();
        }
        
        private void SetCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
