using UnityEngine;
using SurfaceRiseOfHumanity.Combat;

namespace SurfaceRiseOfHumanity.Weapons
{
    public sealed class AR7SurvivorRifle : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private int damage = 25;
        [SerializeField] private int magazineSize = 30;
        [SerializeField] private int reserveAmmo = 120;
        [SerializeField] private float roundsPerSecond = 8f;
        [SerializeField] private float range = 150f;
        [SerializeField] private float reloadDuration = 1.6f;
        private int magazine;
        private float fireCooldown;
        private float reloadRemaining;
        public int Magazine => magazine;
        public int ReserveAmmo => reserveAmmo;
        public bool IsReloading => reloadRemaining > 0f;

        private void Awake() { magazine = magazineSize; if (aimCamera == null) aimCamera = Camera.main; }
        private void Update()
        {
            fireCooldown = Mathf.Max(0f, fireCooldown - Time.deltaTime);
            if (reloadRemaining <= 0f) return;
            reloadRemaining -= Time.deltaTime;
            if (reloadRemaining <= 0f) FinishReload();
        }

        public bool TryFire()
        {
            if (IsReloading || fireCooldown > 0f) return false;
            if (magazine <= 0) { TryReload(); return false; }
            fireCooldown = 1f / roundsPerSecond;
            magazine--;
            Camera cameraToUse = aimCamera != null ? aimCamera : Camera.main;
            if (cameraToUse == null) return true;
            Ray ray = cameraToUse.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                Damageable target = hit.collider.GetComponentInParent<Damageable>();
                if (target != null) target.ApplyDamage(damage, gameObject);
            }
            return true;
        }

        public bool TryReload()
        {
            if (IsReloading || magazine >= magazineSize || reserveAmmo <= 0) return false;
            reloadRemaining = reloadDuration;
            return true;
        }

        private void FinishReload()
        {
            int needed = magazineSize - magazine;
            int loaded = Mathf.Min(needed, reserveAmmo);
            magazine += loaded;
            reserveAmmo -= loaded;
        }
    }
}
