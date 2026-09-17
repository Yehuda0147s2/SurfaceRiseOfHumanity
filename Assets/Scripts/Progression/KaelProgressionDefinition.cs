using UnityEngine;

namespace SurfaceRiseOfHumanity.Progression
{
    [CreateAssetMenu(menuName = "Surface/Kael Progression Definition", fileName = "KaelProgressionDefinition")]
    public sealed class KaelProgressionDefinition : ScriptableObject
    {
        [Header("Starting values")]
        public int startingLevel = 1;
        public int startingHealth = 100;
        public int startingArmor = 0;
        public float startingStamina = 100f;
        public float startingEnergy = 100f;
        public int xpToNextLevel = 100;

        [Header("Overclock")]
        public float overclockDuration = 8f;
        public float overclockEnergyCost = 35f;
        public float overclockCooldown = 12f;
        [Range(1f, 2f)] public float overclockMovementMultiplier = 1.15f;
        [Range(1f, 2f)] public float overclockScannerMultiplier = 1.25f;
        [Range(0f, 1f)] public float overclockStaminaMultiplier = 0.8f;
    }
}
