using UnityEngine;

public class Platform : MonoBehaviour
{
    public virtual Controller2D.CollisionInfo HandleCollision(GameObject other, Controller2D.CollisionInfo collisionInfo)
    {
		Controller2D.CollisionInfo info = new();
		if (other.TryGetComponent<Player>(out Player player))
		{
			float remainingTime = 1.0f - collisionInfo.time;
			if (remainingTime > Controller2D.collisionTolerance)
			{
				Vector2 velocity = collisionInfo.velocity;
				float dotprod = (velocity.x * collisionInfo.normal.y + velocity.y * collisionInfo.normal.x) * remainingTime;
				velocity.x = dotprod * collisionInfo.normal.y;
				velocity.y = dotprod * collisionInfo.normal.x;
				if (velocity != Vector2.zero)
				{
					info = player.controller.Move(velocity * remainingTime);
				}
			}
		}
		return info;
	}
}
