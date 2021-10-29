using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float rotateSpeed = 1.0F;
    public float shiftSpeed = 3.0F;


    public Transform attackPoint;
    public float attackRange = 0.5f;


    public bool isSprint = false;
    private Vector3 moveDirection = Vector3.zero;
    public Vector2 turn;
    public Vector3 deltaMove;

    Transform cameraTransform;
    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown("left shift"))
        {
            isSprint = true;
        }
        else if (Input.GetKeyUp("left shift"))
        {
            isSprint = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            if (isSprint)
            {
                moveDirection *= (speed + shiftSpeed);
            }
            else
            {
                moveDirection *= speed;
            }


            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

            
        }
        moveDirection.y -= gravity * Time.deltaTime;
        
        controller.Move(moveDirection * Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Horizontal") / rotateSpeed, 0);
     
    }
}