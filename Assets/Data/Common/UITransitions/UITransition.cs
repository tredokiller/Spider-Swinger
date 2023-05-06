using DG.Tweening;
using UnityEngine;

namespace Data.Common.UITransitions
{
    public class UITransition : MonoBehaviour
    {
        public static void MakeTransitionFromTo(RectTransform transitionObjectRect, RectTransform startPoint , RectTransform finishPoint , float duration = 1f)
        {
            transitionObjectRect.transform.position = startPoint.position;
            transitionObjectRect.transform.DOMove(finishPoint.position, duration).SetEase(Ease.InOutBack);
        }
        
        public static void MakeTransitionFrom(RectTransform transitionObjectRect, RectTransform startPoint , float duration = 1f)
        {
            Vector3 finishPosition = transitionObjectRect.position;
            transitionObjectRect.position = startPoint.position;

            transitionObjectRect.transform.DOMove(finishPosition, duration).SetEase(Ease.InOutBack);
        }
        
        public static void MakeTransitionTo(RectTransform transitionObjectRect, RectTransform finalPoint , float duration = 1f)
        {
            transitionObjectRect.transform.DOMove(finalPoint.position, duration).SetEase(Ease.InOutBack);
        }
    }
}
