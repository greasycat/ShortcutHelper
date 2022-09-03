using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SquareControl : MonoBehaviour
    {
        public bool selected;
        private Color _original;
        private bool _mouseOver;
        private List<float> _clickList = new();
        
        [SerializeField] private TMP_Text _text;
        public string SquareName { private set; get; }

        private void Start()
        {
        }

        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _mouseOver = true;

            switch (Toolbar.Instance.currentTool)
            {
                case Tool.Pen:
                    RunIfDoubleClick(HandleDoubleClick);

                    if (Input.GetMouseButtonUp(1) && selected)
                    {
                        GetComponent<SpriteRenderer>().color = _original;
                        Interaction.Instance.SelectedSquareNames.Remove(name);
                        selected = false;
                        SetText("T");
                    }

                    break;
                case Tool.Eraser:
                    if (Input.GetMouseButton(0))
                        DeleteSquare();
                    break;
                case Tool.Select:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                foreach (var pair in Interaction.Instance.Squares[name].GetAdjacentSquares())
                {
                    Debug.Log($"{pair.Key}, {pair.Value}");
                }
            }
        }

        public void DeleteSquare()
        {
            if (selected)
            {
                Interaction.Instance.SelectedSquareNames.Remove(name);
                selected = false;
                Interaction.Instance.DeleteSquare(name);
            }
            else
            {
                Interaction.Instance.DeleteSquare(name);
            }
        }

        public void Select(string customName = "")
        {
            _original = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = Color.red;
            Interaction.Instance.SelectedSquareNames.Add(name);
            selected = true;
            SetText(customName);
        }

        private void SetText(string text)
        {
            _text.text = $"{text}";
            SquareName = text;
        }

        private string GetText() => _text.text;

        public void HideText()
        {
            _text.text = "";
        }

        private void OnMouseExit()
        {
            _mouseOver = false;
        }

        private void HandleDoubleClick()
        {
            if (!selected)
            {
                Select();
                Interaction.Instance.SquareToEdit = this;
                Interaction.Instance.SetOnScreenInputAction((string s, bool success) =>
                    {
                        if (success) SetText(s);
                    }
                    , GetText());
            }
            else 
            {
                Interaction.Instance.SetOnScreenInputAction((string s, bool success) =>
                    {
                        if (success) SetText(s);
                    }
                    , GetText());
            }
        }

        private void RunIfDoubleClick(Action action)
        {
            if (Input.GetMouseButtonUp(0))
            {
                var currentTime = Time.time;
                if (_clickList.Count > 1)
                {
                    if (currentTime - _clickList.Last() < 0.25f)
                    {
                        action();
                        _clickList.RemoveRange(0, 2);
                    }
                }

                _clickList.Add(currentTime);
            }
        }
    }
}