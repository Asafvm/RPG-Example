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
                DamageText instance = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
                instance.SetValue(damageText);
                
            }
        }
    }
}