using UnityEngine;

namespace SupanthaPaul
{
	public class CameraFollow : MonoBehaviour
	{
		[SerializeField]
		private Transform target;
		[SerializeField]
		private float smoothSpeed = 0.125f;
		public Vector3 offset;
		[Header("Camera bounds")]
		public Vector3 minCamerabounds;
		public Vector3 maxCamerabounds;

		private void FixedUpdate()
		{
			transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed);
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x, minCamerabounds.x, maxCamerabounds.x),
				Mathf.Clamp(transform.position.y, minCamerabounds.y, maxCamerabounds.y),
				Mathf.Clamp(transform.position.z, minCamerabounds.z, maxCamerabounds.z)
			);
		}

		public void SetTarget(Transform targetToSet)
		{
			target = targetToSet;
		}
	}
}
