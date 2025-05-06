using UnityEngine;

public class DoorDetector : MonoBehaviour
{
    public GameObject Door;
    private static readonly int isDetected = Animator.StringToHash("isDetected");

    void OnTriggerEnter2D(Collider2D collision)
    {
        // print("Player Detected");
        Door.GetComponent<Animator>().SetBool(isDetected, true);
    }
}
