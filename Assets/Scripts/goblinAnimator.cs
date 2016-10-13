using UnityEngine;
using System.Collections;

public class goblinAnimator : MonoBehaviour {
	Animator anim;
	NavMeshAgent agent;
	bool traversingLink = false;
	OffMeshLinkData currLink;
	Vector2 smoothDeltaPosition = Vector2.zero;
	Vector2 velocity = Vector2.zero;

	void Start ()
	{
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		// Don’t update position automatically
		agent.updatePosition = false;
	}

	void Update ()
	{
		Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

		// Map 'worldDeltaPosition' to local space
		float dx = Vector3.Dot (transform.right, worldDeltaPosition);
		float dz = Vector3.Dot (transform.forward, worldDeltaPosition);
		Vector2 deltaPosition = new Vector2 (dx, dz);

		// Low-pass filter the deltaMove
		float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
		smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

		// Update velocity if time advances
		if (Time.deltaTime > 1e-5f)
			velocity = smoothDeltaPosition / Time.deltaTime;

		bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

		// Update animation parameters
		anim.SetBool("Moving", shouldMove);
		anim.SetFloat ("xForce", velocity.x);
		anim.SetFloat ("zForce", velocity.y);

		if (Input.GetKey(KeyCode.LeftShift))
		{
			anim.SetBool("run", true);
		}
		else
		{
			anim.SetBool("run", false);
		}

		// get the off-mesh link, will trigger the jump animation
		// every time it will go through 2 mesh, so use a number to count
		if (agent.isOnOffMeshLink) {
			if (!traversingLink) {
				Debug.Log ("Should start jump now");
				//agent.Stop ();
				anim.SetBool ("Moving", false);
				anim.Play ("Idle");
				anim.Play ("Idle Jump");
				currLink = agent.currentOffMeshLinkData;
				traversingLink = true;
			}

			var tlerp = anim.GetCurrentAnimatorStateInfo (0).normalizedTime;
			var newPosition = Vector3.Lerp (currLink.startPos, currLink.endPos, tlerp);
			newPosition.y += 2f * Mathf.Sin (Mathf.PI * tlerp);
			transform.position = newPosition;

			if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle Jump")) {
				transform.position = currLink.endPos;
				traversingLink = false;
				agent.CompleteOffMeshLink();
				anim.SetBool ("Moving", shouldMove);
				agent.Resume();
			}
			/*anim.SetBool("Moving", false);
			agent.Stop();
			if (jumpNbr % 2 == 0)
			{
				Debug.Log("should jump here!");
				anim.SetTrigger("Jump");
			}
			jumpNbr += 1;
			anim.SetBool("Moving", true);
			agent.Resume ();*/
		}
	}

	void OnAnimatorMove ()
	{
		// Update position to agent position
		transform.position = agent.nextPosition;
	}
}
