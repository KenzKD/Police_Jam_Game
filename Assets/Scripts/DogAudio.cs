using UnityEngine;

public class DogAudio : MonoBehaviour
{
    public void PlayFootStep()
    {
        AudioManager.Instance.StopLoopingSFX();
        AudioManager.Instance.PlaySFX("DogFootStep" + Random.Range(1, 3));
    }

    // public void PlayAttack()
    // {
    //     AudioManager.Instance.StopLoopingSFX();
    //     AudioManager.Instance.PlaySFX("Attack" + Random.Range(1, 3));
    // }

    // public void PlayDie()
    // {
    //     AudioManager.Instance.StopLoopingSFX();
    //     AudioManager.Instance.PlaySFX("Die");
    // }

    // public void PlayJumpUp()
    // {
    //     AudioManager.Instance.StopLoopingSFX();
    //     AudioManager.Instance.PlaySFX("JumpUp");
    // }

    public void PlayWallGrab()
    {
        AudioManager.Instance.StopLoopingSFX();
        AudioManager.Instance.PlayLoopingSFX("WallGrab");
    }

    public void PlayIdle()
    {
        AudioManager.Instance.StopLoopingSFX();
        AudioManager.Instance.PlaySFX("Idle");
    }
}
