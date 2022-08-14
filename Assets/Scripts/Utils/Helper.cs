using UnityEngine;

namespace Utils
{
    public class Helper
    {
        private const float FloatTolerance = 0.01f;
        public static bool IsFloatEqual(float a, float b) => Mathf.Abs(a - b) <= FloatTolerance;
    }
}