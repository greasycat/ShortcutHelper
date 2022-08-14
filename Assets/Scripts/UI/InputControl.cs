using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InputControl : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        private Action<string, bool> _action;
        private bool _actionTriggered;
        private void Start()
        {
            // inputField = GetComponent<TMP_InputField>();
        }

        public void SetTextAction(Action<string, bool> action, string placeholder="")
        {
            if (action == null)
            {
                Debug.LogError("Null");
                return;
            }
            inputField.text = placeholder;
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            _action = action;
            _actionTriggered = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                transform.parent.gameObject.SetActive(false);
                if (!_actionTriggered)
                    _action(inputField.text, true);   
                inputField.text = "";
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                transform.parent.gameObject.SetActive(false);
                inputField.text = "";
                _action(null, false);
            }
        }
    }
}
