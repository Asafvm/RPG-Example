using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;


        public void Spawn(float damageText)
        {
            if (damageTextPrefab != null)
            {
                DamageText canvas = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
                canvas.GetComponentInChildren<TextMeshProUGUI>().text = damageText.ToString("0");
            }
        }
    }
}