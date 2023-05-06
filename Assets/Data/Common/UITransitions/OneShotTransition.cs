using UnityEngine;

namespace Data.Common.UITransitions
{
    public class OneShotTransition : MonoBehaviour
    {
        [SerializeField] private RectTransform point;
        [SerializeField] private RectTransform transitionUIElementRect;

        [SerializeField] private bool isStartTransition = true;
        [SerializeField] private float transitionDuration = 1f;
        
        private void Start()
        {
            if (isStartTransition)
            {
                MakeTransitionFrom();
            }
        }

        public void MakeTransitionFrom()
        {
            UITransition.MakeTransitionFrom(transitionUIElementRect, point, transitionDuration);
        }
        
        public void MakeTransitionTo()
        {
            UITransition.MakeTransitionTo(transitionUIElementRect, point, transitionDuration);
        }
    }
}
