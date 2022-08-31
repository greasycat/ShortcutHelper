using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UI;
using UnityEngine;
using Utils;
using Grid = UI.Grid;

namespace Core
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private GameObject squarePrefab;
        [SerializeField] private new Camera camera;

        public Dictionary<string, Square> Squares;

        private int _maxSquareCounter;

        [SerializeField] public float currentSideScale = 1;
        private string _textScale;

        //UI
        //Background
        public Grid grid;
        public Background background;

        //Message
        public TMP_Text message;

        //Toolbar
        [NonSerialized] public Tool CurrentTool = Tool.Pen;


        //Input
        [SerializeField] private Canvas inputCanvas;
        [SerializeField] private InputControl inputControl;
        [SerializeField] private TMP_InputField inputField;

        //Active square
        [NonSerialized] public SquareControl SquareToEdit;

        //Selected square
        [NonSerialized] public List<string> SelectedSquareNames;

        //Shortest distance
        // public Dictionary<string, float> ShortestDistance;
        public Dictionary<string, Dictionary<string, float>> ShortestDistance;
        [SerializeField] private string guiLabelOutput = "";

        //Save/Load
        [SerializeField] public string configFilePath = "maze.json";
        private List<SerializableSquare> _serializedSquareList;


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
        }

        public void NewSquare(Vector3 position)
        {
            if (CheckPositionOverlapping(position)) return;
            var target = new Square(Instantiate(squarePrefab, position, Quaternion.identity), currentSideScale);
            Square.SetAdjacentSquare(Squares, target); //Update adjacency info

            _maxSquareCounter += 1; //Add max counter;
            target.GameObject.name = $"Square@{_maxSquareCounter}";
            Squares[target.GameObject.name] = target;
        }

        private bool CheckPositionOverlapping(Vector3 position)
        {
            return Squares.Values.Any(square => square.CheckIfOverlapping(position, currentSideScale));
        }

        private void LoadASquare(Vector3 position, float sideScale = 1, bool selected = false, string customName = "T")
        {
            var target = new Square(Instantiate(squarePrefab, position, Quaternion.identity), sideScale);

            Square.SetAdjacentSquare(Squares, target); //Update adjacency info
            _maxSquareCounter += 1; //Add max counter;
            target.GameObject.name = $"Square@{_maxSquareCounter}";
            Squares[target.GameObject.name] = target;
            if (selected)
                target.GetControl().Select(customName);
        }


        public void DeleteSquare(string squareName)
        {
            Squares[squareName].Dispose();
            Squares.Remove(squareName);
        }

        public void ClickOneThird()
        {
            currentSideScale = 1f / 3f;
            grid.DrawGrid();
        }


        private void SaveSquareConfiguration(string filePath)
        {
            var serializedList = Squares.Select(pair => new SerializableSquare
            {
                position = pair.Value.GameObject.transform.position, sideScale = pair.Value.SideScale,
                selected = pair.Value.GetControl().Selected, customName = pair.Value.GetControl().SquareName
            }).ToList();
            var output = JsonConvert.SerializeObject(serializedList);
            if (string.IsNullOrEmpty(filePath)) return;
            try
            {
                File.WriteAllText(filePath, output);
            }
            catch (Exception e)
            {
                StartCoroutine(ShowMessageCoroutine(e.Message));
                Debug.Log(e.Message);
            }
        }

        private void LoadSquareConfiguration(string filePath)
        {
            foreach (var square in Squares.Values)
            {
                square.GetControl().DeleteSquare();
            }
            try
            {
                var input = File.ReadAllText(filePath);
                var serializedList = JsonConvert.DeserializeObject<List<SerializableSquare>>(input);
                foreach (var square in serializedList)
                {
                    LoadASquare(square.position, square.sideScale, square.selected, square.customName);
                }
            }
            catch (Exception e)
            {
                StartCoroutine(ShowMessageCoroutine(e.Message));
                Debug.Log(e.Message);
            }
        }

        public void SetOnScreenInputAction(Action<string, bool> setTextAction, string placeholder = "")
        {
            inputCanvas.gameObject.SetActive(true);
            inputControl.SetTextAction(setTextAction, placeholder);
        }

        public void ClickLoad()
        {
            SetOnScreenInputAction((string text, bool success) =>
                {
                    if (success)
                        LoadSquareConfiguration(text);
                }
                , "maze.json");
        }

        public void ClickSave()
        {
            SetOnScreenInputAction((string text, bool success) =>
                {
                    if (success)
                        SaveSquareConfiguration(text);
                }
                , "maze.json");
        }

        public void ClickCalculate()
        {
            try
            {
                Shortcut.CalculateAll();
                var output = "Source, Destination, Distance\r\n";
                foreach (var (source, value) in ShortestDistance)
                {
                    output = value.Select(p2 => p2.Key).Aggregate(output, (current, destination) => current + $"{Squares[source].GetControl().SquareName},{Squares[destination].GetControl().SquareName}, {ShortestDistance[source][destination]}\r\n");
                }

                File.WriteAllText("shortcuts.csv", output);
            }
            catch (Exception exception)
            {
                StartCoroutine(ShowMessageCoroutine(exception.Message));
            }

            StartCoroutine(ShowInfoCoroutine("Calculations saved to shortcuts.csv"));
        }


        public IEnumerator ShowMessageCoroutine(string text, float duration = 2f)
        {
            message.SetText(text);
            message.color = Color.red;
            yield return new WaitForSeconds(duration);
            message.SetText("");
        }

        public IEnumerator ShowInfoCoroutine(string text, float duration = 2f)
        {
            message.SetText(text);
            message.color = Color.green;
            yield return new WaitForSeconds(duration);
            message.SetText("");
        }

    }
}