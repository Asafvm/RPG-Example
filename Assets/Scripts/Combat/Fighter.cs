using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Core;
using RPG.Movement;

using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float timeBetweenAttacks = 1f;
        float timeSinceLastAttack = 0;
        Health target;
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if(!TargetInRange(target.transform))
                GetComponent<Mover>().MoveTo(target.transform.position);
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(CombatTarget combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && targetToTest.IsAlive && targetToTest.transform != transform;
        }

        private void AttackBehaviour()
        {
            //transform.LookAt(target.transform);

            if (timeSinceLastAttack < timeBetweenAttacks) return;
            if(target.TryGetComponent(out Health targetHealth))
                {
                if (targetHealth.IsAlive)
                {
                    transform.LookAt(targetHealth.transform);
                    GetComponent<Animator>().ResetTrigger("stopAttack");
                    GetComponent<Animator>().SetTrigger("Attack");
                    timeSinceLastAttack = 0;
                }
                else
                {
                    Cancel();
                }
            }
           

        }

        //Animation event
        public void Hit()
        {
            if(target.TryGetComponent(out Health targetHealth))
                targetHealth.TakeDamage(weaponDamage);
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
                
        }

        private bool TargetInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < weaponRange;
        }
    }
}

