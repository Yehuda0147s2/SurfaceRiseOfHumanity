using UnityEngine;

namespace SurfaceRiseOfHumanity.Exploration
{
    public sealed class PointOfInterestDiscovery : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private PointOfInterest[] pointsOfInterest;
        [SerializeField, Min(0.25f)] private float checkInterval = 1f;
        private float timer;

        private void Update()
        {
            if (player == null) return;
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            timer = checkInterval;
            for (int i = 0; i < pointsOfInterest.Length; i++)
                if (pointsOfInterest[i] != null) pointsOfInterest[i].TryDiscover(player.position);
        }
    }
}
