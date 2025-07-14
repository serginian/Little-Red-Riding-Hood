using serginian.Data;
using UnityEngine;
using UnityEngine.Events;

namespace serginian.UI
{
    public class ChoiceWindow : UiWindow
    {
        [SerializeField] private GameObject buttonTemplate;
        [SerializeField] private Transform buttonContainer;
        
        public event UnityAction<Option> OnOptionSelected;
        

        public Awaitable ShowChoiceAsync(Option[] choices)
        {
            ClearButtonContainer();
            
            foreach (var option in choices)
                CreateButton(option);

            return ShowAsync();
        }

        private void CreateButton(Option option)
        {
            GameObject go = Instantiate(buttonTemplate, buttonContainer);
            UiOptionButton button = go.GetComponent<UiOptionButton>();
            button.SetOption(option, OnChoiceMade);
            go.SetActive(true);
        }

        private void OnChoiceMade(Option option)
        {
            OnOptionSelected?.Invoke(option);
        }
        
        private void ClearButtonContainer()
        {
            foreach (Transform child in buttonContainer)
            {
                if (child.gameObject.activeSelf)
                    Destroy(child.gameObject);
            }
        }

    } // end of class
}