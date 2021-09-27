using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;

using UnityEngine;

public class LevelDisplay : MonoBehaviour
{

    BaseStats baseStats;
void Start()
{
        baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();

    }

void Update()
{
        GetComponent<TextMeshProUGUI>().text = $"{baseStats.GetLevel()}";
    }
}
