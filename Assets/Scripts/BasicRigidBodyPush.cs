using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
	public LayerMask pushLayers;
	public bool canPush;
	[Range(0.5f, 5f)] public float strength = 1.1f;

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (canPush) PushRigidBodies(hit);
	}

	private void PushRigidBodies(ControllerColliderHit hit)
	{
		// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

		// убедитесь, что мы попали в некинематическое твердое тело
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) return;

		// убедитесь, что мы нажимаем только нужный слой (слои)
		var bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0) return;

		// Мы не хотим толкать предметы под собой
		if (hit.moveDirection.y < -0.3f) return;

		// Вычислить направление толчка из направления движения, только горизонтальное движение
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

		// Примените толчок и примите во внимание силу
		body.AddForce(pushDir * strength, ForceMode.Impulse);
	}
}