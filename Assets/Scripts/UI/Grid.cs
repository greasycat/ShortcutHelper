using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Utils;

namespace UI
{
    public class Grid : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private new Camera camera;
        private List<GameObject> _horizontalLines;
        private List<GameObject> _verticalLines;

        private int _scrollingTriggered;
        private float _scrollingTime = 0;
        private bool _isScrolling;
        private float _zoomLevel=1;

        private void Start()
        {
            _horizontalLines = new List<GameObject>();
            _verticalLines = new List<GameObject>();
            DrawGrid();
        }

        // Update is called once per frame
        private void Update()
        {
            var mouseScroll = Input.mouseScrollDelta.y;
            // Debug.Log(0.1f * mouseScroll);
            _scrollingTriggered += (int)mouseScroll;
            if (Helper.IsFloatEqual(mouseScroll, 0) && _scrollingTriggered == 0)
            {
                _scrollingTime = Time.time;
            }
            
            if (Time.time - _scrollingTime >= 0.2f)
            {
                Debug.Log(_scrollingTriggered);
                if (_scrollingTriggered != 0)
                {
                    _zoomLevel *= Mathf.Pow(2,Mathf.Sign(_scrollingTriggered));
                    Interaction.Instance.currentSideScale = _zoomLevel;
                    StartCoroutine(Interaction.Instance.ShowMessageCoroutine($"Changed scaled to {_zoomLevel}"));
                    DrawGrid();
                }
                _scrollingTriggered = 0;
            }
        }

        public void DrawGrid()
        {
            if (_verticalLines.Count > 0)
            {
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
            var verticalLineNumber = (int) (worldScreenWidth / Interaction.Instance.currentSideScale);

            var scale = Interaction.Instance.currentSideScale;

            for (var i = 0; i < horizontalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                var positions = new Vector3[]
                {
                    new(-worldScreenWidth, i * scale, -1),
                    new(0, i * scale, -1),
                    new(worldScreenWidth, i * scale, -1)
                };
                lineRenderer.SetPositions(positions);
                _horizontalLines.Add(line);
            }

            for (var i = 1; i < horizontalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                var positions = new Vector3[]
                {
                    new(-worldScreenWidth, -i * scale, -1),
                    new(0, -i * scale, -1),
                    new(worldScreenWidth, -i * scale, -1)
                };
                lineRenderer.SetPositions(positions);
                _horizontalLines.Add(line);
            }


            for (var i = 0; i < verticalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                var positions = new Vector3[]
                {
                    new(i * scale, worldScreenHeight, -1),
                    new(i * scale, 0, -1),
                    new(i * scale, -worldScreenHeight, -1)
                };
                lineRenderer.SetPositions(positions);
                _verticalLines.Add(line);
            }

            for (var i = 1; i < verticalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                var positions = new Vector3[]
                {
                    new(-i * scale, worldScreenHeight, -1),
                    new(-i * scale, 0, -1),
                    new(-i * scale, -worldScreenHeight, -1)
                };
                lineRenderer.SetPositions(positions);
                _verticalLines.Add(line);
            }
        }


        private void OnGUI()
        {
        }
    }
}