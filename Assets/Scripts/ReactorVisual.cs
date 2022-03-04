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

    [Header("Parameters")]
    [SerializeField] float clickAnimationDuration;

    Reactor reactor;

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
    }

    private void Explode()
    {
        Debug.Log($"EXPLODE");

        explosion.SetOpacity(1);
        explosion.DOFade(0, 0.5f);

        explosion.transform.localScale = Vector3.zero;
        explosion.transform.DOScale(reactor.config.baseReactorRange * 2, 0.5f);

    }

    private void AddFuel(float arg0)
    {
        body.transform.DOScale(initialBodyScale*0.8f, clickAnimationDuration/2).SetLoops(2, LoopType.Yoyo).From(initialBodyScale).SetEase(Ease.InOutSine);
    }
}
