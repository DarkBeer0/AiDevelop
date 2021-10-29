﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraFollowRotate : MonoBehaviour {

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition;

    [SerializeField]
    private Space offsetPositionSpace = Space.Self;

    [SerializeField]
    private bool lookAt = true;

    public bool reallyCamera = false;
    public bool RotateAroundPlayer = false;
    public float RotationSpeed = 5.0f;

    public Vector3 target2;
    public string act; // what we do on click

    //Code to be place in a MonoBehaviour with a GraphicRaycaster component
    public GraphicRaycaster m_Raycaster;
    public PointerEventData m_PointerEventData;
    public EventSystem m_EventSystem;
    public List<RaycastResult> RayResults;

    private void LateUpdate()
    {
        
        
        Refresh();
        
    }
    void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        RayResults = new List<RaycastResult>();
        
        RotateAroundPlayer = false;
       

        //Create the PointerEventData with null for the EventSystem


    }

    void Update()
    {
        moveCamera();
       // CharacterController controller = GetComponent<CharacterController>();
       // transform.Rotate(0, Input.GetAxis("Mouse X") * 8, 0);
       // Debug.Log(Input.GetAxis("Mouse X"));
    }




    // target = hit.point;





    public void moveCamera()
    {
        // rotate camera on not
        
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * RotationSpeed,
                Vector3.up);
            Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * RotationSpeed,
                Vector3.left);

            offsetPosition = camTurnAngle * offsetPosition;
            offsetPosition = camTurnAngleY * offsetPosition;
        
    }



        public void Refresh()
    {


        if (target == null)
        {
            Debug.LogWarning("Missing target ref !", this);

            return;
        }

        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            transform.position = target.TransformPoint(offsetPosition);
        }
        else
        {
            transform.position = target.position + offsetPosition;
        }

        // compute rotation
        if (lookAt)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.rotation = target.rotation;
        }
    }
}