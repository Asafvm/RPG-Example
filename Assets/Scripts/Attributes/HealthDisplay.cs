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
            float healthPer = health.GetHealthPercentage();
            Slider healthSlider = GetComponentInParent<Slider>();
            float currentDisplay = healthSlider.value;
            float newValueToDisplay = healthPer / 100;

            GetComponent<TextMeshProUGUI>().text = $"{healthPer.ToString("0")}%";
            GetComponentInParent<Slider>().value = Mathf.Lerp(currentDisplay, newValueToDisplay, Time.deltaTime * 5); ;
        }
    }
}