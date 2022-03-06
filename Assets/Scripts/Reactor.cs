using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableDragTarget))]
public class Reactor : Interactable
{
    public GameConfigSO.Reactor config => State.config.reactor;

    public UnityEvent<float> OnAddFuel;
    public UnityEvent onExplode;

    public float currentFuel { get; private set; }

    public float fuelRequired => config.baseFuelRequirement;
    public float radioctivityOnExplosion => config.baseRadioactivity;

    InteractableDragTarget target;

    public override void Awake()
    {
        base.Awake();
        target = GetComponent<InteractableDragTarget>();
    }

    private void Start()
    {
        target.onInteractableDraggedOn.AddListener(DraggedOn);
    }

    private void DraggedOn(Interactable arg0)
    {
        if(arg0 is Human)
        {
            Human human = (Human)arg0;
            human.Die();
        }
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        OnAddFuel.Invoke(Mathf.Clamp(currentFuel/fuelRequired,0,1));
        if(currentFuel >= fuelRequired)
        {
            currentFuel -= fuelRequired;
            Explode();
        }
    }
    public void Explode()
    {
        Vector2 reactorCenter = (Vector2)transform.position + dragPointOffset;
        foreach (var col in Physics2D.OverlapCircleAll(reactorCenter , config.baseReactorRange))
        {
            if(col.TryGetComponent(out Human human))
            {
                if(Vector2.Distance(human.transform.position, reactorCenter) <= config.baseReactorRange)
                {
                    float radioctivity = config.radioctivityFaloff.Evaluate(Mathf.InverseLerp(config.baseReactorRange, 0, Vector2.Distance(human.transform.position, reactorCenter))) * radioctivityOnExplosion;
                    human.Mutate(radioctivity);
                }
            }
        }

        onExplode.Invoke();
    }

    public override void Click()
    {
        base.Click();
        AddFuel(config.baseFuelPerClick);
    }
}
