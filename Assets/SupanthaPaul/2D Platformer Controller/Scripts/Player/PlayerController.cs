using UnityEngine;

namespace SupanthaPaul
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private float speed;
		[Header("Jumping")]
		[SerializeField] private float jumpForce;
		[SerializeField] private float fallMultiplier;
		[SerializeField] private Transform groundCheck;
		[SerializeField] private Vector2 groundCheckSize;
		[SerializeField] private LayerMask whatIsGround;
		[SerializeField] private int extraJumpCount = 1;
		[SerializeField] private GameObject jumpEffect;
		[Header("Dashing")]
		[SerializeField] private float dashSpeed = 30f;
		[Tooltip("Amount of time (in seconds) the player will be in the dashing speed")]
		[SerializeField] private float startDashTime = 0.1f;
		[Tooltip("Time (in seconds) between dashes")]
		[SerializeField] private float dashCooldown = 0.2f;
		[SerializeField] private GameObject dashEffect;

		// Access needed for handling animation in Player script and other uses
		[HideInInspector] public bool isGrounded;
		[HideInInspector] public float moveInput;
		[HideInInspector] public bool canMove = true;

		[HideInInspector] public bool isDashing = false;
		[HideInInspector] public bool isAttacking = false;
		[HideInInspector] public bool isDead = false;

		[HideInInspector] public bool actuallyWallGrabbing = false;
		// controls whether this instance is currently playable or not
		[HideInInspector] public bool isCurrentlyPlayable = false;

		[Header("Wall grab & jump")]
		[Tooltip("Right offset of the wall detection sphere")]
		public Vector2 grabRightOffset = new(0.16f, 0f);
		public Vector2 grabLeftOffset = new(-0.16f, 0f);
		public float grabCheckRadius = 0.24f;
		public float slideSpeed = 2.5f;
		public Vector2 wallJumpForce = new(10.5f, 18f);
		public Vector2 wallClimbForce = new(4f, 14f);

		private Rigidbody2D m_rb;
		[SerializeField] private ParticleSystem m_dustParticle1, m_dustParticle2;
		private bool m_facingRight = true;
		private readonly float m_groundedRememberTime = 0.25f;
		private float m_groundedRemember = 0f;
		private int m_extraJumps;
		private float m_extraJumpForce;
		private float m_dashTime;
		private bool m_hasDashedInAir = false;
		private bool m_onWall = false;
		private bool m_onRightWall = false;
		private bool m_onLeftWall = false;
		private bool m_wallGrabbing = false;
		private readonly float m_wallStickTime = 0.25f;
		private float m_wallStick = 0f;
		private bool m_wallJumping = false;
		private float m_dashCooldown;

		// 0 -> none, 1 -> right, -1 -> left
		private int m_onWallSide = 0;
		private int m_playerSide = 1;


		void Start()
		{
			// create pools for particles
			PoolManager.instance.CreatePool(dashEffect, 2);
			PoolManager.instance.CreatePool(jumpEffect, 2);

			// if it's the player, make this instance currently playable
			if (transform.CompareTag("Player"))
				isCurrentlyPlayable = true;

			m_extraJumps = extraJumpCount;
			m_dashTime = startDashTime;
			m_dashCooldown = dashCooldown;
			m_extraJumpForce = jumpForce * 0.7f;

			m_rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			if (!SettingsManager.Instance.AllowGamePlay() || isDead) return;

			// check if grounded
			isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, whatIsGround);
			Vector2 position = transform.position;
			// check if on wall
			Collider2D rightHit = Physics2D.OverlapCircle(position + grabRightOffset, grabCheckRadius, whatIsGround);
			Collider2D leftHit = Physics2D.OverlapCircle(position + grabLeftOffset, grabCheckRadius, whatIsGround);

			m_onRightWall = rightHit != null && !rightHit.CompareTag("Platform");
			m_onLeftWall = leftHit != null && !leftHit.CompareTag("Platform");
			m_onWall = m_onRightWall || m_onLeftWall;

			// calculate player and wall sides as integers
			CalculateSides();

			if ((m_wallGrabbing || isGrounded) && m_wallJumping)
			{
				m_wallJumping = false;
			}
			// if this instance is currently playable
			if (isCurrentlyPlayable)
			{
				// horizontal movement
				if (m_wallJumping)
				{
					m_rb.linearVelocity = Vector2.Lerp(m_rb.linearVelocity, new Vector2(moveInput * speed, m_rb.linearVelocity.y), 1.5f * Time.fixedDeltaTime);
				}
				else
				{
					if (canMove && !m_wallGrabbing)
						m_rb.linearVelocity = new Vector2(moveInput * speed, m_rb.linearVelocity.y);
					else if (!canMove)
						m_rb.linearVelocity = new Vector2(0f, m_rb.linearVelocity.y);
				}
				// better jump physics
				if (m_rb.linearVelocity.y < 0f)
				{
					m_rb.linearVelocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
				}

				// Flipping
				if (!m_facingRight && moveInput > 0f)
					Flip();
				else if (m_facingRight && moveInput < 0f)
					Flip();

				// Dashing logic
				if (isDashing)
				{
					if (m_dashTime <= 0f)
					{
						isDashing = false;
						m_dashCooldown = dashCooldown;
						m_dashTime = startDashTime;
						m_rb.linearVelocity = Vector2.zero;
					}
					else
					{
						m_dashTime -= Time.deltaTime;
						if (m_facingRight)
							m_rb.linearVelocity = Vector2.right * dashSpeed;
						else
							m_rb.linearVelocity = Vector2.left * dashSpeed;
					}
				}

				// wall grab
				if (m_onWall && !isGrounded && m_rb.linearVelocity.y <= 0f && m_playerSide == m_onWallSide)
				{
					actuallyWallGrabbing = true;    // for animation
					m_wallGrabbing = true;
					m_rb.linearVelocity = new Vector2(moveInput * speed, -slideSpeed);
					m_wallStick = m_wallStickTime;
				}
				else
				{
					m_wallStick -= Time.deltaTime;
					actuallyWallGrabbing = false;
					if (m_wallStick <= 0f)
						m_wallGrabbing = false;
				}
				if (m_wallGrabbing && isGrounded)
					m_wallGrabbing = false;

				// enable/disable dust particles
				float playerVelocityMag = m_rb.linearVelocity.sqrMagnitude;
				if (m_dustParticle1.isPlaying && playerVelocityMag == 0f)
				{
					m_dustParticle1.Stop();
					m_dustParticle2.Stop();
				}
				else if (!m_dustParticle1.isPlaying && playerVelocityMag > 0f)
				{
					m_dustParticle1.Play();
					m_dustParticle2.Play();
				}
			}
		}

		void Update()
		{
			if (!isCurrentlyPlayable && !SettingsManager.Instance.AllowGamePlay() || isDead) return;

			// horizontal input
			moveInput = InputSystem.HorizontalRaw();

			if (isGrounded)
			{
				m_extraJumps = extraJumpCount;
			}

			// grounded remember offset (for more responsive jump)
			m_groundedRemember -= Time.deltaTime;
			if (isGrounded)
				m_groundedRemember = m_groundedRememberTime;

			// if not currently dashing and hasn't already dashed in air once
			if (!isDashing && !m_hasDashedInAir && m_dashCooldown <= 0f)
			{
				// dash input (left shift)
				if (InputSystem.Dash())
				{
					isDashing = true;
					// dash effect
					PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);
					AudioManager.Instance.StopLoopingSFX();
					AudioManager.Instance.PlaySFX("Attack" + Random.Range(1, 3));
					// AudioManager.Instance.PlaySFX("Bark");
					// if player in air while dashing
					if (!isGrounded)
					{
						m_hasDashedInAir = true;
					}
					// dash logic is in FixedUpdate
				}
			}
			m_dashCooldown -= Time.deltaTime;

			// if has dashed in air once but now grounded
			if (m_hasDashedInAir && isGrounded)
				m_hasDashedInAir = false;

			// Jumping
			if (InputSystem.Jump())
			{
				if (m_extraJumps > 0 && !isGrounded && !m_wallGrabbing) // extra jumping
				{
					m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, m_extraJumpForce);
					m_extraJumps--;
				}
				else if (isGrounded || m_groundedRemember > 0f) // normal single jumping
				{
					m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, jumpForce);
				}
				else if (m_wallGrabbing)
				{
					m_wallGrabbing = false;
					m_wallJumping = true;
					if (moveInput != m_onWallSide) // wall jumping off the wall
					{
						m_rb.AddForce(new Vector2(-m_onWallSide * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
						Debug.Log("Wall jumped");
					}
					else if (moveInput != 0 && moveInput == m_onWallSide) // wall climbing jump
					{
						m_rb.AddForce(new Vector2(-m_onWallSide * wallClimbForce.x, wallClimbForce.y), ForceMode2D.Impulse);
						Debug.Log("Wall climbed");
					}
					if (m_playerSide == m_onWallSide)
						Flip();
				}

				// jumpEffect
				PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
				AudioManager.Instance.StopLoopingSFX();
				AudioManager.Instance.PlaySFX("JumpUp");
			}
		}

		void Flip()
		{
			m_facingRight = !m_facingRight;
			Vector3 scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;
		}

		void CalculateSides()
		{
			if (m_onRightWall)
				m_onWallSide = 1;
			else if (m_onLeftWall)
				m_onWallSide = -1;
			else
				m_onWallSide = 0;

			if (m_facingRight)
				m_playerSide = 1;
			else
				m_playerSide = -1;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
			Gizmos.DrawWireSphere((Vector2)transform.position + grabRightOffset, grabCheckRadius);
			Gizmos.DrawWireSphere((Vector2)transform.position + grabLeftOffset, grabCheckRadius);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Obstacle"))
			{
				Die();
			}

			if (other.CompareTag("Enemy"))
			{
				if (isDashing)
				{
					other.GetComponent<EnemyController>().TriggerDeathAnimation();
				}
				else
				{
					Die();
				}
			}
		}

		private void Die()
		{
			print("Died");
			m_rb.linearVelocity = Vector2.zero;
			m_dustParticle1.Stop();
			m_dustParticle2.Stop();
			isDashing = false;
			isAttacking = false;
			isCurrentlyPlayable = false;
			isDead = true;
		}
	}
}
