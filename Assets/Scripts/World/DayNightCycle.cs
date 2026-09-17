using UnityEngine;

namespace SurfaceRiseOfHumanity.World
{
    public sealed class DayNightCycle : MonoBehaviour
    {
        [SerializeField] private Light sun;
        [SerializeField] private float dayLengthSeconds = 900f;
        [Range(0f, 1f)] [SerializeField] private float normalizedTime = 0.25f;
        [SerializeField] private Gradient sunColor;
        [SerializeField] private AnimationCurve sunIntensity = AnimationCurve.EaseInOut(0f, 0.05f, 0.5f, 1f);
        public bool IsNight => normalizedTime < 0.22f || normalizedTime > 0.78f;

        private void Update()
        {
            normalizedTime = Mathf.Repeat(normalizedTime + Time.deltaTime / Mathf.Max(1f, dayLengthSeconds), 1f);
            if (sun == null) return;
            sun.transform.rotation = Quaternion.Euler(normalizedTime * 360f - 90f, -25f, 0f);
            sun.intensity = sunIntensity.Evaluate(normalizedTime);
            if (sunColor != null) sun.color = sunColor.Evaluate(normalizedTime);
        }
    }
}
