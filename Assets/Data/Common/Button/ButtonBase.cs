using UnityEngine;

namespace Data.Common.Button
{
    [RequireComponent(typeof(RectTransform))]
    public class ButtonBase : MonoBehaviour
    {
        private RectTransform _buttonRect;
        protected UnityEngine.UI.Button Button;
        
        private Vector3 _defaultScale;
        private Vector3 _selectedScale;

        private const float ScaleMultiplier = 1.15f;
        private const float ScaleSpeed = 5f;

        private void Awake()
        {
            _buttonRect = GetComponent<RectTransform>();
            Button = GetComponent<UnityEngine.UI.Button>();

            _defaultScale = _buttonRect.localScale;
            _selectedScale = _defaultScale * ScaleMultiplier;
        }

        protected void Update()
        {
            SmoothSelect();
        }
        
        private void SmoothSelect()
        {
            if (IsMouseInButtonZone())
            {
                _buttonRect.localScale =
                    Vector3.Slerp(_buttonRect.localScale, _selectedScale, Time.deltaTime * ScaleSpeed);
            }
            _buttonRect.localScale =
                Vector3.Slerp(_buttonRect.localScale, _defaultScale, Time.deltaTime * ScaleSpeed);
        }
        

        private bool IsMouseInButtonZone()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_buttonRect, Input.mousePosition))
            {
                return true;
            }

            return false;
        }
    }
}
