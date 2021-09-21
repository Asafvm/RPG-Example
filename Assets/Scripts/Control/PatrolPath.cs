using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] const float waypointGizmoRadius = 0.3f;
        private int _numberOfPoints = 0;
        public int NumberOfPoints { set => _numberOfPoints = value; get => _numberOfPoints; }
        private void OnDrawGizmos()
        {
            NumberOfPoints = transform.childCount;
            for (int i = 0; i < NumberOfPoints; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }

        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount) return 0;
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

    }
}