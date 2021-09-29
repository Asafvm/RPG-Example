using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine.Events;
using System;
using GameDevTV.Utils;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> health;
        Animator animator;
        BaseStats baseStats;

        [SerializeField] UnityEvent<float> takeDamage;
        public bool IsAlive { get => health.value > 0; }
        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            animator = GetComponent<Animator>();
            health = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private void Start()
        {
            if (health.value < 0)
            {
                animator.SetBool("isAlive", IsAlive);
            }
        }
        private void OnEnable()
        {
            baseStats.levelup += OnLevelUp;
        }
        private void OnDisable()
        {
            baseStats.levelup -= OnLevelUp;

        }
        private void OnLevelUp()
        {
            if (IsAlive)
                health.value = baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {

            health.value = Mathf.Max(0, health.value - damage);
            if (!IsAlive)
            {
                AwardXP(instigator);
                Die();
            }
            else 
            { 
                takeDamage?.Invoke(damage); 
            }


        }

        private void AwardXP(GameObject instigator)
        {
            if (instigator.TryGetComponent(out Experience instigatorXpComponent))
            {

                instigatorXpComponent.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }

        }

        private void Die()
        {
            animator.SetBool("isAlive", IsAlive);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            GetComponent<Animator>().SetBool("isAlive", IsAlive);

        }

        public float GetHealthPercentage()
        {
            return 100 * health.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }
    }
}


