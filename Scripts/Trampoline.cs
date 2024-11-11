using UnityEngine;

public class Trampoline : Platform
{
	public override void HandleCollision(GameObject other, Controller2D.CollisionInfo collisionInfo)
	{
		if (other.TryGetComponent<Player>(out Player player))
		{
			player.controller.grounded = false;
			player.velocity.y = player.jumpVelocity; 
		}
	}
}
