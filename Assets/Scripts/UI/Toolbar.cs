using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using TMPro;
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

        [SerializeField] private GameObject sizeProperty;
        [SerializeField] private TMP_InputField sizeInputField;
        [SerializeField] private Slider slider;

        [SerializeField] private GameObject dimensionProperty;
        [SerializeField] private TMP_InputField widthInputField;
        [SerializeField] private TMP_InputField heightInputField;
        

        private List<Button> _selectableButtons;
        public Tool currentTool;
        public int currentToolSize = 1;

        private string _lastRightClicked = "pen";

        public static Toolbar Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }

            Instance = this;
        }
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

        //Handle Pen Size
        public void RightClickToChangeSize(string buttonName)
        {
            if (_lastRightClicked == buttonName)
            {
                sizeProperty.SetActive(!sizeProperty.activeSelf);
            }
            else
            {
                sizeProperty.SetActive(true);
            }
            _lastRightClicked = buttonName;
            var pos = sizeProperty.transform.position;
            pos.y = buttonName switch
            {
                "pen" => penButton.transform.position.y,
                "eraser" => eraserButton.transform.position.y,
                _ => pos.y 
            };
            sizeProperty.transform.position = pos;
        }

        public void OnSizeInputFieldChange()
        {
            var changedText = sizeInputField.text;
            if (int.TryParse(changedText, out var size))
            {
                currentToolSize = size;
                slider.value = size;
                Interaction.Instance.ShowInfoForSeconds($"Change size to {currentToolSize}");
            }
            else
            {
                sizeInputField.text = currentToolSize.ToString();
                slider.value = size;
            }
        }

        public void OnSliderChange()
        {
            var changedValue = slider.value;
            if (slider.value >= 200) return;
            currentToolSize = (int)changedValue;
            sizeInputField.text = $"{currentToolSize}";
            Interaction.Instance.ShowInfoForSeconds($"Change size to {currentToolSize}");
        }

        //Handle Map Dimension UI
        public void RightClickToChangeDimension()
        {
            dimensionProperty.SetActive(!sizeProperty.activeSelf);
        }

        public void ClickToConfirmChangeDimension()
        {
            if (!int.TryParse(widthInputField.text, out var width) ||
                !int.TryParse(heightInputField.text, out var height)
                || width<=0 || height <=0)
            {
                Interaction.Instance.ShowMessageForSeconds("Please input the correct dimension");
                return;
            }
            
            Interaction.Instance.NewMap(width, height);
        }

        public void SelectButton(string buttonName)
        {
            currentTool = buttonName switch
            {
                "pen" => Tool.Pen,
                "eraser" => Tool.Eraser,
                "select" => Tool.Select,
                _ => throw new NotImplementedException("No such tool")
            };
            for (var i = 0; i < _selectableButtons.Count; ++i)
            {
                if ((int) currentTool == i)
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

        public void ToolSizeChange()
        {
            Interaction.Instance.SetOnScreenInputAction((text, success) =>
            {
                if (!success) return;
                if (!int.TryParse(text, out var tempSize)) return;
                if (tempSize < 1) return;
                currentToolSize = tempSize;
                Interaction.Instance.ShowInfoForSeconds($"Change tool size to {currentToolSize}");
            }, $"{(int)(currentToolSize)}");
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