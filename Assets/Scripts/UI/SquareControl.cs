using System;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SquareControl : MonoBehaviour
    {
        public bool Selected;
        private Color _original;
        private bool _mouseOver;
        [SerializeField] private TMP_Text _text;
        public string SquareName { private set; get; }

        private void Start()
        {
        }

        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _mouseOver = true;

            switch (Interaction.Instance.CurrentTool)
            {
                case Tool.Pen:
                    if (Input.GetMouseButtonUp(0) && !Selected)
                    {
                        Select();
                        Interaction.Instance.SquareToEdit = this;
                        Interaction.Instance.SetOnScreenInputAction((string s, bool success) =>
                            {
                                if (success) SetText(s);
                            }
                            , GetText());
                    }
                    else if (Input.GetMouseButtonUp(0) && Selected)
                    {
                        Interaction.Instance.SetOnScreenInputAction((string s, bool success) =>
                            {
                                if (success) SetText(s);
                            }
                            , GetText());
                    }
                    else if (Input.GetMouseButtonUp(1) && Selected)
                    {
                        GetComponent<SpriteRenderer>().color = _original;
                        Interaction.Instance.SelectedSquareNames.Remove(name);
                        Selected = false;
                        SetText("T");
                    }

                    break;
                case Tool.Eraser:
                    if (Input.GetMouseButton(0) )
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
            if (Selected)
            {
                Interaction.Instance.SelectedSquareNames.Remove(name);
                Selected = false;
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
            Selected = true;
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
    }
}