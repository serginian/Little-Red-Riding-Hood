using serginian.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace serginian.UI
{
    public class UiOptionButton : UiButton
    {
        [SerializeField] private TextMeshProUGUI textLabel;
        
        private Option _option;
        private UnityAction<Option> _callback;

        public void SetOption(Option option, UnityAction<Option> callback)
        {
            _option = option;
            _callback = callback;
            textLabel.text = option.Text;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            _callback?.Invoke(_option);
        }
        
    } // end of class
}