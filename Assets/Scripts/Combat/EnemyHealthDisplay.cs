using System.Collections;
using System.Collections.Generic;

using RPG.Attributes;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health health;
        [SerializeField] Slider healthBarRight, healthBarLeft;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
                healthBarRight.value = health.GetHealthPercentage() / 100;
                healthBarLeft.value = health.GetHealthPercentage() / 100;
          
        }
    }
}