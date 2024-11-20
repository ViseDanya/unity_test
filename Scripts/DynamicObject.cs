using UnityEngine;
using System.Collections.Generic;

public abstract class DynamicObject : MonoBehaviour
{
	public Vector2 velocity;

	// TODO: make the value a list of DynamicObjects to take into account collisions
	// with multiple objects in the same direction
	public Dictionary<Direction, DynamicObject> Adjacencies = new();

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

	protected void ResetAdjacency()
    {
		Adjacencies.Clear();
	}

	public List<DynamicObject> GetAdjacencyList(Direction direction)
    {
		List<DynamicObject> adjacencyList = new();
		DynamicObject currentObj = this;
		while(currentObj.Adjacencies.ContainsKey(direction))
        {
			currentObj = currentObj.Adjacencies[direction];
			adjacencyList.Add(currentObj);
        }
		return adjacencyList;
    }

	public enum Direction { UP, DOWN, LEFT, RIGHT};
}
