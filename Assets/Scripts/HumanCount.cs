using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HumanCount : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        State.Events.OnHumanDeath.AddListener(RemoveHuman);
        State.Events.OnHumanSpawn.AddListener(AddHuman);

        text.SetText($"{State.Instance.Humans.Count}/{State.config.gameplay.maxHumans}");
    }


    private void AddHuman(Human arg0)
    {
        text.SetText($"{State.Instance.Humans.Count}/{State.config.gameplay.maxHumans}");
    }

    private void RemoveHuman(Human arg0)
    {
        text.SetText($"{State.Instance.Humans.Count}/{State.config.gameplay.maxHumans}");
    }
}
