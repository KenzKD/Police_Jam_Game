using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.SetParent(transform);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.transform.SetParent(null);
    }
}