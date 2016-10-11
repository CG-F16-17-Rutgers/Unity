using UnityEngine;
using System.Collections;

enum Direction {
	idle,
	forward,
	backward
};

public class CharacterController : MonoBehaviour {
	Animator anim;

	Direction direction;
	Direction lastDirection;

	float horizontalMove;
	float verticalMove;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		lastDirection = Direction.idle;
		direction = Direction.idle;
	}

	// Update is called once per frame
	void Update () {

		float zForce = Input.GetAxis("Vertical");
		float xForce = Input.GetAxis("Horizontal");

		//*** used to change duirection so we can switch between run backward and forward
		if (zForce > 0) {
			direction = Direction.forward;
		} else if (zForce == 0) {
			direction = Direction.idle;
		} else {
			direction = Direction.backward;
		}

		if (direction != lastDirection)
		{
			anim.SetBool("directionChanged", true);
			Debug.Log("direction changed");
		}
		else {
			anim.SetBool("directionChanged", false);
		}

		lastDirection = direction;

		anim.SetInteger("direction", (int)direction);
		//*** used to change duirection so we can switch between run backward and forward

		anim.SetFloat("zForce", zForce);
		anim.SetFloat("xForce", xForce);


		// for running
		if (Input.GetKey(KeyCode.LeftShift) && zForce != 0)
		{
			anim.SetBool("run", true);
		}
		else
		{
			anim.SetBool("run", false);
		}

		// for strafe 
		if (Input.GetKey(KeyCode.LeftControl) && xForce!=0)
		{
			anim.SetBool("strafe", true);
		}
		else
		{
			if (zForce == 0 && xForce != 0)
			{
				transform.Rotate(new Vector3(0, xForce * 60 * Time.deltaTime));
			}
			anim.SetBool("strafe", false);
		}

		// for jump
		if (Input.GetKeyDown(KeyCode.Space))
		{
			anim.SetTrigger("Jump");
		}

	}
}