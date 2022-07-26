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
        private float _lineWidth = 0.02f;
        private bool _enableGrid = false;

        private void Start()
        {
            _horizontalLines = new List<GameObject>();
            _verticalLines = new List<GameObject>();
            DrawGrid();
        }
        
        public static Grid Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }

            Instance = this;
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void SetLineWidth(float width)
        {
            _lineWidth = width;
        }

        public void AdjustLineWidthByZoom(float zoomLevel)
        {
            _lineWidth = 0.02f + 0.004f*Mathf.Log(zoomLevel);
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
            
            if (!_enableGrid) return;

            var worldScreenHeight = (float) camera.orthographicSize * 2.0f;
            var worldScreenWidth = (float) worldScreenHeight / Screen.height * Screen.width;
            var horizontalLineNumber = (int) (worldScreenHeight / Interaction.Instance.currentSideScale);
            var verticalLineNumber = (int) (worldScreenWidth / Interaction.Instance.currentSideScale);
            print(worldScreenHeight);

            var scale = Interaction.Instance.currentSideScale;
            var offset = Background.Instance.originOffset;
            var x = offset.x;
            var y = offset.y;

            for (var i = 0; i < horizontalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                lineRenderer.startWidth = _lineWidth;
                lineRenderer.endWidth = _lineWidth;
                var positions = new Vector3[]
                {
                    new(-worldScreenWidth, i * scale, -1),
                    new(0, i * scale, -1),
                    new(worldScreenWidth, i * scale, -1)
                };
                
                for (var j = 0; j != positions.Length; ++j)
                {
                    positions[j].x += x;
                    positions[j].y += y;
                }
                
                lineRenderer.SetPositions(positions);
                _horizontalLines.Add(line);
            }

            for (var i = 1; i < horizontalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                lineRenderer.startWidth = _lineWidth;
                lineRenderer.endWidth = _lineWidth;
                var positions = new Vector3[]
                {
                    new(-worldScreenWidth, -i * scale, -1),
                    new(0, -i * scale, -1),
                    new(worldScreenWidth, -i * scale, -1)
                };

                for (var j = 0; j != positions.Length; ++j)
                {
                    positions[j].x += x;
                    positions[j].y += y;
                }

                lineRenderer.SetPositions(positions);
                _horizontalLines.Add(line);
            }


            for (var i = 0; i < verticalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                lineRenderer.startWidth = _lineWidth;
                lineRenderer.endWidth = _lineWidth;
                var positions = new Vector3[]
                {
                    new(i * scale, worldScreenHeight, -1),
                    new(i * scale, 0, -1),
                    new(i * scale, -worldScreenHeight, -1)
                };
                for (var j = 0; j != positions.Length; ++j)
                {
                    positions[j].x += x;
                    positions[j].y += y;
                }
                lineRenderer.SetPositions(positions);
                _verticalLines.Add(line);
            }

            for (var i = 1; i < verticalLineNumber / 2; ++i)
            {
                var line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 3;
                lineRenderer.startWidth = _lineWidth;
                lineRenderer.endWidth = _lineWidth;
                var positions = new Vector3[]
                {
                    new(-i * scale, worldScreenHeight, -1),
                    new(-i * scale, 0, -1),
                    new(-i * scale, -worldScreenHeight, -1)
                };
                for (var j = 0; j != positions.Length; ++j)
                {
                    positions[j].x += x;
                    positions[j].y += y;
                }
                lineRenderer.SetPositions(positions);
                _verticalLines.Add(line);
            }
        }


        private void OnGUI()
        {
        }
    }
}