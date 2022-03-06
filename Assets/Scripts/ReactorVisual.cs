using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Reactor))]
public class ReactorVisual : MonoBehaviour
{
    [Header("Dependancies")]
    [SerializeField] SpriteRenderer body;
    [SerializeField] SpriteRenderer explosion;
    [SerializeField] SpriteRenderer rangeIndicator;
    [SerializeField] SpriteRenderer chargeSprite;
    [SerializeField] AudioSource levelUp;

    [Header("Parameters")]
    [SerializeField] float clickAnimationDuration;
    [SerializeField] float chargeAnimationSpeed;

    Reactor reactor;

    float chargeTarget;
    float chargeVelocity;
    float chargeCurrent;

    float initialBodyScale = 1;
    private void Awake()
    {
        reactor = GetComponent<Reactor>();
        initialBodyScale = body.transform.localScale.x;
    }

    private void Start()
    {
        reactor.OnAddFuel.AddListener(AddFuel);
        reactor.onExplode.AddListener(Explode);

        rangeIndicator.transform.localScale = Vector3.one * reactor.config.baseReactorRange * 2;

        State.Events.OnLevelComplete.AddListener(() => levelUp.Play());
    }

    private void Explode()
    {
        explosion.SetOpacity(1);
        explosion.DOFade(0, 0.5f);

        explosion.transform.localScale = Vector3.zero;
        explosion.transform.DOScale(reactor.config.baseReactorRange * 2, 0.5f);

        float progress = reactor.currentFuel / reactor.fuelRequired;
        chargeSprite.material.SetFloat("_progress", progress);
        chargeTarget = progress;
    }

    private void Update()
    {
        chargeCurrent = Mathf.SmoothDamp(chargeCurrent, chargeTarget, ref chargeVelocity, 1 / chargeAnimationSpeed);
        chargeSprite.material.SetFloat("_progress", chargeCurrent);
    }

    private void AddFuel(float progress)
    {
        chargeTarget = progress;
        body.transform.DOScale(initialBodyScale*0.8f, clickAnimationDuration/2).SetLoops(2, LoopType.Yoyo).From(initialBodyScale).SetEase(Ease.InOutSine);
    }
}
