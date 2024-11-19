using UnityEngine;

public abstract class DynamicObject : MonoBehaviour
{
	public Vector2 velocity;

	public bool up;
	public bool down;
	public bool left;
	public bool right;

	public Bounds bounds;

	public virtual float mass { get; }

	public virtual void Start()
    {
		bounds.center = transform.position;
		bounds.extents = transform.localScale / 2.0f;
	}

	public bool CollidesWith(DynamicObject other)
    {
		return bounds.max.x > other.bounds.min.x &&
			   other.bounds.max.x > bounds.min.x &&
			   bounds.max.y > other.bounds.min.y &&
			   other.bounds.max.y > bounds.min.y;
	}
}
