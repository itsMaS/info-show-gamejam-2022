using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : Interactable
{
    Collider2D pickCollider;
    public override void Awake()
    {
        base.Awake();
        pickCollider = GetComponent<Collider2D>();
    }
    public override void Click()
    {
        base.Click();

        var human = State.Instance.SpawnHuman(transform.position, new Genome(true), 0.5f);

        human.SetMovePosition(transform.position + Vector3.up * -5);

        pickCollider.enabled = false;
        DOVirtual.DelayedCall(0.5f, () => pickCollider.enabled = true);
    }
}
