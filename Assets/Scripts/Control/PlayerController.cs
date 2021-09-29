using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public partial class PlayerController : MonoBehaviour
    {
        private Health health;

        [SerializeField] float maxNavmeshProjectionDistance = 1f;
        [SerializeField] float maxPathDistance = 30f;

        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;

        }
        [SerializeField] CursorMapping[] cursorMappings = null;

        // Start is called before the first frame update
        void Awake()
        {
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
{
            if (InteractWithUI()) return;
            

            if (!health.IsAlive)
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;

            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            foreach(RaycastHit hit in RaycastAllSorted())
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for(int i=0;i<hits.Length;i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

 

        private bool InteractWithMovement()
        {
            if(RaycastNavmesh(out Vector3 target))
            {

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavmesh(out Vector3 target)
        {
            target = new Vector3();
            if(Physics.Raycast(GetMouseRay(), out RaycastHit hit))
            {
                if(NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavmeshProjectionDistance, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    target = navMeshHit.position;
                    if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path))
                    {
                        if (path.status != NavMeshPathStatus.PathComplete) return false;
                        float pathLength = GetPathLength(path);

                        if (pathLength > maxPathDistance) return false;
                    }

                    return true;
                }
                return false;
            }


            return false;
        }

        private static float GetPathLength(NavMeshPath path)
        {
            float pathLength = 0;
            if (path.corners.Length < 2) return pathLength;
            for (int i = 0; i < path.corners.Length - 1; i++)
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            return pathLength;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping map in cursorMappings)
                if (map.type == type) return map;
            return cursorMappings[0];
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}




