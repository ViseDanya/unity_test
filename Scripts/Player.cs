using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

	InputAction moveAction;
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	public float moveSpeed = 6;
	public int id = 0;

	float gravity;
	public float jumpVelocity;
	public Vector3 velocity;

	public Controller2D controller;
	public bool moved;

	Vector2 input;

	void Start()
	{
		controller = GetComponent<Controller2D>();

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
		if (controller.grounded)
		{
			velocity.y = input.y > 0 ? jumpVelocity : 0;
		}

		velocity.x = input.x * moveSpeed;
		velocity.y += gravity * Time.fixedDeltaTime;
		return velocity;
	}

	void FixedUpdate()
	{
		if(!moved)
        {
			controller.Move(GetVelocity() * Time.fixedDeltaTime);
			moved = true;
		}
		moved = false;
	}

}