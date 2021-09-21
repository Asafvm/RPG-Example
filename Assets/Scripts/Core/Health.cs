using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Core {
    [RequireComponent(typeof(ActionScheduler))]
    public class Health : MonoBehaviour
    {
        [SerializeField] float health = 100f;
        public bool IsAlive { get => health > 0; }
        public void TakeDamage(float damage)
        {
            health = Mathf.Max(0, health - damage);
            if (health > 0) return;
            if(TryGetComponent(out Animator animator)){
                animator.SetTrigger("Die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }    
    }
}


