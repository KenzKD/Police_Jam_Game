using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator m_anim;
    private bool isTrulyDead = false;
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!SettingsManager.Instance.AllowGamePlay() || isTrulyDead) return;
    }

    public void TriggerDeathAnimation()
    {
        m_anim.SetTrigger(IsDead);
        DOVirtual.DelayedCall(2f, () => Destroy(gameObject));
    }
}
