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
    void Start()
    {
        currentHealth = maxHealth;

        field = GetComponent<FieldOfView>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        
        GotoNextPoint();
    }

    void Update()
    {
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

        if (Vector3.Distance(player.position, transform.position) < hearRadius && Vector3.Distance(player.position, transform.position) >= 1 && field.visibleTargets.Count == 0)
        {
            FaceTarget();
        }
        

        updateSmartAi();
    }

    void updateSmartAi()
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

                //agent.destination = d;
                //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(d), 0.1f);
                //this.transform.position = Vector3.MoveTowards(transform.position, lastPlayerPos.Value, 0.035f);
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
                Vector3 direction = player.position - this.transform.position;
                //float angle = Vector3.Angle(direction, this.transform.forward);

                lastPlayerPos = player.position;
                isSee = true;
                //Vector3 direction = player.position - this.transform.position; - поиск идет вокруг.
                direction.y = 0;

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                animator.SetBool("isIdle", false);
                agent.speed = 0;

                if (direction.magnitude > 5)
                {
                    //0.035f
                    _stage = 3;
                    updateSmartAi();
                    return;
                }
                else if (direction.magnitude > 2.4f)
                {
                    this.transform.Translate(0, 0, 0.035f);
                    _stage = 2;
                    updateSmartAi();
                    return;
                }
                else
                {
                    _stage = 4;
                    updateSmartAi();
                    return;
                }
                return;
            default:
                return;
        }
    }

    

    void GotoNextPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }
    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");
        animator.SetBool("isDead", false);


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Skeleton died!");

        animator.SetBool("isDead", true);

        GetComponent<Collider>().enabled = false;
        GetComponent<FieldOfView>().viewMeshFilter.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.enabled = false;
        

    }

    // Point towards the player
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearRadius);
    }
}