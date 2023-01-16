using DG.Tweening;
using PuzzleDungeon.Tools;
using UnityEditor;
using UnityEngine;

namespace PuzzleDungeon.Scriptable
{
    [CreateAssetMenu(fileName = "_LightFlickerSetup", menuName = "PuzzleDungeon/LightsFlicker", order = 0)]
    public class LightFlickerSetup : ScriptableObject
    {
        [SerializeField] private MinMaxFloat randomTimeToStart;

        [Header("Radius")]
        [SerializeField] private bool flickerRadius;
        [SerializeField] private MinMaxFloat radiusRange;
        [SerializeField] private float       radiusFlickerDuration;
        [SerializeField] private Ease        radiusEase;

        [Header("Intensity")]
        [SerializeField] private bool flickerIntensity;
        [SerializeField] private MinMaxFloat intensityRange;
        [SerializeField] private float       intensityFlickerDuration;
        [SerializeField] private Ease        intensityEase;

        public Ease        P_RadiusEase               => radiusEase;
        public Ease        P_IntensityEase            => intensityEase;
        public MinMaxFloat P_RadiusRange              => radiusRange;
        public MinMaxFloat P_IntensityRange           => intensityRange;
        public MinMaxFloat P_RandomTimeToStart        => randomTimeToStart;
        public bool        P_FlickerRadius            => flickerRadius;
        public bool        P_FlickerIntensity         => flickerIntensity;
        public float       P_RadiusFlickerDuration    => radiusFlickerDuration;
        public float       P_IntensityFlickerDuration => intensityFlickerDuration;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LightFlickerSetup))]
    public class LightFlickerSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Reload lights"))
            {
                var lightWithThisPreset = GameObject.FindObjectsOfType<LightFlicker>();
                foreach (var light in lightWithThisPreset)
                {
                    light.Reload();
                }
            }
        }
    }
#endif
}