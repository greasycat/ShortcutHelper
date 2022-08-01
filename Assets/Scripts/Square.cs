using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

    private bool _selected;
    private Color _original;
    private GameObject _number;
    private TextMesh _numberMesh;

    private void Start()
    {
        _number = new GameObject("Number")
        {
            transform =
            {
                parent = transform
            }
        };
        _numberMesh = _number.AddComponent<TextMesh>();
        if (_numberMesh == null) return;
        _numberMesh.transform.position = transform.position;
        _numberMesh.characterSize = 0.2f;
        _numberMesh.fontSize = 8;
        _numberMesh.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1) && !_selected)
            Interaction.Instance.DeleteSquare(name);
        if (Input.GetMouseButtonUp(0))
        {
            if (!_selected)
            {
                _original = GetComponent<SpriteRenderer>().color;
                GetComponent<SpriteRenderer>().color = Color.red;
                Interaction.Instance.SelectedSquareNames.Add(name);
                _selected = true;
                ShowNumber();
            }
            else
            {
                
                GetComponent<SpriteRenderer>().color = _original;
                Interaction.Instance.SelectedSquareNames.Remove(name);
                _selected = false;
                HideNumber();
            }
            
        }
    }

    private void ShowNumber()
    {
        _numberMesh.text = $"{Interaction.Instance.SelectedSquareNames.IndexOf(name)}";
    }

    private void HideNumber()
    {
        _numberMesh.text = "";
    }

    private void OnMouseExit()
    {
    }
}
