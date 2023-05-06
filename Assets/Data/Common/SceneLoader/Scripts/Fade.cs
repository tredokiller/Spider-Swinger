using DG.Tweening;
using UnityEngine;

namespace Data.Common.SceneLoader.Scripts
{
    public class Fade : MonoBehaviour
    {
        public delegate void FadeToFinished();
        public delegate void FadeOutFinished();

        public static void FadeTo(CanvasGroup canvas, float duration , FadeToFinished callback = null)
        {
            if (callback != null)
            {
                canvas.DOFade(1, duration).OnComplete(() => callback());
            }
            else
            {
                canvas.DOFade(1, duration);
            }
        }
        
        public static void FadeOut(CanvasGroup canvas, float duration , FadeOutFinished callback = null)
        {
            if (callback != null)
            {
                canvas.DOFade(0, duration).OnComplete(() => callback());
                return;
            }
            canvas.DOFade(0, duration); 
        }
    }
}
