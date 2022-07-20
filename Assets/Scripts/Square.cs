using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _mouseOver;
    private SpriteRenderer _renderer;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_mouseOver)
        {
            if (Input.GetMouseButtonUp(1))
                Interaction.Instance.DeleteSquare(name);
                
        }
    }

    private void PrintVertices()
    {
        foreach (var v2 in _renderer.sprite.vertices)
        {
            Debug.Log(v2);
        }
    }

    private void OnMouseOver()
    {
        _mouseOver = true;
    }

    private void OnMouseExit()
    {
        _mouseOver = false;
    }
}
