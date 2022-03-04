using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Person))]
public class PersonVisual : MonoBehaviour
{
    [Header("Dependancies")]
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] SpriteRenderer body;

    [Header("Parameters")]
    [SerializeField] float rotationAmplitude;
    [SerializeField] float rotationAccelerationSpeed;


    Person person;

    float baseShadowOpacity;
    Quaternion rotationTarget;

    private void Awake()
    {
        person = GetComponent<Person>();

        baseShadowOpacity = shadow.color.a;
    }

    private void Start()
    {
        person.onDrag.AddListener(Dragging);
        person.onDragBegin.AddListener(BeginDrag);
        person.onDragEnd.AddListener(EndDrag);
    }

    private void Update()
    {
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, rotationTarget, Time.deltaTime * rotationAccelerationSpeed);
    }

    private void EndDrag()
    {
        shadow.DOFade(baseShadowOpacity, 0.5f);
        rotationTarget = Quaternion.identity;
    }

    private void BeginDrag()
    {
        shadow.DOFade(0, 0.5f);
    }

    private void Dragging()
    {
        rotationTarget = Quaternion.Euler(0,0, Mathf.Clamp(-rotationAmplitude * person.moveVelocity.x, -90, 90));
    }
}
