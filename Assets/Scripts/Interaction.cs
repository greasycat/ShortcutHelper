using System;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private new Camera camera;

    private Square _currentSquare;
    public Dictionary<string, Square> Squares;
    private int _maxSquareCounter;
    [SerializeField] public float currentSideScale = 1;
    private string _textScale;
    public Grid grid;
    public Background background;
    [NonSerialized] public List<string> SelectedSquareNames;

    public Dictionary<string, float> ShortestDistance;
    [SerializeField] private float floatTolerance = 0.01f;
    [SerializeField] private string guiLabelOutput = "";

    public enum AdjacentSide
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


    public class Square : IDisposable
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

        public Dictionary<string, float> GetAdjacentSquares()
        {
            var result = new Dictionary<string, float>();

            if (Up != null)
                result.Add(Up.Self.name, _sideScale);

            if (Down != null)
                result.Add(Down.Self.name, _sideScale);

            if (Left != null)
                result.Add(Left.Self.name, _sideScale);

            if (Right != null)
                result.Add(Right.Self.name, _sideScale);

            return result;
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
        Squares = new Dictionary<string, Square>();
        SelectedSquareNames = new List<string>();
    }


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            PrintDebugInfo();

        if (Input.GetKeyUp(KeyCode.Equals))
        {
            if (camera.orthographicSize > 1)
            {
                camera.orthographicSize -= 1;
                background.ResizeSpriteToScreen();
                grid.DrawGrid();
            }
        }

        if (Input.GetKeyUp(KeyCode.Minus))
        {
            camera.orthographicSize += 1;
            background.ResizeSpriteToScreen();
            grid.DrawGrid();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            _textScale = "1";
            grid.DrawGrid();
        }
        
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            
            _textScale = "0.5";
            grid.DrawGrid();
        }
        
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            
            _textScale = "0.25";
            grid.DrawGrid();
        }
        
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            _textScale = "0.125";
            grid.DrawGrid();
        }
    }

    private void PrintDebugInfo()
    {
        foreach (var square in Squares)
        {
            Debug.Log(square);
        }
    }

    public Square NewSquare(Vector3 position)
    {
        var square = new Square(Instantiate(squarePrefab, position, Quaternion.identity), currentSideScale);
        SetAdjacentSquare(square); //Update adjacency info

        _maxSquareCounter += 1; //Add max counter;
        square.Self.name = $"Square@{_maxSquareCounter}";
        Squares[square.Self.name] = square;

        return square;
    }

    public void DeleteSquare(string squareName)
    {
        Squares[squareName].Dispose();
        Squares.Remove(squareName);
    }


    private void SetAdjacentSquare(Square target)
    {
        foreach (var pair in Squares)
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

    private void SaveSquareConfiguration()
    {
        
    }


    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(50, 25, 400, 180));
        GUI.Box(new Rect(10, 0, 400, 160), "");
        GUI.Label(new Rect(10, 20, 150, 20), "New Size: 1");
        GUI.Label(new Rect(10, 40, 150, 20), $"Side Length: {currentSideScale}");
        GUI.Label(new Rect(10, 60, 100, 20), $"Enter Length:");
        _textScale = GUI.TextField(new Rect(100, 60, 50, 20), $"{_textScale}", 5);
        try
        {
            currentSideScale = float.Parse(_textScale);
            grid.DrawGrid();
        }
        catch (FormatException)
        {
            currentSideScale = 1;
        }
        GUI.Label(new Rect(10, 80, 250, 40), $"You can also use key 1234 to change");

        if (GUI.Button(new Rect(200, 20, 150, 20), "Calculate Shortest Path"))
        {
            Shortcut.Calculate();
            if (SelectedSquareNames is {Count:> 1} && ShortestDistance is {Count: > 1}) 
            {
                var output = string.Empty;
                for (var i = 1; i<SelectedSquareNames.Count; ++i)
                {
                    output += $"{SelectedSquareNames[i-1]}==>{SelectedSquareNames[i]}: {ShortestDistance[SelectedSquareNames[i]]}\n";
                }

                guiLabelOutput = output;
            }
        }

        GUI.EndGroup();
        
        GUI.BeginGroup(new Rect(50, 200, 400, 180));
        GUI.Box(new Rect(10, 0, 400, 180), "");
        GUI.Label(new Rect(10, 20, 400, 200), guiLabelOutput);
        GUI.EndGroup();
    }
}