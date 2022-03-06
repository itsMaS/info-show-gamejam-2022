using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : Interactable
{
    Collider2D col;
    public override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
    }
    public override void Click()
    {
        base.Click();

        var human = State.Instance.SpawnHuman(transform.position, new Genome(true), 0.5f);

        human.SetMovePosition(transform.position + Vector3.up * -5);

        col.enabled = false;
        DOVirtual.DelayedCall(0.5f, () => col.enabled = true);
    }
}
