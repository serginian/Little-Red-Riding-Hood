using DG.Tweening;
using TMPro;
using UnityEngine;

namespace serginian.UI
{
    public class DialogueWindow : UiWindow
    {
        [Header("Settings")]
        [SerializeField] private float fadeInTime = 0.5f;
        [SerializeField] private Ease fadeInEasing = Ease.OutQuad;
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private Ease fadeOutEasing = Ease.InOutBounce;
        
        [Header("References")]
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private TextMeshProUGUI nameLabel;
        
        private bool _isNameVisible = false;
        private bool _isTextVisible = false;

        protected override void Awake()
        {
            base.Awake();
            
            textLabel.DOFade(0f, 0f).Complete();
            nameLabel.DOFade(0f, 0f).Complete();
        }


        /****************** PUBLIC INTERFACE ******************/
        
        public async Awaitable ShowTextAsync(string text)
        {
            if (!IsVisible)
                await ShowAsync();
            
            if (_isTextVisible)
                await HideTextAsync();
            
            textLabel.text = text;
            textLabel.DOKill();
            await textLabel.DOFade(1f, fadeInTime).SetEase(fadeInEasing).AsyncWaitForCompletion();
        }

        public async Awaitable ShowNameAsync(string name, Color color)
        {
            if (_isNameVisible)
                await HideNameAsync();
            
            nameLabel.text = name;
            nameLabel.color = color;
            nameLabel.DOKill();
            await nameLabel.DOFade(1f, fadeInTime).SetEase(fadeInEasing).AsyncWaitForCompletion();
        }

        public async Awaitable HideTextAsync()
        {
            textLabel.DOKill();
            await textLabel.DOFade(0f, fadeOutTime).SetEase(fadeOutEasing).AsyncWaitForCompletion();
            _isTextVisible = false;
        }

        public async Awaitable HideNameAsync()
        {
            nameLabel.DOKill();
            await nameLabel.DOFade(0f, fadeOutTime).SetEase(fadeOutEasing).AsyncWaitForCompletion();
            _isNameVisible = false;
        }
        
    } // end of class
}