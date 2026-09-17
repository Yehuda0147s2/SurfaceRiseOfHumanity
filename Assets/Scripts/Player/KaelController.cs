using UnityEngine;
using SurfaceRiseOfHumanity.Progression;

namespace SurfaceRiseOfHumanity.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class KaelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private KaelProgression progression;
        [SerializeField] private KaelOverclock overclock;
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 2.2f;
        [SerializeField] private float runSpeed = 4.2f;
        [SerializeField] private float sprintSpeed = 6.2f;
        [SerializeField] private float rotationSharpness = 14f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float crouchHeight = 1.1f;
        [SerializeField] private float standingHeight = 1.8f;
        [SerializeField] private float dodgeSpeed = 8f;
        [SerializeField] private float dodgeDuration = 0.22f;
        [SerializeField] private float dodgeCooldown = 0.8f;

        private CharacterController characterController;
        private Vector2 moveInput;
        private float verticalVelocity;
        private float dodgeRemaining;
        private float dodgeCooldownRemaining;
        private bool sprintHeld;
        private bool crouchHeld;

        public bool IsGrounded => characterController != null && characterController.isGrounded;
        public bool IsCrouching => crouchHeld;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (progression == null) progression = GetComponent<KaelProgression>();
            if (overclock == null) overclock = GetComponent<KaelOverclock>();
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            sprintHeld = Input.GetKey(KeyCode.LeftShift);
            crouchHeld = Input.GetKey(KeyCode.LeftControl);
            if (Input.GetButtonDown("Jump")) Jump();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.LeftAlt)) TryDodge();
            Move(moveInput, sprintHeld, crouchHeld);
        }

        public void SetMoveInput(Vector2 input) => moveInput = Vector2.ClampMagnitude(input, 1f);
        public void SetSprint(bool held) => sprintHeld = held;
        public void SetCrouch(bool held) => crouchHeld = held;
        public void Jump()
        {
            if (!IsGrounded || crouchHeld) return;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        public bool TryDodge()
        {
            if (dodgeCooldownRemaining > 0f || dodgeRemaining > 0f || moveInput.sqrMagnitude < 0.01f) return false;
            dodgeRemaining = dodgeDuration;
            dodgeCooldownRemaining = dodgeCooldown;
            return true;
        }

        private void Move(Vector2 input, bool sprint, bool crouch)
        {
            dodgeCooldownRemaining = Mathf.Max(0f, dodgeCooldownRemaining - Time.deltaTime);
            Vector3 direction = GetCameraRelativeDirection(input);
            float speed = input.magnitude > 0.05f ? (sprint ? sprintSpeed : runSpeed) : 0f;
            if (crouch) speed = walkSpeed * 0.55f;
            if (overclock != null) speed *= overclock.MovementMultiplier;
            if (dodgeRemaining > 0f)
            {
                dodgeRemaining -= Time.deltaTime;
                direction = direction.sqrMagnitude > 0.01f ? direction : transform.forward;
                speed = dodgeSpeed;
            }

            if (IsGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = direction * speed + Vector3.up * verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSharpness * Time.deltaTime);
            }
            UpdateCapsule(crouch);
        }

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            if (cameraTransform == null) return new Vector3(input.x, 0f, input.y).normalized;
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            return Vector3.ClampMagnitude(right * input.x + forward * input.y, 1f);
        }

        private void UpdateCapsule(bool crouched)
        {
            float targetHeight = crouched ? crouchHeight : standingHeight;
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, 12f * Time.deltaTime);
            characterController.center = Vector3.up * characterController.height * 0.5f;
        }
    }
}
