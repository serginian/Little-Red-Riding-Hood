using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Animation
{
    [CreateAssetMenu(menuName = "serginian/UI/Animation/Window Fade In-Out", fileName = "Window Fade In-Out",
        order = 0)]
    public class WindowFadeAnimation : UiWindowAnimation
    {
        [Header("Fade In")] public float showDuration = 1f;
        public Ease showEase = Ease.Linear;
        [Header("Fade Out")] public float hideDuration = 1f;
        public Ease hideEase = Ease.Linear;


        /********************** PUBLIC INTERFACE **********************/

        public override async Awaitable ShowAsync(UiWindow window)
        {
            window.CanvasGroup.DOKill();
            await window.CanvasGroup.DOFade(1f, showDuration).SetEase(showEase).AsyncWaitForCompletion();
        }

        public override async Awaitable HideAsync(UiWindow window)
        {
            window.CanvasGroup.interactable = false;
            window.CanvasGroup.DOKill();
            await window.CanvasGroup.DOFade(0f, hideDuration).SetEase(hideEase).AsyncWaitForCompletion();
        }
        
    } // end of class
}