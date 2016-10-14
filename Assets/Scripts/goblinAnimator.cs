using UnityEngine;
using System.Collections;

public class goblinAnimator : MonoBehaviour {
	Animator anim;
	NavMeshAgent agent;

	bool traversingLink = false;
	OffMeshLinkData currLink;
    Vector3 target;
    float timeLeft = 10;

    Vector2 smoothDeltaPosition = Vector2.zero;
	Vector2 velocity = Vector2.zero;
    Vector2 runVelocity = Vector2.zero;

    bool isRunning = false;

	void Start ()
	{
		anim = GetComponent<Animator> ();
        anim.applyRootMotion = true;

		agent = GetComponent<NavMeshAgent> ();
		// Don’t update position automatically
		agent.updatePosition = false;
        //agent.enabled = false;
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

        runVelocity = velocity * 5;

        // Update animation parameters
        anim.SetBool("Moving", shouldMove);
        anim.SetFloat("xForce", velocity.x);
        anim.SetFloat("zForce", velocity.y);

        if (Input.GetKey(KeyCode.LeftShift))
		{
            isRunning = true;
            anim.SetBool("run", true);
		}
		else
		{
            isRunning = false;
            anim.SetBool("run", false);
		}

        if (Input.GetKey(KeyCode.Space)) {
            agent.enabled = false;
            anim.SetTrigger("Jump");
        }

        // get the off-mesh link, will trigger the jump animation
        // every time it will go through 2 mesh, so use a number to count
        if (traversingLink && timeLeft < 0)
        {
            //anim.enabled = true;
            traversingLink = false;
            transform.position = currLink.endPos;
            Debug.Log("jump end");
            agent.CompleteOffMeshLink();
            agent.Resume();
            agent.SetDestination(target);
        }

        if (traversingLink)
        {
            timeLeft -= Time.deltaTime;
        }

        if (agent.isOnOffMeshLink) {
            target = agent.destination;

			if (!traversingLink) {
				Debug.Log ("Should start jump now");
				//agent.Stop ();
				anim.Play ("Idle Jump");

                currLink = agent.currentOffMeshLinkData;

                traversingLink = true;
                timeLeft = 2.14f;
			}
		}
	}

	void OnAnimatorMove ()
	{
        if (timeLeft < 0.93f && timeLeft > 0f)
        {
            if (traversingLink)
            {
                float percent = 1 - (timeLeft - 0.14f)/0.79f;

                Debug.Log(percent);

                float z = (currLink.endPos.z - currLink.startPos.z) * percent;
                float y = 1f * Mathf.Sin(Mathf.PI * percent);
                Vector3 newPos = new Vector3(currLink.endPos.x, currLink.endPos.y + y, currLink.startPos.z + z);
                transform.position = newPos;
            }
        }
        else {
            if (!traversingLink)
            {
                if (isRunning)
                {
                    agent.speed = 8;
                }
                else {
                    agent.speed = 3.5f;
                }
                transform.position = agent.nextPosition; 
                         
            }

        }
    }
}
