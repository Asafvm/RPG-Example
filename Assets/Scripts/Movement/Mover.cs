using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{

    private Transform targetDestination;
    [SerializeField] private GameObject cameraFocalPoint;
    Ray lastRay;
    [SerializeField] private float rotationSpeed;

    // Update is called once per frame
    void Update()
    {


        MoveToCursor();
        UpdateAnimator();

        //RotateCamera();


    }

    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        GetComponent<Animator>().SetFloat("forwardSpeed", localVelocity.z);
    }

    private void RotateCamera()
    {
        float horizontal = Input.GetAxis("Horizontal");
        cameraFocalPoint.transform.Rotate(Vector3.up , rotationSpeed * horizontal * Time.deltaTime);

    }
    
    private void MoveToCursor()
    {
        if (Input.GetMouseButton(0))
        {
            lastRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(lastRay, out RaycastHit hitInfo))
            {
                GetComponent<NavMeshAgent>().SetDestination(hitInfo.point);
            }
        }
    }


}
