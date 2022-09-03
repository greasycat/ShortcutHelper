using UnityEngine;

namespace Utils
{
    public static class Helper
    {
        private const float FloatTolerance = 0.01f;
        public static bool IsFloatEqual(float a, float b) => Mathf.Abs(a - b) <= FloatTolerance;
        
        public static Vector2 GetNearestSquareCenter(Vector2 sourceVector, float unit)
        {
            var x = sourceVector.x;
            var y = sourceVector.y;
            var cx = Mathf.Sign(x) * Mathf.Ceil(Mathf.Abs(x) / unit) * unit;
            var cy = Mathf.Sign(y) * Mathf.Ceil(Mathf.Abs(y) / unit) * unit;
            var fx = Mathf.Sign(x) * Mathf.Floor(Mathf.Abs(x) / unit) * unit;
            var fy = Mathf.Sign(y) * Mathf.Floor(Mathf.Abs(y) / unit) * unit;

            return new Vector2((cx + fx) / 2, (cy + fy) / 2);
        }

        public static bool IsOdd(int num)
        {
            return num % 2 == 1;
        }

        public static bool IsEven(int num)
        {
            return num % 2 == 0;
        }

        public static int TwoDimensionToOne(int x, int y, int width)
        {
            return x + width * y;
        }

        public static Vector2 OneDimensionToTwo(int i, int width)
        {
            var x = i % width;
            var y = (i - x) / width;
            return new Vector2(x, y);
        }

    }
}