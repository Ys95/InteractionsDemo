using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using PuzzleDungeon.Scriptable;
using UnityEngine;

namespace PuzzleDungeon.Tools
{
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private LightFlickerSetup preset;

        private Light                                   _light;
        private TweenerCore<float, float, FloatOptions> _intensityFlicker;
        private TweenerCore<float, float, FloatOptions> _radiusFlicker;

        public LightFlickerSetup P_Preset => preset;

        public void Reload()
        {
            if (!Application.IsPlaying(gameObject))
            {
                if (preset == null)
                {
                    return;
                }

                if (preset.P_FlickerRadius)
                {
                    GetComponent<Light>().range = preset.P_RadiusRange.Min;
                }

                if (preset.P_FlickerIntensity)
                {
                    GetComponent<Light>().intensity = preset.P_IntensityRange.Min;
                }

                return;
            }

            Invoke(nameof(CreateIntensityTween), preset.P_RandomTimeToStart.Roll());
            Invoke(nameof(CreateRadiusTween),    preset.P_RandomTimeToStart.Roll());
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        private void OnEnable()
        {
            Reload();
        }

        private void CreateRadiusTween()
        {
            if (!preset.P_FlickerRadius)
            {
                return;
            }

            if (_radiusFlicker != null)
            {
                _radiusFlicker.Kill();
            }

            _light.range = preset.P_RadiusRange.Min;
            _radiusFlicker = DOTween.To(() => _light.range, x => _light.range = x, preset.P_RadiusRange.Max, preset.P_RadiusFlickerDuration)
                                    .SetEase(preset.P_RadiusEase)
                                    .SetLoops(-1, LoopType.Yoyo);
            _radiusFlicker.Play();
        }

        private void CreateIntensityTween()
        {
            if (!preset.P_FlickerIntensity)
            {
                return;
            }

            if (_intensityFlicker != null)
            {
                _intensityFlicker.Kill();
            }

            _light.intensity = preset.P_IntensityRange.Min;
            _intensityFlicker = _light.DOIntensity(preset.P_IntensityRange.Max, preset.P_IntensityFlickerDuration)
                                      .SetEase(preset.P_IntensityEase)
                                      .SetLoops(-1, LoopType.Yoyo);
            _intensityFlicker.Play();
        }

        private void OnDisable()
        {
            if (_intensityFlicker != null)
            {
                _intensityFlicker.Kill();
            }

            if (_radiusFlicker != null)
            {
                _radiusFlicker.Kill();
            }
        }
    }
}