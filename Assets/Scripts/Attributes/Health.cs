using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        float health = -1f;
        Animator animator;
        public bool IsAlive { get => health > 0; }

        private void Start()
        {
            animator = GetComponent<Animator>();
            if(health < 0)
            {
                health = GetComponent<BaseStats>().GetHealth();
                animator.SetBool("isAlive", IsAlive);
            }
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            health = Mathf.Max(0, health - damage);
            if (IsAlive) return;
            AwardXP(instigator);
            Die();

        }

        private void AwardXP(GameObject instigator)
        {
            if(instigator.TryGetComponent(out Experience instigatorXpComponent))
            {

                instigatorXpComponent.GainExperience(GetComponent<BaseStats>().GetXPValue());
            }
            
        }

        private void Die()
        {
                animator.SetBool("isAlive",IsAlive);
                GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            GetComponent<Animator>().SetBool("isAlive", IsAlive);

        }

        public float GetHealthPercentage()
        {
            return 100 * health / GetComponent<BaseStats>().GetHealth();
        }
    }
}


