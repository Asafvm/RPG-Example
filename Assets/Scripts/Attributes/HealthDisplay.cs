using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {
            int healthPer = Mathf.FloorToInt(health.GetHealthPercentage());

            GetComponent<TextMeshProUGUI>().text = $"{healthPer}%";
            GetComponentInParent<Slider>().value = healthPer/100f;
        }
    }
}