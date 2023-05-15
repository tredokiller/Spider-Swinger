using Data.Common.Button;
using Data.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Data.Scenes.Menu.Buttons
{
    public class SoundButton : ButtonBase
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        [SerializeField] private Color soundsOnColor;
        [SerializeField] private Color soundsOffColor;

        private const string SoundsOnText = "SFX: ON";
        private const string SoundsOffText = "SFX: OFF";
        
        private void Start()
        {
            ApplySfxChanges();
        }

        private void OnEnable()
        {
            GameSettings.OnSfxChanged += ApplySfxChanges;
        }

        private void OnDisable()
        {
            GameSettings.OnSfxChanged -= ApplySfxChanges;
        }

        public void ButtonPressed()
        {
            GameSettings.OnSfxChanged.Invoke();
        }
        
        private void ApplySfxChanges()
        {
            if (GameSettings.Instance.IsSfxOn)
            { 
                buttonImage.color = soundsOnColor;
                buttonText.text = SoundsOnText;
                return;
            }
            buttonImage.color = soundsOffColor;
            buttonText.text = SoundsOffText;
        }
    }
}
