using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ñhase : MonoBehaviour
{

    public Transform player;
    private Animator animator;
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private FieldOfView field;

    void Start()
    {
        field = GetComponent<FieldOfView>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        GotoNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        bool isSee = false;
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }

        if (field.visibleTargets.Count != 0)
        {
            isSee = true;
            direction.y = 0;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            animator.SetBool("isIdle", false);
            agent.speed = 0;

            if (direction.magnitude > 5)
            {
                //0.035f
                this.transform.Translate(0, 0, 0.015f);
                animator.SetBool("isRuning", true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdle", false);
            }
            else if (direction.magnitude > 4f)
            {
                this.transform.Translate(0, 0, 0.015f);
                animator.SetBool("isWalking", true);
                animator.SetBool("isRuning", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isIdle", false);
            }
            else
            {
                animator.SetBool("isAttacking", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isRuning", false);
                animator.SetBool("isIdle", false);
            }
        }
        else
        {
            agent.speed = 3;
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isRuning", false);
            animator.SetBool("isAttacking", false);
        }
        if (points.Length == 0 && isSee == false)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRuning", false);
            animator.SetBool("isAttacking", false);
        }

    }
    void GotoNextPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }


}
