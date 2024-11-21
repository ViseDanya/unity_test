using UnityEngine;
using System.Collections.Generic;

public abstract class DynamicObject : PhysicsObject
{
	public Vector2 velocity;

	public bool isOnCeiling;
	public bool isOnFloor;
	public bool isOnWallLeft;
	public bool isOnWallRight;

	// TODO: make the value a set of DynamicObjects to take into account collisions
	// with multiple objects in the same direction
	public Dictionary<Direction, DynamicObject> Adjacencies = new();

	public virtual float mass { get; }

	protected void Reset()
    {
		isOnCeiling = false;
		isOnFloor = false;
		isOnWallLeft = false;
		isOnWallRight = false;
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
