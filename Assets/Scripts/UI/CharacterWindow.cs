using System;
using DG.Tweening;
using serginian.Data;
using UnityEngine;
using UnityEngine.UI;

namespace serginian.UI
{
    public class CharacterWindow : UiWindow
    {
        [Header("Settings")]
        [SerializeField] private float fadeInTime = 0.5f;
        [SerializeField] private Ease fadeInEasing = Ease.OutQuad;
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private Ease fadeOutEasing = Ease.InOutBounce;
        
        [Header("References")]
        [SerializeField] private Image leftImage;
        [SerializeField] private Image rightImage;

        private bool isLeftCharacterVisible = false;
        private bool isRightCharacterVisible = false;

        
        
        /******************** MONO BEHAVIOR ********************/
        
        protected override void Start()
        {
            ShowWithoutAnimation();
        }

        
        /****************** PUBLIC INTERFACE ******************/
        
        public override async Awaitable ShowAsync()
        {
            // suspend default behaviour
        }

        public override async Awaitable CloseAsync()
        {
            // suspend default behaviour
        }

        public Awaitable ShowCharacterAsync(Character character, CharacterPosition position)
        {
            switch (position)
            {
                case CharacterPosition.Left: return ShowLeftCharacterAsync(character); break;
                case CharacterPosition.Right: return ShowRightCharacterAsync(character); break;
                default: throw new NotImplementedException("Position of the character is not implemented: " + position + "");
            }
        }

        public void HideCharacters()
        {
            _ = HideLeftCharacterAsync();
            _ = HideRightCharacterAsync();
        }

        
        /******************** INNER LOGIC ********************/
        
        private async Awaitable ShowLeftCharacterAsync(Character character)
        {
            if (leftImage.sprite == character.sprite && isLeftCharacterVisible)
                return;
            
            if (isLeftCharacterVisible)
                await HideLeftCharacterAsync();
            
            leftImage.sprite = character.sprite;
            leftImage.DOKill();
            await leftImage.DOFade(1f, fadeInTime).SetEase(fadeInEasing).AsyncWaitForCompletion();
            isLeftCharacterVisible = true;
        }

        private async Awaitable ShowRightCharacterAsync(Character character)
        {
            if (rightImage.sprite == character.sprite && isRightCharacterVisible)
                return;
            
            if (isRightCharacterVisible)
                await HideRightCharacterAsync();
    
            rightImage.sprite = character.sprite;
            rightImage.DOKill();
            await rightImage.DOFade(1f, fadeInTime).SetEase(fadeInEasing).AsyncWaitForCompletion();
            isRightCharacterVisible = true;
        }

        private async Awaitable HideLeftCharacterAsync()
        {
            leftImage.DOKill();
            await leftImage.DOFade(0f, fadeOutTime).SetEase(fadeOutEasing).AsyncWaitForCompletion();
            isLeftCharacterVisible = false;
        }
        
        private async Awaitable HideRightCharacterAsync()
        {
            rightImage.DOKill();
            await rightImage.DOFade(0f, fadeOutTime).SetEase(fadeOutEasing).AsyncWaitForCompletion();
            isRightCharacterVisible = false;
        }
        
    } // end of class
}