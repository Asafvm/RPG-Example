using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Core;
using RPG.Attributes;

using RPG.Movement;
using RPG.Saving;

using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        
        [SerializeField] Transform handTransformRight = null;
        [SerializeField] Transform handTransformLeft = null;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] string defauleWeaponName = "Unarmed";
        Weapon currentWeapon = null;
        float timeSinceLastAttack = 0;
        Health target, healthComponent;



        private void Start()
        {
            healthComponent = GetComponent<Health>();
            if (currentWeapon == null)
                EquipWeapon(defaultWeapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(handTransformRight, handTransformLeft, animator);
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

            if (timeSinceLastAttack < currentWeapon.timeBetweenAttacks) return;
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
        public Health GetTargetHealth()
        {
            return target;
        }


        //Animation event
        public void Hit()
        {
            if(target!=null && target.TryGetComponent(out Health targetHealth))
            {
                if (currentWeapon.HasProjectile())
                    currentWeapon.LaunchProjectile(handTransformRight, handTransformLeft, targetHealth, gameObject);
                else
                    targetHealth.TakeDamage(gameObject, currentWeapon.GetDamage());
            }
                
        }
        // Animation event (wrapper for projectile)
        public void Shoot() => Hit();

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
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeapon.GetRange();
        }

        public object CaptureState()
        {
            if (currentWeapon == null)
                return defauleWeaponName;
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon;
            if (string.IsNullOrEmpty(weaponName))
                weapon = Resources.Load<Weapon>(defauleWeaponName);
            else
                weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

