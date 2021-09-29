using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Core;
using RPG.Attributes;

using RPG.Movement;
using RPG.Saving;

using UnityEngine;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifier
    {
        
        [SerializeField] Transform handTransformRight = null;
        [SerializeField] Transform handTransformLeft = null;
        //[SerializeField] Weapon defaultWeapon = null;
        [SerializeField] string defauleWeaponName = "Unarmed";
        LazyValue<Weapon> currentWeapon;
        float timeSinceLastAttack = 0;
        Health target, healthComponent;


        private void Awake()
        {
            healthComponent = GetComponent<Health>();
            currentWeapon = new LazyValue<Weapon>(InitWeapon);
        }

        private Weapon InitWeapon()
        {
            Weapon defaultWeapon = Resources.Load<Weapon>(defauleWeaponName);

            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
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

            if (timeSinceLastAttack < currentWeapon.value.timeBetweenAttacks) return;
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
                float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
                Debug.Log($"Damage: {damage} points");

                if (currentWeapon.value.HasProjectile())
                    currentWeapon.value.LaunchProjectile(handTransformRight, handTransformLeft, targetHealth, gameObject, damage);
                else
                {
                    targetHealth.TakeDamage(gameObject, damage);
                }
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
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeapon.value.GetRange();
        }

        public object CaptureState()
        {
            if (currentWeapon == null)
                return defauleWeaponName;
            return currentWeapon.value.name;
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
                yield return currentWeapon.value.GetDamage();
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
                yield return currentWeapon.value.GetPercentageBonus();
        }
    }
}

