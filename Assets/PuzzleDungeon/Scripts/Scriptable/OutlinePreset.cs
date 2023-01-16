using UnityEngine;

namespace PuzzleDungeon.Scriptable
{
    [CreateAssetMenu(fileName = "_OutlinePreset", menuName = "PuzzleDungeon/OutlinePreset", order = 0)]
    public class OutlinePreset : ScriptableObject
    {
        [SerializeField]                 private OutlineFromPreset.Mode outlineMode;
        [SerializeField]                 private Color                  outlineColor = Color.white;
        [SerializeField, Range(0f, 10f)] private float                  outlineWidth = 2f;
        [Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
        + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
        [SerializeField]                 private bool                   precomputeOutline;

        public OutlineFromPreset.Mode OutlineMode         => outlineMode;
        public Color                  OutlineColor        => outlineColor;
        public float                  OutlineWidth        => outlineWidth;
        public bool                   PrecomputeOutline => precomputeOutline;
    }
}