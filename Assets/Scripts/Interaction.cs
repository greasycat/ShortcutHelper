using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private new Camera camera;

    private Square _currentSquare;
    private Dictionary<string, Square> _squares;
    private int _maxSquareCounter;
    [SerializeField] public float currentSideScale = 1;
    private string _textScale;
    private enum Mode 
    {
        Keyboard,
        Mouse
    }

    [SerializeField] private Mode mode = Mode.Mouse;
    

    private enum AdjacentSide
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    private struct SquareInfo
    {
        public Vector2 Min;
        public Vector2 Max;

        public override string ToString()
        {
            return $"Min: {Min.x}:{Min.y} Max: {Max.x}:{Max.y}";
        }
    }


    private class Square : IDisposable
    {
        public readonly GameObject Self;
        public Square Up;
        public Square Down;
        public Square Left;
        public Square Right;
        private readonly float _sideScale;

        public Square(GameObject square, float sideScale = 1)
        {
            Self = square;
            _sideScale = sideScale;
            square.transform.localScale = new Vector3(sideScale, sideScale, square.transform.localScale.z);
        }

        private SquareInfo GetRect()
        {
            var position = Self.transform.position;
            return new SquareInfo
            {
                Min = new Vector2(position.x - _sideScale / 2, position.y - _sideScale / 2),
                Max = new Vector2(position.x + _sideScale / 2, position.y + _sideScale / 2)
            };
        }

        public AdjacentSide CheckIfAdjacent(Square target)
        {
            var targetRect = target.GetRect();
            var selfRect = GetRect();

            // Debug.Log(targetRect);
            // Debug.Log(selfRect);
            //check up
            if (IsFloatEqual(targetRect.Min.y, selfRect.Max.y) &&
                ((targetRect.Min.x >= selfRect.Min.x && targetRect.Max.x <= selfRect.Max.x) ||
                 (targetRect.Min.x <= selfRect.Min.x && targetRect.Max.x >= selfRect.Max.x)))
            {
                return AdjacentSide.Up;
            }

            //check down
            if (IsFloatEqual(targetRect.Max.y, selfRect.Min.y) &&
                ((targetRect.Min.x >= selfRect.Min.x && targetRect.Max.x <= selfRect.Max.x) ||
                 (targetRect.Min.x <= selfRect.Min.x && targetRect.Max.x >= selfRect.Max.x)))
            {
                return AdjacentSide.Down;
            }

            //check left
            if (IsFloatEqual(targetRect.Min.x, selfRect.Max.x) &&
                ((targetRect.Min.y >= selfRect.Min.y && targetRect.Max.y <= selfRect.Max.y) ||
                 (targetRect.Min.y <= selfRect.Min.y && targetRect.Max.y >= selfRect.Max.y)))
            {
                return AdjacentSide.Right;
            }

            //check right
            if (IsFloatEqual(targetRect.Max.x, selfRect.Min.x) &&
                ((targetRect.Min.y >= selfRect.Min.y && targetRect.Max.y <= selfRect.Max.y) ||
                 (targetRect.Min.y <= selfRect.Min.y && targetRect.Max.y >= selfRect.Max.y)))
            {
                return AdjacentSide.Left;
            }

            return AdjacentSide.None;
        }

        public override string ToString()
        {
            return
                $"Name:{Self.name}\t Up: {Up?.Self.name}\t Down: {Down?.Self.name}\t Left: {Left?.Self.name}\t Right: {Right?.Self.name}";
        }

        public void Dispose()
        {
            if (Up != null)
            {
                Up.Down = null;
                Up = null;
            }

            if (Down != null)
            {
                Down.Up = null;
                Down = null;
            }

            if (Left != null)
            {
                Left.Right = null;
                Left = null;
            }

            if (Right != null)
            {
                Right.Left = null;
                Right = null;
            }

            Destroy(Self);
        }
    }

    private static bool IsFloatEqual(float f1, float f2)
    {
        return Mathf.Abs(f1 - f2) < 0.001;
    }

    public static Interaction Instance { get; private set; }
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
        _squares = new Dictionary<string, Square>();
        // SetSquareActive(NewSquare(Vector3.zero));
    }


    // Update is called once per frame
    private void Update()
    {
        switch (mode)
        {
            case Mode.Keyboard:
                // HandleKeyboardInput();
                break;
            case Mode.Mouse:
                break;
        }
    }

    // private void HandleKeyboardInput()
    // {
    //     var mousePosition = Input.mousePosition;
    //     var worldMousePosition = camera.ScreenToWorldPoint(mousePosition);
    //     worldMousePosition.z += 1;
    //
    //     if (Input.GetKeyUp(KeyCode.W))
    //         HandleMovement(KeyCode.W);
    //     if (Input.GetKeyUp(KeyCode.S))
    //         HandleMovement(KeyCode.S);
    //     if (Input.GetKeyUp(KeyCode.A))
    //         HandleMovement(KeyCode.A);
    //     if (Input.GetKeyUp(KeyCode.D))
    //         HandleMovement(KeyCode.D);
    //
    //     if (Input.GetKeyUp(KeyCode.P))
    //         PrintDebugInfo();
    // }

    private void PrintDebugInfo()
    {
        foreach (var square in _squares)
        {
            Debug.Log(square);
        }
    }

    // private void SetSquareActive(Square target)
    // {
    //     if (_currentSquare != null && _currentSquare.Self != null)
    //         _currentSquare.Self.GetComponent<SpriteRenderer>().color = Color.white;
    //
    //     _currentSquare = target;
    //     if (!target.Self) throw new NullReferenceException("Null target self");
    //     _currentSquare.Self.GetComponent<SpriteRenderer>().color = Color.gray;
    //     Debug.Log("set current active square");
    // }

    private Square NewSquare(Vector3 position, float sideLength = 1)
    {
        var square = new Square(Instantiate(squarePrefab, position, Quaternion.identity), sideLength);
        _maxSquareCounter += 1;
        square.Self.name = $"Square@{_maxSquareCounter}";
        _squares[square.Self.name] = square;
        return square;
    }

    public void DeleteSquare(string squareName)
    {
        _squares[squareName].Dispose();
        _squares.Remove(squareName);
    }

    private void SetAdjacentSquare(Square target)
    {
        foreach (var pair in _squares)
        {
            var square = pair.Value;
            var result = target.CheckIfAdjacent(square);
            switch (result)
            {
                case AdjacentSide.Up:
                    target.Up = square;
                    square.Down = target;
                    break;
                case AdjacentSide.Down:
                    target.Down = square;
                    square.Up = target;
                    break;
                case AdjacentSide.Left:
                    target.Left = square;
                    square.Right = target;
                    break;
                case AdjacentSide.Right:
                    target.Right = square;
                    square.Left = target;
                    break;
                case AdjacentSide.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    // private void HandleMovement(KeyCode key)
    // {
    //     var newSquarePosition = _currentSquare.Self.transform.position;
    //     Square newSquare;
    //     switch (key)
    //     {
    //         case KeyCode.W:
    //
    //             if (_currentSquare.Up != null)
    //                 break;
    //
    //             newSquarePosition.y += currentSideScale;
    //             newSquare = NewSquare(newSquarePosition, currentSideScale);
    //             SetAdjacentSquare(newSquare);
    //             SetSquareActive(newSquare);
    //             break;
    //
    //         case KeyCode.S:
    //
    //             if (_currentSquare.Down != null)
    //                 break;
    //
    //             newSquarePosition.y -= currentSideScale;
    //             newSquare = NewSquare(newSquarePosition, currentSideScale);
    //             SetAdjacentSquare(newSquare);
    //             SetSquareActive(newSquare);
    //             break;
    //
    //         case KeyCode.A:
    //
    //             if (_currentSquare.Left != null)
    //                 break;
    //
    //             newSquarePosition.x -= currentSideScale;
    //             newSquare = NewSquare(newSquarePosition, currentSideScale);
    //             SetAdjacentSquare(newSquare);
    //             SetSquareActive(newSquare);
    //             break;
    //
    //         case KeyCode.D:
    //             if (_currentSquare.Right != null)
    //                 break;
    //
    //             newSquarePosition.x += currentSideScale;
    //             newSquare = NewSquare(newSquarePosition, currentSideScale);
    //             SetAdjacentSquare(newSquare);
    //             SetSquareActive(newSquare);
    //             break;
    //     }
    // }

    private void OnGUI()
    {
        GUI.Box(new Rect(50, 25, 180, 140), "");
        GUI.Label(new Rect(60, 40, 150, 50), "New Size: 1");
        GUI.Label(new Rect(60, 65, 150, 50), $"Mode: {mode}");
        // if (GUI.Button(new Rect(60, 105, 150, 20), "Change Mode"))
        // {
        //     mode = mode == Mode.Keyboard ? Mode.Mouse : Mode.Keyboard;
        // }
        GUI.Label(new Rect(60, 80, 150, 50), $"Side Length: {currentSideScale}");
        _textScale = GUI.TextField(new Rect(60, 130, 150, 20), $"{_textScale}");
        try
        {
            currentSideScale = float.Parse(_textScale);
        }
        catch (FormatException e)
        {
            currentSideScale = 1;
        }
    }
}