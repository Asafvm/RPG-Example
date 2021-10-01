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
        [SerializeField] WeaponConfig defaultWeapon = null;
        LazyValue<Weapon> currentWeapon;
        WeaponConfig currentWeaponConfig;
        float timeSinceLastAttack = Mathf.Infinity;
        Health target, healthComponent;


        private void Awake()
        {
            healthComponent = GetComponent<Health>(); 
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(InitWeapon);

        }

        private Weapon InitWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(handTransformRight, handTransformLeft, animator);
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
            if (GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !TargetInRange(combatTarget.transform)) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && targetToTest.IsAlive && targetToTest.transform != transform;
        }


        private void AttackBehaviour()
        {
            if (timeSinceLastAttack < currentWeaponConfig.timeBetweenAttacks) return;
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
                if(currentWeapon.value != null)
                {
                    currentWeapon.value.OnHit();
                }
                float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

                if (currentWeaponConfig.HasProjectile())
                    currentWeaponConfig.LaunchProjectile(handTransformRight, handTransformLeft, targetHealth, gameObject, damage);
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
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        }

        public object CaptureState()
        {
            Debug.Log($"Saving {gameObject.name} WeaponConfig = {currentWeaponConfig.name}");
            Debug.Log($"Saving {gameObject.name} Weapon = {currentWeapon.value.name}");

            if (currentWeapon.value == null)
                return currentWeapon.value.name;
            return defaultWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Debug.Log($"Restoring {gameObject.name} WeaponName = {weaponName} or {defaultWeapon.name}");
            WeaponConfig weapon;
            if (string.IsNullOrEmpty(weaponName))
                weapon = Resources.Load<WeaponConfig>(defaultWeapon.name);
            else
                weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
            Debug.Log($"Restoring {gameObject.name} WeaponConfig = {currentWeaponConfig.name}");
            Debug.Log($"Restoring {gameObject.name} Weapon = {currentWeapon.value.name}");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
                yield return currentWeaponConfig.GetDamage();
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
                yield return currentWeaponConfig.GetPercentageBonus();
        }
    }
}

