using System;
using UnityEngine;
using SurfaceRiseOfHumanity.Core;
using SurfaceRiseOfHumanity.WorldState;

namespace SurfaceRiseOfHumanity.Progression
{
    public sealed class KaelProgression : MonoBehaviour
    {
        [SerializeField] private KaelProgressionDefinition definition;
        [SerializeField] private int level = 1;
        [SerializeField] private int experience;
        [SerializeField] private int skillPoints;
        [SerializeField] private int health;
        [SerializeField] private int armor;
        [SerializeField] private float stamina;
        [SerializeField] private float energy;
        [SerializeField] private int combat;
        [SerializeField] private int survival;
        [SerializeField] private int technology;

        public event Action<int> LevelChanged;
        public int Level => level;
        public int Experience => experience;
        public int SkillPoints => skillPoints;
        public int Health => health;
        public int Armor => armor;
        public float Stamina => stamina;
        public float Energy => energy;
        public int GetSkill(SkillBranch branch) => branch == SkillBranch.Combat ? combat : branch == SkillBranch.Survival ? survival : technology;

        private void Awake() => ResetToDefinitionIfNeeded();

        public void ResetToDefinitionIfNeeded()
        {
            if (definition == null || level > 0 && health > 0) return;
            level = definition.startingLevel;
            health = definition.startingHealth;
            armor = definition.startingArmor;
            stamina = definition.startingStamina;
            energy = definition.startingEnergy;
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0) return;
            experience += amount;
            int threshold = Mathf.Max(1, definition != null ? definition.xpToNextLevel : 100) * level;
            while (experience >= threshold)
            {
                experience -= threshold;
                level++;
                skillPoints++;
                threshold = Mathf.Max(1, definition != null ? definition.xpToNextLevel : 100) * level;
                LevelChanged?.Invoke(level);
            }
        }

        public bool SpendSkillPoint(SkillBranch branch)
        {
            if (skillPoints <= 0) return false;
            skillPoints--;
            if (branch == SkillBranch.Combat) combat++;
            else if (branch == SkillBranch.Survival) survival++;
            else technology++;
            return true;
        }

        public void RestoreEnergy(float amount) => energy = Mathf.Clamp(energy + amount, 0f, definition != null ? definition.startingEnergy : 100f);
        public bool TrySpendEnergy(float amount) { if (energy < amount) return false; energy -= amount; return true; }
        public void RestoreStamina(float amount) => stamina = Mathf.Clamp(stamina + amount, 0f, definition != null ? definition.startingStamina : 100f);

        public void ApplyPlayerState(PlayerState state)
        {
            if (state == null) return;

            level = Mathf.Max(1, state.level);
            experience = Mathf.Max(0, state.experience);
            skillPoints = Mathf.Max(0, state.skillPoints);
            health = Mathf.Clamp(state.health, 0, Mathf.Max(1, state.maxHealth));
            armor = Mathf.Max(0, state.armor);
            stamina = Mathf.Clamp(state.stamina, 0f, Mathf.Max(1f, state.maxStamina));
            energy = Mathf.Clamp(state.energy, 0f, Mathf.Max(1f, state.maxEnergy));

            if (definition != null)
            {
                definition.startingHealth = Mathf.Max(1, state.maxHealth);
                definition.startingEnergy = Mathf.Max(1f, state.maxEnergy);
                definition.startingStamina = Mathf.Max(1f, state.maxStamina);
            }
        }

        public PlayerState CapturePlayerState()
        {
            PlayerState state = new PlayerState();
            state.level = level;
            state.experience = experience;
            state.skillPoints = skillPoints;
            state.health = health;
            state.maxHealth = Mathf.Max(1, definition != null ? definition.startingHealth : 100);
            state.armor = armor;
            state.stamina = stamina;
            state.maxStamina = Mathf.Max(1f, definition != null ? definition.startingStamina : 100f);
            state.energy = energy;
            state.maxEnergy = Mathf.Max(1f, definition != null ? definition.startingEnergy : 100f);
            return state;
        }
    }
}
