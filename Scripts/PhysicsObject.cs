using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

	public Bounds bounds;

	public virtual void Start()
	{
		bounds.center = transform.position;
		bounds.extents = transform.localScale / 2.0f;
	}

	public bool CollidesWith(PhysicsObject other)
	{
		return bounds.max.x > other.bounds.min.x &&
			   other.bounds.max.x > bounds.min.x &&
			   bounds.max.y > other.bounds.min.y &&
			   other.bounds.max.y > bounds.min.y;
	}
}
