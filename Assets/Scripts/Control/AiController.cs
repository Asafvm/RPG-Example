using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;

using UnityEngine;
using GameDevTV.Utils;

namespace RPG.Control {
    public class AiController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 2f;
        [SerializeField] float dwellingTime = 1f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField][Range(0,1)] float patrolSpeedFraction = 0.2f;

        private Fighter fighter;
        private int currentWaypointIndex = 0;
        Health health;
        GameObject player;
        LazyValue<Vector3> startingPos;
        float timeSinceLastSawPlayer = float.MaxValue;
        float timeSinceArrivedAtWaypoint = float.MaxValue;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            startingPos = new LazyValue<Vector3>(InitStartingPos);
        }

        private Vector3 InitStartingPos()
        {
            return transform.position;
        }

        private void Start()
        {
            startingPos.ForceInit();
        }
        void Update()
        {
            if (!health.IsAlive) return;
            if (PlayerInRange(player) && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                GuardBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void GuardBehaviour()
        {
            Vector3 nextPosition = startingPos.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                    
           
            nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > dwellingTime)
                GetComponent<Mover>().StartMoveAction(nextPosition, patrolSpeedFraction);

        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);

        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
        }

        private bool PlayerInRange(GameObject player)
        {
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }

    
}
