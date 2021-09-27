using System.Collections;
using System.Collections.Generic;

using RPG.Attributes;

using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField][Range(1,99)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;


        public float GetHealth()
        {
            return progression.GetStat(Stat.Health, characterClass,startingLevel);
        }

        public float GetXPValue()
        {
            return progression.GetStat(Stat.ExperienceReward, characterClass, startingLevel);
        }
        public float GetExperiencePercentage()
        {
            if (TryGetComponent(out Experience experience))
            {
                int currentLevel = GetLevel();
                float currentXP = experience.GetPoints();
                if (currentLevel == 1)
                    return currentXP / GetXpForLevel(currentLevel);
                return (currentXP - GetXpForLevel(currentLevel-1)) / GetXpForLevel(currentLevel);
            }
            return 0;


        }
        public int GetLevel()
        {
            if(TryGetComponent(out Experience experience))
            {
                float currentXP = experience.GetPoints();
                int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
                for (int level = 1; level < maxLevel; level++)
                {
                    if (currentXP >= GetXpForLevel(level)) continue;
                    return level;


                }
                return maxLevel + 1;
            }
            return 1;
        }

        private float GetXpForLevel(int level)
        {
            return progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
        }
    }
}
