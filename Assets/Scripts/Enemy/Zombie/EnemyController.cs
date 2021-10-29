using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/* Makes enemies follow and attack the player */

public class EnemyController : MonoBehaviour
{
	public EnemyHealth1 health1;

	public float lookRadius = 10f;
	public float wanderSpeed = 4f;
	public float chaseSpeed = 7f;

	Transform target;
	NavMeshAgent agent;


	
	private Animator animator;
	private float stopDistance = 1.65f;
	
	void Start()
	{
		

		animator = GetComponent<Animator>();
		target = PlayerManager.instance.player.transform;
		agent = GetComponent<NavMeshAgent>();

	}

	void Update()
	{
		agent.speed = chaseSpeed;
		animator.SetBool("Aware", false);
		// Get the distance to the player
		float distance = Vector3.Distance(target.position, transform.position);

		// If inside the radius
		if (distance <= lookRadius)
		{
			agent.SetDestination(target.position);
			animator.SetBool("Aware", true);
			agent.speed = wanderSpeed;
		}
		if (agent.remainingDistance < stopDistance)
		{
			GetComponent<Animator>().SetTrigger("isAttack");
			FaceTarget();
			agent.speed = 0;
		}
		
	}

	void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	// Point towards the player
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}

	void OnCollisionEnter(Collision collision)
	{

		if (collision.gameObject.tag == "KillZombie")
		{
			if (health1)
            {
				health1.onTakeDamage(10);
				Debug.Log(health1);
			}
		}
	}

}