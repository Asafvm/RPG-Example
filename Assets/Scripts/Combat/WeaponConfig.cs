using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Attributes;

using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        public string name = "";
        [SerializeField] AnimatorOverrideController animtorOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] [Range(2, 10)] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float weaponPercentageBonus = 0;
        [SerializeField] [Range(.5f, 3f)] public float timeBetweenAttacks = 1f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform handTransformRight, Transform handTransformLeft, Animator animator)
        {
            DestroyOldWeapon(handTransformRight, handTransformLeft);
            Weapon weapon = null;
            if (weaponPrefab != null)
            {
                weapon = Instantiate(weaponPrefab, GetHandTransform(handTransformRight, handTransformLeft));
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animtorOverride != null)
                animator.runtimeAnimatorController = animtorOverride;
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform handTransformRight, Transform handTransformLeft)
        {
            Transform oldWeapon = handTransformRight.Find(weaponName);

            if (oldWeapon == null)
                oldWeapon = handTransformLeft.Find(weaponName);
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);

        }

        private Transform GetHandTransform(Transform handTransformRight, Transform handTransformLeft)
        {
            return isRightHanded ? handTransformRight : handTransformLeft;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform handTransformRight, Transform handTransformLeft, Health target, GameObject instigator, float calaulatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(handTransformRight, handTransformLeft).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calaulatedDamage);
        }

        public float GetDamage() => weaponDamage;
        public float GetRange() => weaponRange;

        internal float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }
    }
}
