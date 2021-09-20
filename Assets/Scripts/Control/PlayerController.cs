using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithCombat()) return;

            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                if(hit.transform.TryGetComponent(out CombatTarget target))
                {
                    if (!GetComponent<Fighter>().CanAttack(target)) continue;

                    if (Input.GetMouseButtonDown(0))
                    {
                        
                        GetComponent<Fighter>().Attack(target);
                    }
                    return true;
                    
                }
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hitInfo))
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hitInfo.point);
                }
                return true;
            }
            return false;
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}




