using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewChase : MonoBehaviour
{
    public Transform player;
    private Animator animator;
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private FieldOfView field;
    public float hearRadius = 10f;
    private bool heal = false;
    Transform bestPosition = null;

    Vector3? lastPlayerPos = null;

    public int maxHealth = 100;
    int currentHealth;

    [HideInInspector]public uint _stage = 0;
    [HideInInspector]public bool isSee = false;
    public enum states
    {
        STATE_IDLE = 1,
        STATE_WALK = 2,
        STATE_RUN = 3,
        STATE_ATTACK = 4,
        STATE_GO_LAST_POS = 5,
        STATE_ZERO_TARGETS = 6
    }
    private void Start()
    {
        currentHealth = maxHealth;

        field = GetComponent<FieldOfView>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        
        GotoNextPoint();
    }

    private void Update()
    {
        if (heal)
            Heel();

        isSee = false;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }

        if (field.visibleTargets.Count != 0)
        {
            _stage = 6;
        }
        else if (lastPlayerPos != null)
        {
            _stage = 5;
        }
        else if (points.Length != 0 && isSee == false && lastPlayerPos == null)
        {
            agent.speed = 1;
            _stage = 2;
        }
        if (points.Length == 0 && isSee == false && lastPlayerPos == null)
        {
            _stage = 1;
        }

        if (Vector3.Distance(player.position, transform.position) < hearRadius && Vector3.Distance(player.position, transform.position) >= 1 && field.visibleTargets.Count == 0 && !heal)
        {
            FaceTarget();
        }
        

        updateSmartAi();
    }

    private void updateSmartAi()
    {
        switch (_stage)
        {
            // Idle state 1
            case (uint)states.STATE_IDLE:
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRuning", false);
                animator.SetBool("isAttacking", false);
                return;
            // Walk state 2
            case (uint)states.STATE_WALK:
                //agent.speed = 3;
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdle", false);
                animator.SetBool("isRuning", false);
                animator.SetBool("isAttacking", false);
                return;
            // Run state 3
            case (uint)states.STATE_RUN:
                //0.035f
                this.transform.Translate(0, 0, 0.035f);
                animator.SetBool("isRuning", true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdle", false);
                return;
            // Attack state 4
            case (uint)states.STATE_ATTACK:
                animator.SetBool("isAttacking", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRuning", false);
                animator.SetBool("isIdle", false);
                return;
            // Go to last player pos 5
            case (uint)states.STATE_GO_LAST_POS:
                if (lastPlayerPos == null)
                    return;

                var d = lastPlayerPos.Value;
                agent.speed = 3;

                if (agent.SetDestination(d))
                {
                    agent.SetDestination(d);
                    animator.SetBool("isRuning", true);
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isIdle", false);

                    if ((d - transform.position).magnitude < 1)
                        lastPlayerPos = null;
                }
                return;
            // visibleTargets.Count != 0
            case (uint)states.STATE_ZERO_TARGETS:
                if (heal)
                {
                    _stage = 3;
                    lastPlayerPos = null;
                    updateSmartAi();
                    return;
                }


                Vector3 direction = player.position - this.transform.position;

                lastPlayerPos = player.position;
                isSee = true;
                direction.y = 0;

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                animator.SetBool("isIdle", false);
                agent.speed = 0;

                if (direction.magnitude > 5 && !heal)
                {
                    //0.035f
                    _stage = 3;
                }
                else if (direction.magnitude > 2.4f && !heal)
                {
                    this.transform.Translate(0, 0, 0.035f);
                    _stage = 2;
                }
                else
                    _stage = 4;
                updateSmartAi();
                return;

            default:
                return;
        }
    }



    private void GotoNextPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }
    private void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        int rand = Random.Range(0,5);
        animator.SetTrigger("Hurt");
        animator.SetBool("isDead", false);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (5 == 5)
        {
            heal = true;
            RunAway();
        }

    }

    private void RunAway()
    {
        lastPlayerPos = null;
        Vector3 direction2 = new Vector3(0,0,0);
        foreach (Transform point in points)
        {
            direction2 = point.localPosition - this.transform.position;
            if (bestPosition == null)
                bestPosition = point;
            else
            {
                Vector3 distToBestPos = bestPosition.localPosition - this.transform.position;
                if (direction2.magnitude < distToBestPos.magnitude)
                    bestPosition = point;
            }
        }
        agent.destination = bestPosition.position;
        Debug.Log("Best pos:" + bestPosition.position);
    }

    private void Heel()
    {
        this.transform.Translate(0, 0, 0.035f);
        if (agent.remainingDistance < 0.3f)
        {
            Debug.Log("Best pos:" + currentHealth);
            currentHealth += 50;
            heal = false;
            Debug.Log("Best pos:" + currentHealth);
            bestPosition = null;
        }
    }

    private void Die()
    {
        Debug.Log("Skeleton died!");

        animator.SetBool("isDead", true);

        GetComponent<Collider>().enabled = false;
        GetComponent<FieldOfView>().viewMeshFilter.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.enabled = false;
        

    }

    // Point towards the player
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearRadius);
    }
}