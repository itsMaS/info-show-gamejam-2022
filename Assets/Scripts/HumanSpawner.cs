using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : Interactable
{
    public override void Click()
    {
        base.Click();

        State.Instance.SpawnHuman(transform.position, new Genome(true), 0.5f);
    }
}
