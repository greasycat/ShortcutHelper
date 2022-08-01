using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private new Camera camera;
    private List<GameObject> _horizontalLines;
    private List<GameObject> _verticalLines;

    private void Start()
    {
        _horizontalLines = new List<GameObject>();
        _verticalLines = new List<GameObject>();
        DrawGrid();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
            DrawGrid();
    }

    public void DrawGrid()
    {
        if (_verticalLines.Count > 0) {
            _verticalLines.ForEach((Destroy));
            _verticalLines = new List<GameObject>();
        }

        if (_horizontalLines.Count > 0)
        {
            _horizontalLines.ForEach((Destroy));
            _horizontalLines = new List<GameObject>();
        }

        var worldScreenHeight = (float) camera.orthographicSize * 2.0f;
        var worldScreenWidth = (float) worldScreenHeight / Screen.height * Screen.width;
        var horizontalLineNumber = (int) (worldScreenHeight / Interaction.Instance.currentSideScale);
        var verticalLineNumber = (int)( worldScreenWidth / Interaction.Instance.currentSideScale);

        var scale = Interaction.Instance.currentSideScale;

        for (var i = 0; i < horizontalLineNumber/2; ++i)
        {
            var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            var positions = new Vector3[]
            {
                new(-worldScreenWidth, i*scale, -1),
                new(0, i*scale, -1),
                new(worldScreenWidth, i*scale, -1)
            };
            lineRenderer.SetPositions(positions);
            _horizontalLines.Add(line);
        }
        for (var i = 1; i < horizontalLineNumber/2; ++i)
        {
            var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            var positions = new Vector3[]
            {
                new(-worldScreenWidth, -i*scale, -1),
                new(0, -i*scale, -1),
                new(worldScreenWidth, -i*scale, -1)
            };
            lineRenderer.SetPositions(positions);
            _horizontalLines.Add(line);
        }
        
        
        for (var i = 0; i < verticalLineNumber/2; ++i)
        {
            var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            var positions = new Vector3[]
            {
                new(i*scale, worldScreenHeight, -1),
                new(i*scale, 0, -1),
                new( i*scale,-worldScreenHeight ,-1)
            };
            lineRenderer.SetPositions(positions);
            _verticalLines.Add(line);
        }

        for (var i = 1; i < verticalLineNumber/2; ++i)
        {
            var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            var positions = new Vector3[]
            {
                new(-i*scale, worldScreenHeight, -1),
                new(-i*scale, 0, -1),
                new( -i*scale,-worldScreenHeight ,-1)
            };
            lineRenderer.SetPositions(positions);
            _verticalLines.Add(line);
        }
    }

    private void OnGUI()
    {

    }
}