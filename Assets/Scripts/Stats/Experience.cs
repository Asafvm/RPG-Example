using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;

using UnityEngine;

namespace RPG.Stats {
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = -1f;
        public event Action OnExperienceGained;

        private void Awake()
        {
            if (experiencePoints < 0)
                experiencePoints = 0;
        }

        public object CaptureState()
        {
            Debug.Log($"Saving {experiencePoints} XP points");
            return experiencePoints;
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            Debug.Log($"XP = {experiencePoints} XP points");

            OnExperienceGained?.Invoke();
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            Debug.Log($"Restoring {experiencePoints} XP points");
            experiencePoints = (float)state;
            OnExperienceGained?.Invoke();
        }
    }
}
