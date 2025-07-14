using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace serginian.IO
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionReference touchInput;

        public static event UnityAction OnInteracted;


        private void Start()
        {
            touchInput.action.performed += OnClicked;
        }

        private void OnDestroy()
        {
            touchInput.action.performed -= OnClicked;
        }

        private void OnEnable()
        {
            if (touchInput != null)
                touchInput.action.Enable();
        }

        private void OnDisable()
        {
            if (touchInput != null)
                touchInput.action.Disable();
        }

        private void OnClicked(InputAction.CallbackContext obj)
        {
            OnInteracted?.Invoke();
        }
    }
}
