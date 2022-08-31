using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Core
{
    public enum AdjacentSide
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public struct SquareInfo
    {
        public Vector2 Min;
        public Vector2 Max;

        public override string ToString()
        {
            return $"Min: {Min.x}:{Min.y} Max: {Max.x}:{Max.y}";
        }
    }

    [Serializable]
    public struct SerializableSquare
    {
        public Vector3 position;
        public float sideScale;
        public string customName;
        public bool selected;
    }


    public class Square : IDisposable
    {
        public GameObject GameObject { get; }
        private Square _up;
        private Square _down;
        private Square _left;
        private Square _right;
        public readonly float SideScale;

        public Square(GameObject square, float sideScale = 1)
        {
            GameObject = square;
            SideScale = sideScale;
            square.transform.localScale = new Vector3(sideScale, sideScale, square.transform.localScale.z);
        }


        private SquareInfo GetRect()
        {
            var position = GameObject.transform.position;
            return new SquareInfo
            {
                Min = new Vector2(position.x - SideScale / 2, position.y - SideScale / 2),
                Max = new Vector2(position.x + SideScale / 2, position.y + SideScale / 2)
            };
        }

        public bool CheckIfOverlapping(Vector3 position, float scale)
        {
            var targetRect = new Rect(position.x, position.y, scale, scale);

            var pos = GetPosition();
            var selfRect = new Rect(pos.x, pos.y, SideScale, SideScale);
            return selfRect.Overlaps(targetRect);
        }

        private Vector2 GetPosition()
        {
            var pos = GameObject.transform.position;
            return new Vector2(pos.x, pos.y);
        }

        public AdjacentSide CheckIfAdjacent(Square target)
        {
            var targetRect = target.GetRect();
            var selfRect = GetRect();

            //check up
            if (Helper.IsFloatEqual(targetRect.Min.y, selfRect.Max.y) &&
                ((targetRect.Min.x >= selfRect.Min.x && targetRect.Max.x <= selfRect.Max.x) ||
                 (targetRect.Min.x <= selfRect.Min.x && targetRect.Max.x >= selfRect.Max.x)))
            {
                return AdjacentSide.Up;
            }

            //check down
            if (Helper.IsFloatEqual(targetRect.Max.y, selfRect.Min.y) &&
                ((targetRect.Min.x >= selfRect.Min.x && targetRect.Max.x <= selfRect.Max.x) ||
                 (targetRect.Min.x <= selfRect.Min.x && targetRect.Max.x >= selfRect.Max.x)))
            {
                return AdjacentSide.Down;
            }

            //check left
            if (Helper.IsFloatEqual(targetRect.Min.x, selfRect.Max.x) &&
                ((targetRect.Min.y >= selfRect.Min.y && targetRect.Max.y <= selfRect.Max.y) ||
                 (targetRect.Min.y <= selfRect.Min.y && targetRect.Max.y >= selfRect.Max.y)))
            {
                return AdjacentSide.Right;
            }

            //check right
            if (Helper.IsFloatEqual(targetRect.Max.x, selfRect.Min.x) &&
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

            if (_up != null)
                result.Add(_up.GameObject.name, SideScale/2+_up.SideScale/2);

            if (_down != null)
                result.Add(_down.GameObject.name, SideScale/2+_down.SideScale/2);

            if (_left != null)
                result.Add(_left.GameObject.name, SideScale/2+_left.SideScale/2);

            if (_right != null)
                result.Add(_right.GameObject.name, SideScale/2+_right.SideScale/2);

            return result;
        }
        
        public static void SetAdjacentSquare(Dictionary<string, Square> squares, Square target)
        {
            foreach (var pair in squares)
            {
                var square = pair.Value;
                var result = target.CheckIfAdjacent(square);
                switch (result)
                {
                    case AdjacentSide.Up:
                        target._up = square;
                        square._down = target;
                        break;
                    case AdjacentSide.Down:
                        target._down = square;
                        square._up = target;
                        break;
                    case AdjacentSide.Left:
                        target._left = square;
                        square._right = target;
                        break;
                    case AdjacentSide.Right:
                        target._right = square;
                        square._left = target;
                        break;
                    case AdjacentSide.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public UI.SquareControl GetControl() => GameObject.GetComponent<UI.SquareControl>();

        public override string ToString()
        {
            return
                $"Name:{GameObject.name}\t Up: {_up?.GameObject.name}\t Down: {_down?.GameObject.name}\t Left: {_left?.GameObject.name}\t Right: {_right?.GameObject.name}";
        }

        public void Dispose()
        {
            if (_up != null)
            {
                _up._down = null;
                _up = null;
            }

            if (_down != null)
            {
                _down._up = null;
                _down = null;
            }

            if (_left != null)
            {
                _left._right = null;
                _left = null;
            }

            if (_right != null)
            {
                _right._left = null;
                _right = null;
            }

            UnityEngine.Object.Destroy(GameObject);
        }
    }
}