using serginian.UI;
using UnityEngine;
using UnityEngine.UI;

namespace serginian.UI
{
    public class BackgroundWindow : UiWindow
    {
        [SerializeField] private Image backgroundImage;

        public void SetBackground(Sprite sprite)
        {
            backgroundImage.sprite = sprite;
        }
    }
}
