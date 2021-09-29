using System;
using System.Collections;
using System.Collections.Generic;

using GameDevTV.Utils;

using RPG.Attributes;

using UnityEngine;
using UnityEngine.Events;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelupEffect;
        LazyValue<int> currentLevel;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action levelup;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(InitialzeCurrentLevel);
        }

        private int InitialzeCurrentLevel()
        {
            return CalculateLevel();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }
        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
        }
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                //Leveled up
                currentLevel.value = newLevel;
                ShowlevelupEffect();
                levelup?.Invoke();
            }
        }

        private void ShowlevelupEffect()
        {
            Instantiate(levelupEffect, transform.position, levelupEffect.transform.rotation);
        }

        public float GetStat(Stat stat)
        {
            if (stat == Stat.Damage)
                Debug.Log($"{transform.name} Base stats :: base = {GetBaseStat(stat)}, add = {GetAdditiveModifier(stat)}, Per = {(1 + GetPercentageModifier(stat) / 100)}");
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }



        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float additiveModifier = 0;
            foreach (IModifier modifiers in GetComponents<IModifier>())
            {
                foreach (float modifier in modifiers.GetAdditiveModifiers(stat))
                {
                    additiveModifier += modifier;
                }
            }
            return additiveModifier;

        }
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float additiveModifier = 0;
            foreach (IModifier modifiers in GetComponents<IModifier>())
            {
                foreach (float modifier in modifiers.GetPercentageModifiers(stat))
                {
                    additiveModifier += modifier;
                }
            }
            return additiveModifier;
        }
        public float GetExperiencePercentage()
        {
            if (TryGetComponent(out Experience experience))
            {
                int currentLevel = GetLevel();
                float currentXP = experience.GetPoints();
                if (currentLevel == 1)
                    return currentXP / GetXpForLevel(currentLevel);
                return (currentXP - GetXpForLevel(currentLevel - 1)) / GetXpForLevel(currentLevel);
            }
            return 0;


        }

        public int GetLevel()
        {
            return currentLevel.value;
        }
        public int CalculateLevel()
        {
            if (experience == null) return 1;
            float currentXP = experience.GetPoints();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level < maxLevel; level++)
            {
                if (currentXP >= GetXpForLevel(level)) continue;
                return level;


            }
            return maxLevel + 1;

        }

        private float GetXpForLevel(int level)
        {
            return progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
        }
    }
}
