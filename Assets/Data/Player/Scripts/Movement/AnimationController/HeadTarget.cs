using Data.Player_Camera.Scripts;
using UnityEngine;

using Camera = UnityEngine.Camera;

namespace Data.Player.Scripts.Movement.AnimationController
{
    public class HeadTarget : MonoBehaviour
    {
        private Camera _camera;
        private CameraSettings _cameraSettings;

        [SerializeField] private float smoothValue;


        private void Start()
        {
            _camera = Camera.main;
            if (_camera != null) _cameraSettings = _camera.GetComponent<CameraSettings>();
        }


        private void Update()
        {
            SmoothHeadTargetTransform();
        }

        private void SmoothHeadTargetTransform()
        {
            if (_cameraSettings)
            {
                transform.position = Vector3.Lerp(transform.position, _cameraSettings.GetHeadTarget().position,
                    smoothValue);
            }
        }
    }
}
