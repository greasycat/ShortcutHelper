using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mono
{
    public class Square : MonoBehaviour
    {
        public bool Selected;
        private Color _original;
        private bool _mouseOver;
        [SerializeField] private TMP_Text _text;
        public string squareName;

        private void Start()
        {
        }

        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            _mouseOver = true;
            if (Input.GetMouseButtonUp(1) && !Selected)
                Interaction.Instance.DeleteSquare(name);
            if (Input.GetMouseButtonUp(0))
            {
                if (!Selected)
                {
                    Select();
                    Interaction.Instance.hoveredSquare = this;
                    Interaction.Instance.ShowOnScreenInput();
                }
                else
                {
                    Interaction.Instance.ShowOnScreenInput();
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                GetComponent<SpriteRenderer>().color = _original;
                Interaction.Instance.SelectedSquareNames.Remove(name);
                Selected = false;
            }
        }

        public void Select(string customName = "T")
        {
                _original = GetComponent<SpriteRenderer>().color;
                GetComponent<SpriteRenderer>().color = new Color(100, 42, 0);
                Interaction.Instance.SelectedSquareNames.Add(name);
                Selected = true;
                SetText(customName);
        }

        public void SetText(string text)
        {
            _text.text = $"{text}";
            squareName = text;
        }

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