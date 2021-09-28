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
        Color defaultColor;
        private void Awake()
        {
            health = GetComponent<Health>();
            defaultColor = healthBarRight.fillRect.GetComponent<Image>().color;
        }

        void Update()
        {
            float currentDisplay = healthBarRight.value;
            float newValueToDisplay = health.GetHealthPercentage() / 100;

            UpdateSliders(currentDisplay, newValueToDisplay);

        }

        private void UpdateSliders(float currentDisplay, float newValueToDisplay)
        {
            healthBarRight.value = Mathf.Lerp(currentDisplay, newValueToDisplay, Time.deltaTime * 5);
            healthBarLeft.value = Mathf.Lerp(currentDisplay, newValueToDisplay, Time.deltaTime * 5);
            if (((int)Mathf.Abs((currentDisplay * 100 - newValueToDisplay * 100))) > .01f)
            {
                healthBarRight.fillRect.GetComponent<Image>().color = Color.white;
                healthBarLeft.fillRect.GetComponent<Image>().color = Color.white;
            }
            else
            {
                healthBarRight.fillRect.GetComponent<Image>().color = defaultColor;
                healthBarLeft.fillRect.GetComponent<Image>().color = defaultColor;
            }
        }
    }
}