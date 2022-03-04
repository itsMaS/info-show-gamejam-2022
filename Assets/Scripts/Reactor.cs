using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Reactor : Interactable
{
    public GameConfigSO.Reactor config => State.config.reactor;

    public UnityEvent<float> OnAddFuel;
    public UnityEvent onExplode;

    float currentFuel;

    public void AddFuel(float amount)
    {
        OnAddFuel.Invoke(amount);

        currentFuel += amount;
        if(currentFuel >= config.baseFuelRequirement)
        {
            Explode();
            currentFuel -= config.baseFuelRequirement;
        }
    }
    public void Explode()
    {
        foreach (var col in Physics2D.OverlapCircleAll(transform.position, config.baseReactorRange))
        {
            if(col.TryGetComponent(out Mutable mutable))
            {
                mutable.Mutate(this);
            }
        }

        onExplode.Invoke();
    }

    public override void Click()
    {
        base.Click();
        AddFuel(config.baseFuelPerClick);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, config.baseReactorRange);
    }
}
