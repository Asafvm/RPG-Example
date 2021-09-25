using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Core {
    [RequireComponent(typeof(ActionScheduler))]
    public class Health : MonoBehaviour,ISaveable
    {
        [SerializeField] float health = 100f;
        public bool IsAlive { get => health > 0; }

 

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(0, health - damage);
            if (IsAlive) return;
               Die();
            
        }

        private void Die()
        {
            if (TryGetComponent(out Animator animator))
            {
                animator.SetBool("isAlive",IsAlive);
                GetComponent<ActionScheduler>().CancelCurrentAction();

            }
        }
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            if (TryGetComponent(out Animator animator))
                animator.SetBool("isAlive", IsAlive);

        }
    }
}


