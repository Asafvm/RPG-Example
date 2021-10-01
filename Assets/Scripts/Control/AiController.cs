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
        [SerializeField] float aggrevationTime = 5f;
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
        float timeSinceAggrevated = float.MaxValue;
        [SerializeField] float shoutDistance = 5f;

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
            if (IsAggrevated() && fighter.CanAttack(player))
            {
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

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggrevationTime;

        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }
        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;

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
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearby();
        }

        private void AggrevateNearby()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up,0);
            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.TryGetComponent(out AiController hitController))
                {
                    hitController.Aggrevate();
                }
            }    
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }

    
}
