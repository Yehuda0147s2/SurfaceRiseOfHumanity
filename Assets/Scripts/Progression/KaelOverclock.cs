using System;
using UnityEngine;

namespace SurfaceRiseOfHumanity.Progression
{
    public sealed class KaelOverclock : MonoBehaviour
    {
        [SerializeField] private KaelProgression progression;
        [SerializeField] private KaelProgressionDefinition definition;
        public event Action<bool> StateChanged;
        public bool IsActive { get; private set; }
        public float MovementMultiplier => IsActive ? definition.overclockMovementMultiplier : 1f;
        public float ScannerMultiplier => IsActive ? definition.overclockScannerMultiplier : 1f;
        public float StaminaMultiplier => IsActive ? definition.overclockStaminaMultiplier : 1f;
        private float cooldownRemaining;
        private float activeRemaining;

        private void Awake()
        {
            if (progression == null) progression = GetComponent<KaelProgression>();
        }

        private void Update()
        {
            if (cooldownRemaining > 0f) cooldownRemaining -= Time.deltaTime;
            if (!IsActive) return;
            activeRemaining -= Time.deltaTime;
            if (activeRemaining <= 0f) Deactivate();
        }

        public bool TryActivate()
        {
            if (definition == null || IsActive || cooldownRemaining > 0f || progression == null) return false;
            if (!progression.TrySpendEnergy(definition.overclockEnergyCost)) return false;
            IsActive = true;
            activeRemaining = definition.overclockDuration;
            StateChanged?.Invoke(true);
            return true;
        }

        private void Deactivate()
        {
            IsActive = false;
            cooldownRemaining = definition != null ? definition.overclockCooldown : 0f;
            StateChanged?.Invoke(false);
        }
    }
}
