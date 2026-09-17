using UnityEngine;
using SurfaceRiseOfHumanity.Combat;
using SurfaceRiseOfHumanity.Progression;

namespace SurfaceRiseOfHumanity.Enemies
{
    public sealed class RobotScout : MonoBehaviour
    {
        private enum State { Idle, Patrol, Detect, Investigate, Chase, Attack, Dead }
        [SerializeField] private Damageable damageable;
        [SerializeField] private Transform target;
        [SerializeField] private float detectionRange = 20f;
        [SerializeField] private float attackRange = 12f;
        [SerializeField] private float moveSpeed = 2.2f;
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private int experienceReward = 100;
        [SerializeField] private float turnSpeed = 8f;
        private State state = State.Idle;
        private float attackTimer;
        private Damageable targetHealth;

        private void Awake()
        {
            if (damageable == null) damageable = GetComponent<Damageable>();
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
            if (damageable != null) damageable.Damaged += OnDamaged;
            if (damageable != null) damageable.Died += Die;
        }

        private void Update()
        {
            if (state == State.Dead || target == null) return;
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= detectionRange && distance > attackRange) state = State.Chase;
            else if (distance <= attackRange) state = State.Attack;
            else state = State.Patrol;
            if (state == State.Chase) MoveTowardTarget();
            if (state == State.Attack) AttackTarget();
        }

        private void MoveTowardTarget()
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
        }

        private void AttackTarget()
        {
            attackTimer -= Time.deltaTime;
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(direction);
            if (attackTimer > 0f) return;
            attackTimer = attackInterval;
            if (targetHealth == null) targetHealth = target.GetComponent<Damageable>();
            if (targetHealth != null) targetHealth.ApplyDamage(attackDamage, gameObject);
        }

        private void OnDamaged(int amount) { if (state != State.Dead) state = State.Chase; }
        private void Die()
        {
            state = State.Dead;
            KaelProgression playerProgression = FindObjectOfType<KaelProgression>();
            if (playerProgression != null) playerProgression.AddExperience(experienceReward);
            Destroy(gameObject, 3f);
        }
    }
}
