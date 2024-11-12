using UnityEngine;

public class Trampoline : Platform
{
	public override Controller2D.CollisionInfo HandleCollision(GameObject other, Controller2D.CollisionInfo collisionInfo)
	{
		Controller2D.CollisionInfo info = new();
		if (other.TryGetComponent<Player>(out Player player))
		{
			float remainingTime = 1.0f - collisionInfo.time;
			if (remainingTime > 0)
			{
				player.controller.grounded = false;
				player.velocity.y = player.jumpVelocity;
				info = player.controller.Move(player.velocity * remainingTime);
			}
		}
		return info;
	}
}
