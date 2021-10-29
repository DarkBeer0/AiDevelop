using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();

        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isRunning", true);
            Debug.Log("Dick");
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                    anim.SetTrigger("isJump");
        }
        if(controller.velocity.y <= -5)
        {
            anim.SetBool("isFalling", true);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }
    }
}
