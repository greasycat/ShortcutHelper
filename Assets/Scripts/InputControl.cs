using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_InputField _inputField;
    private void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            transform.parent.gameObject.SetActive(false);
            Interaction.Instance.hoveredSquare.SetText(_inputField.text);
            _inputField.text = "";
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            transform.parent.gameObject.SetActive(false);
            _inputField.text = "";
        }
    }
}
