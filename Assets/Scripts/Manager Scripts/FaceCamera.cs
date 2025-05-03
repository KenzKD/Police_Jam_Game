using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Update is called once per frame, after all Update functions have been called
    private void LateUpdate()
    {
        // Rotate the game object to face the camera
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
