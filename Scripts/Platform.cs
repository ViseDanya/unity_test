using UnityEngine;

public class Platform : MonoBehaviour
{
    public virtual void HandleCollision(GameObject other, Controller2D.CollisionInfo collisionInfo)
    {
		if (other.TryGetComponent<Player>(out Player player))
		{
			float remainingTime = 1.0f - collisionInfo.time;
			if (remainingTime > 0)
			{
				Vector2 velocity = collisionInfo.velocity;
				float dotprod = (velocity.x * collisionInfo.normal.y + velocity.y * collisionInfo.normal.x) * remainingTime;
				velocity.x = dotprod * collisionInfo.normal.y;
				velocity.y = dotprod * collisionInfo.normal.x;
				if (velocity != Vector2.zero)
				{
					player.controller.Move(velocity * remainingTime);
				}
			}
		}
	}
}
