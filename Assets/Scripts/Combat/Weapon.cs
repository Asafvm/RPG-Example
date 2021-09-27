using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Attributes;

using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        private string name = "";
        [SerializeField] AnimatorOverrideController animtorOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] [Range(2, 10)] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] [Range(.5f, 3f)] public float timeBetweenAttacks = 1f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public void Spawn(Transform handTransformRight, Transform handTransformLeft, Animator animator)
        {
            DestroyOldWeapon(handTransformRight, handTransformLeft);

            if (weaponPrefab != null)
            {
                GameObject weapon = Instantiate(weaponPrefab, GetHandTransform(handTransformRight, handTransformLeft));
                weapon.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animtorOverride != null)
                animator.runtimeAnimatorController = animtorOverride;
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

        }

        private void DestroyOldWeapon(Transform handTransformRight, Transform handTransformLeft)
        {
            Transform oldWeapon = handTransformRight.Find(weaponName);

            if (oldWeapon == null)
                oldWeapon = handTransformLeft.Find(weaponName);
            if (oldWeapon == null) return;
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

        public void LaunchProjectile(Transform handTransformRight, Transform handTransformLeft, Health target, GameObject instigator)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(handTransformRight, handTransformLeft).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, weaponDamage);
        }

        public float GetDamage() => weaponDamage;
        public float GetRange() => weaponRange;


    }
}
