using UnityEngine;
using UnityEngine.InputSystem;

namespace BeatEmPie
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float jumpForce = 10f;

        [Header("Ground Check")]
        [SerializeField] Transform groundCheck;
        [SerializeField] float groundCheckRadius = 0.15f;
        [SerializeField] LayerMask groundLayer;

        Rigidbody2D rb;
        Animator animator;
        PlayerStats stats;
        SpriteRenderer spriteRenderer;

        Vector2 moveInput;
        bool jumpQueued;
        bool isGrounded;

        static readonly int SpeedHash      = Animator.StringToHash("Speed");
        static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        static readonly int JumpStartHash  = Animator.StringToHash("JumpStart");

        void Awake()
        {
            rb             = GetComponent<Rigidbody2D>();
            animator       = GetComponent<Animator>();
            stats          = GetComponent<PlayerStats>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnEnable()  => stats.OnDeath += OnDeath;
        void OnDisable() => stats.OnDeath -= OnDeath;

        public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

        public void OnJump(InputValue value)
        {
            if (value.isPressed && isGrounded)
                jumpQueued = true;
        }

        void Update()
        {
            CheckGrounded();
            FlipSprite();
            if (animator != null) UpdateAnimator();
        }

        void FixedUpdate()
        {
            if (stats.IsDead) return;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

            if (jumpQueued)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (animator != null) animator.SetTrigger(JumpStartHash);
                jumpQueued = false;
            }
        }

        void CheckGrounded()
        {
            isGrounded = groundCheck != null &&
                         Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        void UpdateAnimator()
        {
            animator.SetFloat(SpeedHash, Mathf.Abs(moveInput.x));
            animator.SetBool(IsGroundedHash, isGrounded);
        }

        void FlipSprite()
        {
            if (moveInput.x != 0)
                spriteRenderer.flipX = moveInput.x < 0;
        }

        void OnDeath()
        {
            if (animator != null) animator.SetTrigger(Animator.StringToHash("Die"));
            enabled = false;
        }
    }
}
