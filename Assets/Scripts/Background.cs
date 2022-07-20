using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private new Camera camera;
    private BoxCollider2D _collider2D;
    [SerializeField] private GameObject shadowSquarePrefab;
    private GameObject _shadowSquare;

    private void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
        ResizeSpriteToScreen();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMouseMovement();
    }

    private void HandleMouseMovement()
    {
    }
    
    private void OnMouseOver()
    {
        var mousePosition = Input.mousePosition;
        var worldMousePosition = camera.ScreenToWorldPoint(mousePosition);
        var mouse2dPosition = new Vector2(worldMousePosition.x, worldMousePosition.y);

        var scale = Interaction.Instance.currentSideScale;
        var squarePosition = GetNearestSquareCenter(mouse2dPosition, scale);
        if (_shadowSquare)
        {
            _shadowSquare.transform.position = squarePosition;
            _shadowSquare.transform.localScale = new Vector3(scale, scale, _shadowSquare.transform.localScale.z);
        }

        if (Input.GetMouseButtonUp(0))
        {
            
        }

    }
    

    private void OnMouseEnter()
    {
        if (_shadowSquare && !_shadowSquare.activeSelf)
        {
                _shadowSquare.SetActive(true);
        }
        else
        {
            _shadowSquare = Instantiate(shadowSquarePrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        if (_shadowSquare)
        {
            _shadowSquare.SetActive(false);
        }
    }

    private static Vector2 GetNearestSquareCenter(Vector2 sourceVector, float unit)
    {
        var x = sourceVector.x;
        var y = sourceVector.y;
        var cx = Mathf.Sign(x) * Mathf.Ceil(Mathf.Abs(x)/unit)*unit;
        var cy = Mathf.Sign(y) * Mathf.Ceil(Mathf.Abs(y)/unit)*unit;
        var fx = Mathf.Sign(x) * Mathf.Floor(Mathf.Abs(x)/unit)*unit;
        var fy = Mathf.Sign(y) * Mathf.Floor(Mathf.Abs(y)/unit)*unit;
        
        // Debug.Log($"cx@{cx}, cy@{cy}, fx@{fx}, fy{fy} at unit@{unit}");

        return new Vector2((cx + fx) / 2, (cy + fy) / 2);
    }


    private void ResizeSpriteToScreen()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        var sprite = sr.sprite;
        var width = sprite.bounds.size.x;
        var height = sprite.bounds.size.y;

        var worldScreenHeight = camera.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var scale = transform.localScale;
        transform.localScale =
            new Vector3((float) (worldScreenWidth / width), (float) (worldScreenHeight / height),
                transform.localScale.z);
        _collider2D.size = sr.size;
    }
}