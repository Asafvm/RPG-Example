using System.Collections;
using System.Collections.Generic;

using RPG.Attributes;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats {
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        BaseStats baseStats;
        // Start is called before the first frame update
        void Start()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();


        }

        // Update is called once per frame
        void Update()
        {

            float xpPercentage = baseStats.GetExperiencePercentage();
            GetComponentInParent<Slider>().value = xpPercentage;

            GetComponent<TextMeshProUGUI>().text = $"{(xpPercentage * 100).ToString("0.00")}%";
        }
    }
}
