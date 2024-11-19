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

	public bool moved;

	Vector2 input;

	public override float mass => 1;

	public override void Start()
	{
		base.Start();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);

		if(id == 0)
        {
			moveAction = InputSystem.actions.FindAction("KeyboardLeft/Move");
		}
		else
        {
			moveAction = InputSystem.actions.FindAction("KeyboardRight/Move");
		}
	}

	void Update()
	{
		input = moveAction.ReadValue<Vector2>();
	}

	public Vector2 GetVelocity()
    {
		if (!moved)
		{
			if (down)
			{
				velocity.y = input.y > 0 ? jumpVelocity : 0;
			}

			velocity.x = input.x * moveSpeed;
			//velocity.y += gravity * Time.fixedDeltaTime;
			velocity.y = input.y * moveSpeed;
			return velocity;
		}
        else
        {
			return Vector2.zero;
        }
	}

	void FixedUpdate()
	{
		up = false;
		down = false;
		left = false;
		right = false;

        velocity = GetVelocity() * Time.fixedDeltaTime;
    }

}