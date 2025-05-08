using DG.Tweening;
using UnityEngine;
public class ItemCollector : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(Vector3.one * 4f, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        transform.DOShakeRotation(0.5f, Vector3.forward * 10f).SetLoops(-1, LoopType.Yoyo);
    }
}
