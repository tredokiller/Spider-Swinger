using DG.Tweening;
using UnityEngine;

namespace Data.Common.Timer
{
    public class Timer : MonoBehaviour
    {
        public delegate void TimerFinished();
        
        public static void StartTimer(float duration, TimerFinished callback)
        {
            float timer = 0f;
            
            DOTween.To(() => timer, x => timer = x, 1f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => callback());
        }
    }
}
