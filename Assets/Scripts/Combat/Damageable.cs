using System;
using UnityEngine;

namespace SurfaceRiseOfHumanity.Combat
{
    public sealed class Damageable : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int armor;
        public event Action<int> Damaged;
        public event Action Died;
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }
        public int MaxHealth => maxHealth;

        private void Awake() => CurrentHealth = maxHealth;

        public void ApplyDamage(int amount, GameObject source = null)
        {
            if (IsDead || amount <= 0) return;
            int finalDamage = Mathf.Max(1, amount - armor);
            CurrentHealth = Mathf.Max(0, CurrentHealth - finalDamage);
            Damaged?.Invoke(finalDamage);
            if (CurrentHealth == 0) Die();
        }

        public void RestoreFullHealth()
        {
            IsDead = false;
            CurrentHealth = maxHealth;
        }

        private void Die()
        {
            IsDead = true;
            Died?.Invoke();
        }
    }
}
