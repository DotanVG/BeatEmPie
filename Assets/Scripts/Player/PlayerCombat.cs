using UnityEngine;
using UnityEngine.InputSystem;

namespace BeatEmPie
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Pie Throwing")]
        [SerializeField] Transform throwOrigin;
        [SerializeField] float throwCooldown = 0.5f;

        Animator animator;
        PlayerStats stats;
        float cooldownTimer;

        static readonly int AttackHash = Animator.StringToHash("Attack");

        void Awake()
        {
            animator = GetComponent<Animator>();
            stats    = GetComponent<PlayerStats>();
        }

        void Update()
        {
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;
        }

        // Called by Unity Input System via PlayerInput component
        public void OnAttack(InputValue value)
        {
            if (!value.isPressed || stats.IsDead || cooldownTimer > 0f) return;

            animator.SetTrigger(AttackHash);
            cooldownTimer = throwCooldown;
            // Pie instantiation will be added when PieBase prefabs exist
        }

        public void OnSwitchPieNext(InputValue value)
        {
            if (value.isPressed)
                Debug.Log("[PlayerCombat] Switch pie next — PieInventory not yet wired");
        }

        public void OnSwitchPiePrev(InputValue value)
        {
            if (value.isPressed)
                Debug.Log("[PlayerCombat] Switch pie prev — PieInventory not yet wired");
        }
    }
}
