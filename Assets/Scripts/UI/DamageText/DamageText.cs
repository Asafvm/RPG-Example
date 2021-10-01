using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

namespace RPG.UI {
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI damageText;
        private void Start()
        {
            Destroy(gameObject, 1f);
        }

        internal void SetValue(float damage)
        {
            damageText.text = damage.ToString();
        }
    }
}
