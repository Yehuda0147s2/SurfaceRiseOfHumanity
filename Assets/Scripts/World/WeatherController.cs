using UnityEngine;

namespace SurfaceRiseOfHumanity.World
{
    public sealed class WeatherController : MonoBehaviour
    {
        public enum WeatherState { Clear, Rain }
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private AudioSource rainAudio;
        [SerializeField] private WeatherState state = WeatherState.Clear;
        public WeatherState State => state;

        public void SetWeather(WeatherState newState)
        {
            state = newState;
            bool raining = state == WeatherState.Rain;
            if (rainParticles != null)
            {
                if (raining) rainParticles.Play();
                else rainParticles.Stop();
            }
            if (rainAudio != null) rainAudio.mute = !raining;
        }
    }
}
