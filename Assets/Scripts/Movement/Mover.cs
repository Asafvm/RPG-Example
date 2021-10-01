using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Attributes;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxPathDistance = 30f;

        private NavMeshAgent navMeshAgent;
        private Health health;

        private void Awake()
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

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            if(TryGetComponent(out ActionScheduler actionScheduler))
                actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path))
            {
                if (path.status != NavMeshPathStatus.PathComplete) return false;
                if (GetPathLength(path) > maxPathDistance) return false;
            }
            return true;
        }
  
        public void MoveTo(Vector3 point, float speedFraction)
        {
            navMeshAgent.destination = point;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        [Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.rotation.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }

        private static float GetPathLength(NavMeshPath path)
        {
            float pathLength = 0;
            if (path.corners.Length < 2) return pathLength;
            for (int i = 0; i < path.corners.Length - 1; i++)
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            return pathLength;
        }
    }
}
