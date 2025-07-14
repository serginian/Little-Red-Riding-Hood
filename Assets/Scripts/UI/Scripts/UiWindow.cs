using serginian.UI.Animation;
using UnityEngine;
using UnityEngine.Events;

namespace serginian.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UiWindow : MonoBehaviour
    {
        public bool isInteractable = true;
        public UiWindowAnimation animationAsset;
        public CanvasGroup CanvasGroup => canvasGroup;
        public RectTransform RectTransform => rectTransform;
        public Vector2 Size => new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        public bool IsVisible { get; protected set; }

        public event UnityAction<UiWindow> OnRelease;

        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;


        /********************** MONO BEHAVIOUR **********************/

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {
            CloseWithoutAnimation();
        }

        protected virtual void OnDestroy()
        {
            OnRelease?.Invoke(this);
        }


        /********************** PUBLIC INTERFACE **********************/

        protected void ShowCursor()
        {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
#endif
            Cursor.visible = true;
        }

        protected void HideCursor()
        {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
            Cursor.visible = false;
        }
        
        protected void ShowWithoutAnimation()
        {
            if (!canvasGroup)
                return;

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
            IsVisible = true;
        }
        
        protected void CloseWithoutAnimation()
        {
            if (!canvasGroup)
                return;
            
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            IsVisible = false;
        }
        
        public virtual async Awaitable ShowAsync()
        {
            if (IsVisible)
                return;

            if (gameObject.activeInHierarchy)
            {
                await animationAsset.ShowAsync(this);
                if (!canvasGroup)
                    return;

                canvasGroup.interactable = isInteractable;
                canvasGroup.blocksRaycasts = isInteractable;
                IsVisible = true;
            }
            else
            {
                ShowWithoutAnimation();
            }
        }

        public virtual async Awaitable CloseAsync()
        {
            if (!IsVisible)
                return;

            if (gameObject.activeInHierarchy)
            {
                if (!canvasGroup)
                    return;

                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                await animationAsset.HideAsync(this);
                IsVisible = false;
            }
            else
            {
                CloseWithoutAnimation();
            }
        }
        
    } // end of class
}