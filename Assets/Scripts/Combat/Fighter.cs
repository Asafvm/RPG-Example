using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Core;
using RPG.Movement;

using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float timeBetweenAttacks = 1f;
        float timeSinceLastAttack = 0;
        Health target, healthComponent;

        private void Start()
        {
            healthComponent = GetComponent<Health>();
        }
        private void Update()
        {
            if (!healthComponent.IsAlive) return;
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if(!TargetInRange(target.transform))
                GetComponent<Mover>().MoveTo(target.transform.position,1f);
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
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
            if(target!=null && target.TryGetComponent(out Health targetHealth))
                targetHealth.TakeDamage(weaponDamage);
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Mover>().Cancel();
            StopAttack();

        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        private bool TargetInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < weaponRange;
        }
    }
}

