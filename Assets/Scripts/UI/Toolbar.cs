using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Toolbar : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private GameObject content;

        [SerializeField] private Button penButton;
        [SerializeField] private Button eraserButton;
        [SerializeField] private Button selectButton;

        private List<Button> _selectableButtons;

        private void Start()
        {
            _selectableButtons = new List<Button>()
            {
                penButton,
                eraserButton,
                selectButton
            };
            
        }

        // Update is called once per frame
        private void Update()
        {
            //Hide Control H
        }

        public void SelectButton(string buttonName)
        {
            Interaction.Instance.CurrentTool = buttonName switch
            {
                "pen" => Tool.Pen,
                "eraser" => Tool.Eraser,
                "select" => Tool.Select,
                _ => throw new NotImplementedException("No such tool")
            };
            for (var i = 0; i < _selectableButtons.Count; ++i)
            {
                if ((int) Interaction.Instance.CurrentTool == i)
                {
                    
                    var colors = _selectableButtons[i].colors;
                    colors.normalColor = Color.white;
                    _selectableButtons[i].colors = colors;
                }
                else
                {

                    var colors = _selectableButtons[i].colors;
                    colors.normalColor = Color.black;
                    _selectableButtons[i].colors = colors;
                }
            }
        }

        private void OnGUI()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.H)
            {
                content.SetActive(!content.activeSelf);
            }
        }
    }
}