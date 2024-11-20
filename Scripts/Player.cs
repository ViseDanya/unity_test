using UnityEngine;
using UnityEngine.InputSystem;

public class Player : DynamicObject
{

	InputAction moveAction;
	public float gravity;
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	public float moveSpeed = 6;
	public int id = 0;

	public float jumpVelocity;
	Vector2 input;

	public override float mass => 1;

	public override void Start()
	{
		base.Start();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		if(id == 0)
        {
			moveAction = InputSystem.actions.FindAction("KeyboardLeft/Move");
		}
		else if(id == 1)
        {
			moveAction = InputSystem.actions.FindAction("KeyboardRight/Move");
		}
		else
        {
			moveAction = InputSystem.actions.FindAction("KeyboardMiddle/Move");
		}
	}

	void Update()
	{
		input = moveAction.ReadValue<Vector2>();
	}

	public Vector2 GetVelocity()
    {
		//if (Adjacencies.ContainsKey(Direction.DOWN))
		//{
		//	velocity.y = input.y > 0 ? jumpVelocity : 0;
		//}

		velocity.x = input.x * moveSpeed;
		velocity.y = input.y * moveSpeed;
		//velocity.y += gravity * Time.fixedDeltaTime;
		return velocity;
	}

	void FixedUpdate()
	{
		ResetAdjacency();
        velocity = GetVelocity() * Time.fixedDeltaTime;
		Debug.Log("Player " + id + ": " + velocity);
    }

}