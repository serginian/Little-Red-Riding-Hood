using TMPro;
using UnityEngine;

namespace serginian.UI
{
    public class MessageWindow : UiWindow
    {
        [SerializeField] private TextMeshProUGUI messageLabel;
        
        protected override void Start()
        {
            CloseWithoutAnimation();
        }

        public Awaitable ShowMessageAsync(string message)
        {
            messageLabel.text = message;
            return ShowAsync();
        }

        public void RequestExit()
        {
#if !UNITY_EDITOR
            Application.Quit();
#endif
        }
        
    } // end of class
}