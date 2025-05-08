using DG.Tweening;
using UnityEngine;

namespace SupanthaPaul
{
	public class PlayerAnimator : MonoBehaviour
	{
		public ParticleSystem dustParticle1, dustParticle2;
		private Rigidbody2D m_rb;
		private PlayerController m_controller;
		private Animator m_anim;
		private bool isTrulyDead = false, isStillSliding = false;
		private static readonly int Move = Animator.StringToHash("Move");
		private static readonly int JumpState = Animator.StringToHash("JumpState");
		private static readonly int IsJumping = Animator.StringToHash("IsJumping");
		private static readonly int WallGrabbing = Animator.StringToHash("WallGrabbing");
		private static readonly int IsDashing = Animator.StringToHash("IsDashing");
		private static readonly int IsDead = Animator.StringToHash("IsDead");
		private static readonly int IsSliding = Animator.StringToHash("IsSliding");
		private static readonly int IsGameStarted = Animator.StringToHash("IsGameStarted");
		private static readonly int StartTrigger = Animator.StringToHash("StartTrigger");

		private void Start()
		{
			m_anim = GetComponentInChildren<Animator>();
			m_controller = GetComponent<PlayerController>();
			m_rb = GetComponent<Rigidbody2D>();

			dustParticle1.Stop();
			dustParticle2.Stop();
		}

		private void FixedUpdate()
		{
			if (!SettingsManager.Instance.AllowGamePlay() || isTrulyDead) return;

			// Idle & Running animation
			m_anim.SetFloat(Move, Mathf.Abs(m_rb.linearVelocity.x));

			// Jump state (handles transitions to falling/jumping)
			m_anim.SetFloat(JumpState, m_rb.linearVelocity.y);

			// Jump animation
			if (!m_controller.isGrounded && !m_controller.actuallyWallGrabbing)
			{
				m_anim.SetBool(IsJumping, true);
			}
			else
			{
				m_anim.SetBool(IsJumping, false);
			}

			if (!m_controller.isGrounded && m_controller.actuallyWallGrabbing)
			{
				m_anim.SetBool(WallGrabbing, true);
				TriggerWallGrabSound();
			}
			else
			{
				m_anim.SetBool(WallGrabbing, false);
				isStillSliding = false;
			}

			// dash animation
			m_anim.SetBool(IsDashing, m_controller.isDashing);

			if (m_controller.isDead)
			{
				isTrulyDead = true;
				TriggerDeathAnimation();
			}
		}

		private void TriggerDeathAnimation()
		{
			m_anim.SetTrigger(IsDead);
			AudioManager.Instance.StopLoopingSFX();
			AudioManager.Instance.PlaySFX("Die");
			DOVirtual.DelayedCall(2f, () => SettingsManager.Instance.OnRestart());
		}

		private void TriggerWallGrabSound()
		{
			if (isStillSliding) return;

			isStillSliding = true;
			m_anim.SetTrigger(IsSliding);
		}

		public void TriggerTutorialStartAnimation()
		{
			m_anim.SetBool(IsGameStarted, true);
			m_anim.SetTrigger(StartTrigger);
		}

		public void PlayDustParticles()
		{
			dustParticle1.Play();
			dustParticle2.Play();
		}
	}
}
