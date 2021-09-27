using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;

using UnityEngine;

namespace RPG.Stats {
    public class Experience : MonoBehaviour, ISaveable
    {
        float experiencePoints = -1f;

        private void Start()
        {
            if (experiencePoints < 0)
                experiencePoints = 0;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}
