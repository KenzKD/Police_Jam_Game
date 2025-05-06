using DG.Tweening;
using UnityEngine;

public class WinDetector : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    private bool isDetected = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDetected) return;
        
        // print("Player Detected");
        isDetected = true;
        AudioManager.Instance.PlaySFX("Win");
        DOVirtual.DelayedCall(3f, () => SettingsManager.Instance.ChooseLevel(levelIndex));
    }
}
