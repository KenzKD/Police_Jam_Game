using DG.Tweening;
using UnityEngine;
public class ItemCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ScoreManager.Instance.AddPoint(1);
            GetComponent<Collider2D>().enabled = false;
            transform.DOScale(0f, 0.5f).SetEase(Ease.InExpo).OnComplete(() => Destroy(transform.parent.gameObject));
        }
    }
}
