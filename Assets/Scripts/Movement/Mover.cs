using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private NavMeshAgent navMeshAgent;
        private Health health;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = health.IsAlive;

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            GetComponent<Animator>().SetFloat("forwardSpeed", localVelocity.z);
        }

        public void StartMoveAction(Vector3 destination)
        {
            if(TryGetComponent(out ActionScheduler actionScheduler))
                actionScheduler.StartAction(this);
            MoveTo(destination);
        }
        public void MoveTo(Vector3 point)
        {
            navMeshAgent.destination = point;
            navMeshAgent.isStopped = false;
        }
    

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
    }
}
