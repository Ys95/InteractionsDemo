using System;
using Random = UnityEngine.Random;

namespace PuzzleDungeon.Tools
{
    [Serializable]
    public struct MinMaxFloat
    {
        public float Min;
        public float Max;

        public float Average => Min + Max / 2;

        public float Roll() => Random.Range(Min, Max);
    }
}