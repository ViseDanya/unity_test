using UnityEngine;

public class ConveyorBelt : Platform
{

	//public float conveyorSpeed = 3;

	//public override Controller2D.CollisionInfo HandleCollision(GameObject other, Controller2D.CollisionInfo collisionInfo)
	//{
	//	Controller2D.CollisionInfo info = new();
	//	if (other.TryGetComponent<Player>(out Player player))
	//	{
	//		float remainingTime = 1.0f - collisionInfo.time;
	//		if (remainingTime > 0)
	//		{
	//			Vector2 velocity = collisionInfo.velocity;
	//			float dotprod = (velocity.x * collisionInfo.normal.y + velocity.y * collisionInfo.normal.x) * remainingTime;
	//			velocity.x = dotprod * collisionInfo.normal.y;
	//			velocity.y = dotprod * collisionInfo.normal.x;
	//			player.controller.Move(velocity * remainingTime + conveyorSpeed * Time.fixedDeltaTime * Vector2.right);
	//		}
	//	}
	//	return info;
	//}
}
