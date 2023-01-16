using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Tools
{
    public static class Tools
    {
        public static bool LayerInLayerMask(LayerMask layerMask, int layer) => layerMask == (layerMask | (1 << layer));
    }
}