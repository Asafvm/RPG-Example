using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering;

using static Cinemachine.DocumentationSortingAttribute;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookup = null;


        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookup[characterClass][stat];
            if (levels.Length < level) return 0;
            return levels[level-1];
           
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = lookup[characterClass][stat];
            return levels.Length;
        }
        private void BuildLookup()
        {
            if (lookup != null) return;
            lookup = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            Dictionary<Stat, float[]> stats = null;
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                stats = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    stats.Add(progressionStat.stat, progressionStat.levels);
                }
                lookup.Add(progressionClass.classType, stats);
            }
        }

        [Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass classType;
            public ProgressionStat[] stats;
        }

        [Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}
