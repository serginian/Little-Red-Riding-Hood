using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Animation
{
    public enum AnimationDirection
    {
        LeftToRight,
        RightToLeft,
        None
    }

    [CreateAssetMenu(menuName = "serginian/UI/Animation/Window Slide In-Out", fileName = "Window Slide In-Out",
        order = 0)]
    public class WindowSlideAnimation : WindowFadeAnimation
    {
        [Header("Slide In")] public float slideDuration = 1f;
        public Ease slideEase = Ease.OutCirc;
        public AnimationDirection direction = AnimationDirection.RightToLeft;

        public override async Awaitable ShowAsync(UiWindow window)
        {
            window.RectTransform.DOKill();

            float startPos = 0f;
            switch (direction)
            {
                case AnimationDirection.LeftToRight: startPos = -window.Size.x; break;
                case AnimationDirection.RightToLeft: startPos = window.Size.x; break;
            }

            window.RectTransform.anchoredPosition = new Vector2(startPos, 0);
            await base.ShowAsync(window);
            await window.RectTransform.DOAnchorPosX(0f, slideDuration).SetEase(slideEase).AsyncWaitForCompletion();
        }

        public override async Awaitable HideAsync(UiWindow window)
        {
            window.RectTransform.DOKill();
            float targetPos = 0f;
            switch (direction)
            {
                case AnimationDirection.LeftToRight: targetPos = window.Size.x; break;
                case AnimationDirection.RightToLeft: targetPos = -window.Size.x; break;
            }

            await window.RectTransform.DOAnchorPosX(targetPos, slideDuration).SetEase(slideEase).AsyncWaitForCompletion();
            await base.HideAsync(window);
        }
        
    } // end of class
}