using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{

	public LayerMask collisionMask;
	public bool grounded;
    readonly float collisionTolerance = 1e-3f;

	BoxCollider2D m_collider;

	void Start()
	{
		m_collider = GetComponent<BoxCollider2D>();
	}

	public CollisionInfo Move(Vector2 velocity)
    {
		Debug.Log("Move: " + velocity);
		if (velocity.y != 0)
		{
			grounded = false;
		}
		CollisionInfo firstCollisionInfo = MoveAndCollide(velocity);
		if (firstCollisionInfo.hit)
		{
			if(firstCollisionInfo.collider.gameObject.TryGetComponent(out Platform platform))
            {
				Debug.Log("Player collided with platform");
				platform.HandleCollision(gameObject, firstCollisionInfo);
			}
            else if (firstCollisionInfo.collider.gameObject.TryGetComponent(out Player player))
            {
				Debug.Log("Player collided with player");
                float remainingTime = 1.0f - firstCollisionInfo.time;
                Vector2 remainingVelocity = remainingTime * velocity;
                Vector2 otherVelocity = player.GetVelocity() * Time.fixedDeltaTime;

				Debug.Log("otherVelocity: " + otherVelocity);

                Vector2 averageVelocity = new Vector2(otherVelocity.x + (Mathf.Abs(firstCollisionInfo.normal.x) * remainingVelocity.x),
													  otherVelocity.y + (Mathf.Abs(firstCollisionInfo.normal.y) * remainingVelocity.y)) / 2;

				Debug.Log("averageVelocity: " + averageVelocity);

				CollisionInfo playerCollision = player.controller.Move(averageVelocity);
                player.moved = true;

                CollisionInfo outCollision = MoveAndCollide(remainingVelocity * playerCollision.time);
				return outCollision;
            }
            else
			{
				float remainingTime = 1.0f - firstCollisionInfo.time;
				if (remainingTime > 0)
				{
					Debug.Log("Remaining time: " + remainingTime);
					float dotprod = (velocity.x * firstCollisionInfo.normal.y + velocity.y * firstCollisionInfo.normal.x) * remainingTime;
					velocity.x = dotprod * firstCollisionInfo.normal.y;
					velocity.y = dotprod * firstCollisionInfo.normal.x;
					if (velocity != Vector2.zero)
					{
						CollisionInfo outCollision = Move(velocity * remainingTime);
						return outCollision;
					}
				}
			}
		}
		return firstCollisionInfo;
		
	}

	public CollisionInfo MoveAndCollide(Vector2 velocity)
	{
		float firstCollisionTime = 1.0f;
		Vector2 firstCollisionNormal = new();
		BoxCollider2D firstCollisionCollider = new();
		foreach(BoxCollider2D collider in GetBroadphaseCollisions(velocity))
        {
            float collisionTime = SweptAABB(m_collider.bounds, collider.bounds, velocity, out Vector2 normal);
            if (collisionTime < firstCollisionTime)
            {
				firstCollisionTime = collisionTime;
				firstCollisionNormal = normal;
				firstCollisionCollider = collider;
			}
		}

		if (firstCollisionNormal == Vector2.up)
		{
			grounded = true;
		}

		transform.Translate(velocity * firstCollisionTime);

		CollisionInfo info;
		info.hit = firstCollisionTime < 1.0f;
		info.velocity = velocity;
		info.time = firstCollisionTime;
		info.normal = firstCollisionNormal;
		info.collider = firstCollisionCollider;
		return info;
	}

	List<Collider2D> GetBroadphaseCollisions(Vector2 velocity)
    {
		BoxCollider2D[] allColliders = FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None);
		Vector2 broadphaseBoxCenter = new(m_collider.bounds.center.x + velocity.x / 2, m_collider.bounds.center.y + velocity.y / 2);
		Vector2 broadphaseBoxSize = new(m_collider.size.x + Mathf.Abs(velocity.x), m_collider.size.y + Mathf.Abs(velocity.y));
		Bounds broadphaseBounds = new(broadphaseBoxCenter, broadphaseBoxSize);

		Debug.DrawLine(broadphaseBounds.min, broadphaseBounds.max, Color.blue);

		List<Collider2D> broadphaseCollisions = new();
		foreach (BoxCollider2D collider in allColliders)
		{
			if (collider != m_collider && AABBCheck(collider.bounds, broadphaseBounds))
			{
				broadphaseCollisions.Add(collider);
			}
		}
		return broadphaseCollisions;
	}

	bool AABBCheck(Bounds b1, Bounds b2)
	{
		return b1.max.x > b2.min.x &&
			   b2.max.x > b1.min.x &&
			   b1.max.y > b2.min.y &&
			   b2.max.y > b1.min.y;
	}

	float SweptAABB(Bounds b1, Bounds b2, Vector2 velocity, out Vector2 normal)
    {
		float xInvEntry, yInvEntry;
		float xInvExit, yInvExit;

		// find the distance between the objects on the near and far sides for both x and y 
		if (velocity.x > 0)
		{
			xInvEntry = b2.min.x - b1.max.x;
			xInvExit = b2.max.x - b1.min.x;
		}
		else
		{
			xInvEntry = b2.max.x - b1.min.x;
			xInvExit = b2.min.x - b1.max.x;
		}

		if (velocity.y > 0)
		{
			yInvEntry = b2.min.y - b1.max.y;
			yInvExit = b2.max.y - b1.min.y;
		}
		else
		{
			yInvEntry = b2.max.y - b1.min.y;
			yInvExit = b2.min.y - b1.max.y;
		}

		float xEntry, yEntry;
		float xExit, yExit;

		if (velocity.x == 0)
		{
			xEntry = Mathf.NegativeInfinity;
			xExit = Mathf.Infinity;
		}
		else
		{
			xEntry = xInvEntry / velocity.x;
			xExit = xInvExit / velocity.x;
		}

		if (velocity.y == 0)
		{
			yEntry = Mathf.NegativeInfinity;
			yExit = Mathf.Infinity;
		}
		else
		{
			yEntry = yInvEntry / velocity.y;
			yExit = yInvExit / velocity.y;
		}

		float entryTime = Mathf.Max(xEntry, yEntry);
		float exitTime = Mathf.Min(xExit, yExit);

		Debug.Log("Entry time: " + entryTime + " Exit time: " + exitTime);
		// if there was no collision
		if (entryTime > exitTime || xEntry < 0.0f && yEntry < 0.0f || xEntry > 1.0f || yEntry > 1.0f)
		{
			normal = Vector2.zero;
			return 1.0f;
		}
		else // if there was a collision 
		{
			// calculate normal of collided surface
			if (xEntry > yEntry)
			{
				if (xInvEntry < 0.0f)
				{
					normal = Vector2.right;
				}
				else if(xInvEntry > 0.0f)
				{
					normal = Vector2.left;
				}
				else
                {
					normal = -Mathf.Sign(velocity.x) * Vector2.right;
                }
			}
			else
			{
				if (yInvEntry < 0.0f)
				{
					normal = Vector2.up;
				}
				else if(yInvEntry > 0.0f)
				{
					normal = Vector2.down;
				}
				else
                {
					normal = -Mathf.Sign(velocity.y) * Vector2.up;
				}
			}
			return entryTime < collisionTolerance ? 0.0f : entryTime;
		}
	}

	public struct CollisionInfo
    {
		public bool hit;
		public float time;
		public Vector2 normal;
		public BoxCollider2D collider;
		public Vector2 velocity;
    }
}