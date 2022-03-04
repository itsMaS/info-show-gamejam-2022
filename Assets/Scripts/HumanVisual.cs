using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Human))]
public class HumanVisual : MonoBehaviour
{
    [Header("Dependancies")]
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] SpriteRenderer body;

    [Header("Parameters")]
    [SerializeField] float rotationAmplitude;
    [SerializeField] float rotationAccelerationSpeed;
    [SerializeField] float pickupScale;
    [SerializeField] float walkSineSpeed;
    [SerializeField] float walkSineAmplitude;
    [SerializeField] float walkShadowSineAmplitude;

    Human human;

    float baseShadowOpacity;
    Quaternion rotationTarget;
    Vector2 initialLocalBodyPosition;
    Vector2 initialShadowScale;

    private void Awake()
    {
        human = GetComponent<Human>();

        baseShadowOpacity = shadow.color.a;
        initialLocalBodyPosition = body.transform.localPosition;
        initialShadowScale = shadow.transform.localScale;
    }

    private void Start()
    {
        human.onDrag.AddListener(Dragging);
        human.onDragBegin.AddListener(BeginDrag);
        human.onDragEnd.AddListener(EndDrag);
    }

    private void Update()
    {
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, rotationTarget, Time.deltaTime * rotationAccelerationSpeed);

        float wave = (1 + Mathf.Sin(Time.time * walkSineSpeed)) * human.velocity.magnitude;
        body.transform.localPosition = initialLocalBodyPosition + Vector2.up * wave * walkSineAmplitude ;

        shadow.transform.localScale = initialShadowScale * (1 + wave * walkShadowSineAmplitude);
    }

    private void EndDrag()
    {
        shadow.DOFade(baseShadowOpacity, 0.5f);
        rotationTarget = Quaternion.identity;
        body.transform.DOScale(1, 0.5f);
    }

    private void BeginDrag()
    {
        shadow.DOFade(0, 0.5f);
        body.transform.DOScale(1f*pickupScale, 0.5f);
    }

    private void Dragging()
    {
        rotationTarget = Quaternion.Euler(0,0, Mathf.Clamp(-rotationAmplitude * human.moveVelocity.x, -90, 90));
    }
}
