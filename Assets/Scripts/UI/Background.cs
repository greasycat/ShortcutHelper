using System;
using Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using File = System.IO.File;
using Input = UnityEngine.Input;

namespace UI
{
    public class Background : MonoBehaviour
    {
        //Background Image
        private const float PixelPerUnit = 1f;

        //Dragging
        private const float CameraSpeed = 16f;

        // Start is called before the first frame update
        [SerializeField] private new Camera camera;

        //Shadow square
        [SerializeField] private GameObject shadowSquarePrefab;
        [SerializeField] private float scrollingTimeThreshold = 0.2f;
        public Vector2 originOffset = Vector2.zero;
        public GameObject image;
        private BoxCollider2D _collider2D;
        private bool _imageLoaded;
        private SpriteRenderer _imageSpriteRender;
        private bool _isScrolling;
        private Vector2 _mouseDownPosition;
        private float _scrollingTime = 0;


        //Scrolling
        private int _scrollingTriggered;
        private GameObject _centerShadowSquare;
        private SpriteRenderer _spriteRenderer;
        private Texture2D _texture;
        private Vector2 _x_axis_maximums;
        private Vector2 _y_axis_maximums;
        private float _zoomLevel = 1;


        public static Background Instance { get; private set; }

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
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<BoxCollider2D>();
            _imageSpriteRender = image.GetComponent<SpriteRenderer>();
            ResizeSpriteToScreen();

            LoadImage("silcton_cropped.jpg");
        }

        // Update is called once per frame
        private void Update()
        {
            HandleScrollMovement();
            HandleArrowKey();
            HandleMiddleMouseDrag();
            HandleMouseLeftClick();
        }

        //When drag
        private void OnMouseDrag()
        {
            HandleMouseLeftClick();
        }

        //When mouse enter
        private void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (_centerShadowSquare && !_centerShadowSquare.activeSelf)
            {
                _centerShadowSquare.SetActive(true);
            }
            else
            {
                _centerShadowSquare = Instantiate(shadowSquarePrefab, Vector3.zero, Quaternion.identity);
            }
        }

        private void OnMouseExit()
        {
            if (_centerShadowSquare)
            {
                _centerShadowSquare.SetActive(false);
            }
        }

        private void HandleMiddleMouseDrag()
        {
            if (Input.GetMouseButton(2))
            {
                var offset = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
                offset *= (Mathf.Log(_zoomLevel/1.5f));
                camera.transform.position += offset;
            }
        }

        private void HandleScrollMovement()
        {
            var mouseScroll = Input.mouseScrollDelta.y;
            // Debug.Log(0.1f * mouseScroll);
            _scrollingTriggered += (int) mouseScroll;
            if (Helper.IsFloatEqual(mouseScroll, 0) && _scrollingTriggered == 0)
            {
                _scrollingTime = Time.time;
            }

            if (Time.time - _scrollingTime >= scrollingTimeThreshold)
            {
                if (_scrollingTriggered != 0)
                {
                    var temp = _zoomLevel * Mathf.Pow(2, Mathf.Sign(_scrollingTriggered));
                    if (temp <= 1)
                    {
                        _zoomLevel = temp;
                        // Interaction.Instance.currentSideScale = _zoomLevel;
                        StartCoroutine(Interaction.Instance.ShowMessageCoroutine($"Changed scaled to {_zoomLevel}"));
                        camera.orthographicSize += CameraSpeed * Mathf.Sign(-_scrollingTriggered);
                        Grid.Instance.AdjustLineWidthByZoom(_zoomLevel);
                        Grid.Instance.DrawGrid();
                        ResizeSpriteToScreen();
                    }
                }

                _scrollingTriggered = 0;
            }
        }

        private void HandleArrowKey()
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                MoveOrigin(Vector2.left * Mathf.Log(_zoomLevel));
                Grid.Instance.DrawGrid();
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                MoveOrigin(Vector2.right * Mathf.Log(_zoomLevel));
                Grid.Instance.DrawGrid();
            }
        }

        private void MoveOrigin(Vector2 offset)
        {
            originOffset += offset;
            camera.transform.position += new Vector3(offset.x, offset.y, 0);
        }

        private void HandleMouseLeftClick()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var mousePosition = Input.mousePosition;
            var worldMousePosition = camera.ScreenToWorldPoint(mousePosition);
            var mouse2dPosition = new Vector2(worldMousePosition.x, worldMousePosition.y);

            var scale = Interaction.Instance.currentSideScale;
            var squarePosition = Helper.GetNearestSquareCenter(mouse2dPosition, scale);

            var penSize = Toolbar.Instance.currentToolSize;

            //When odd pen size
            if (Helper.IsOdd(penSize))
            {
                if (!_centerShadowSquare) return;
                _centerShadowSquare.transform.position = squarePosition;
                _centerShadowSquare.transform.localScale = new Vector3(scale, scale, _centerShadowSquare.transform.localScale.z);
            }
            
            // When even size pen size
            else
            {
                
            }
            
            if (Input.GetMouseButton(0))
            {
                switch (Toolbar.Instance.currentTool)
                {
                    case Tool.Pen:
                        Interaction.Instance.NewSquare(_centerShadowSquare.transform.position);
                        break;
                    case Tool.Eraser:
                        break;
                    case Tool.Select:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void InstantiateSquares(Vector3 center, float scale)
        {
            var positiveOffset = new Vector3(1, 1, 0);
            var size = Toolbar.Instance.currentToolSize;
            var startingPosition = center - scale * positiveOffset;
            for (var i = 0; i != size; ++i)
            {
                for (var j = 0; j != size; ++j)
                {
                    
                }
            }
        }

        public void ResizeSpriteToScreen()
        {
            if (_imageLoaded) return;
            if (!_spriteRenderer) return;

            // transform.localScale = new Vector3(1, 1, 1);
            //
            // var sprite = _spriteRenderer.sprite;
            // var width = sprite.bounds.size.x;
            // var height = sprite.bounds.size.y;
            //
            // var worldScreenHeight = camera.orthographicSize * 2.0;
            // var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            //
            // var scale = transform.localScale;
            // transform.localScale =
            //     new Vector3((float) (worldScreenWidth / width), (float) (worldScreenHeight / height),
            //         transform.localScale.z);
            var edgeVector = camera.ViewportToWorldPoint(new Vector3(1, 1));
            transform.localScale = new Vector3(edgeVector.x * 2, edgeVector.y * 2, transform.localScale.z);
            _collider2D.size = _spriteRenderer.size;
        }

        private void LoadImage(string path)
        {
            var bytes = File.ReadAllBytes(path);
            _texture = new Texture2D(2, 2)
            {
                filterMode = FilterMode.Point
            };
            if (!_texture.LoadImage(bytes)) return;

            _imageSpriteRender.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height),
                new Vector2(0, 1), PixelPerUnit);
            image.transform.position = new Vector3(Mathf.Floor(-_texture.width / (PixelPerUnit * 2f)),
                Mathf.Floor(_texture.height / (PixelPerUnit * 2f)), 2);
            _imageLoaded = true;
            ResizeSpriteToImage();
        }

        private void ResizeSpriteToImage()
        {
            if (!_imageLoaded) return;
            // var worldScreenHeight = camera.orthographicSize * 2.0;
            // var worldScreenWidth = worldScreenHeight / _texture.height * _texture.width;

            // var spriteSize = _imageSpriteRender.sprite.bounds.size;
            // print(spriteSize);

            transform.localScale = new Vector3(_texture.width, _texture.height, -1);
        }

    }
}