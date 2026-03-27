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

        [Header("Sprites")]
        [SerializeField] Sprite spriteIdle;
        [SerializeField] Sprite spriteWalk2;
        [SerializeField] Sprite spriteJump;
        [SerializeField] float walkFrameInterval = 0.15f;  // seconds per walk frame

        Rigidbody2D rb;
        PlayerStats stats;
        SpriteRenderer spriteRenderer;

        Vector2 moveInput;
        bool jumpQueued;
        bool isGrounded;

        // Walk alternation
        bool walkFrame;       // false = idle frame, true = walk2 frame
        float walkTimer;

        void Awake()
        {
            rb             = GetComponent<Rigidbody2D>();
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
            UpdateSprite();
            FlipSprite();
        }

        void FixedUpdate()
        {
            if (stats.IsDead) return;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

            if (jumpQueued)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpQueued = false;
            }
        }

        void CheckGrounded()
        {
            isGrounded = groundCheck != null &&
                         Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        void UpdateSprite()
        {
            if (!isGrounded)
            {
                // In the air — hold jump sprite
                spriteRenderer.sprite = spriteJump != null ? spriteJump : spriteIdle;
                walkTimer = 0f;
                walkFrame = false;
                return;
            }

            bool moving = Mathf.Abs(moveInput.x) > 0.01f;

            if (moving)
            {
                // Alternate idle ↔ walk2 every walkFrameInterval
                walkTimer -= Time.deltaTime;
                if (walkTimer <= 0f)
                {
                    walkFrame = !walkFrame;
                    walkTimer = walkFrameInterval;
                }
                spriteRenderer.sprite = walkFrame
                    ? (spriteWalk2 != null ? spriteWalk2 : spriteIdle)
                    : (spriteIdle  != null ? spriteIdle  : spriteRenderer.sprite);
            }
            else
            {
                // Standing still — idle sprite, reset walk cycle
                spriteRenderer.sprite = spriteIdle != null ? spriteIdle : spriteRenderer.sprite;
                walkTimer = 0f;
                walkFrame = false;
            }
        }

        void FlipSprite()
        {
            // Sprite naturally faces left, so flip when moving right
            if (moveInput.x != 0)
                spriteRenderer.flipX = moveInput.x > 0;
        }

        void OnDeath() => enabled = false;
    }
}
