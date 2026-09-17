using UnityEngine;

namespace SurfaceRiseOfHumanity.Player
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform aimTarget;
        [SerializeField] private float followSharpness = 14f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float defaultDistance = 4.5f;
        [SerializeField] private float aimDistance = 2.6f;
        [SerializeField] private float minPitch = -35f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private LayerMask collisionMask = ~0;
        [SerializeField] private float collisionRadius = 0.2f;
        [SerializeField] private float collisionPadding = 0.15f;

        private float yaw;
        private float pitch = 15f;
        private Vector2 lookInput;
        private bool aiming;

        public void SetLookInput(Vector2 input) => lookInput = input;
        public void SetAiming(bool value) => aiming = value;

        private void LateUpdate()
        {
            if (target == null) return;
            yaw += lookInput.x * rotationSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch - lookInput.y * rotationSpeed * Time.deltaTime, minPitch, maxPitch);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            float distance = aiming ? aimDistance : defaultDistance;
            Vector3 pivot = target.position;
            Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;
            Vector3 position = ResolveCollision(pivot, desiredPosition);
            transform.position = Vector3.Lerp(transform.position, position, followSharpness * Time.deltaTime);
            transform.rotation = rotation;
        }

        private Vector3 ResolveCollision(Vector3 pivot, Vector3 desired)
        {
            Vector3 direction = desired - pivot;
            float distance = direction.magnitude;
            if (distance <= 0.01f) return desired;
            direction /= distance;
            if (Physics.SphereCast(pivot, collisionRadius, direction, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
                return pivot + direction * Mathf.Max(0.1f, hit.distance - collisionPadding);
            return desired;
        }
    }
}
