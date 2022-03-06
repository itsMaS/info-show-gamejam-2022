using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Human))]
public class HumanVisual : MonoBehaviour
{
    [Header("Dependancies")]
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] SpriteRenderer body;
    [SerializeField] ParticleSystem loveParticles;

    [Header("Parameters")]
    [SerializeField] float rotationAmplitude;
    [SerializeField] float rotationAccelerationSpeed;
    [SerializeField] float pickupScale;
    [SerializeField] float walkSineSpeed;
    [SerializeField] float walkSineAmplitude;
    [SerializeField] float walkShadowSineAmplitude;
    [SerializeField] AnimationCurve deathOverAge;

    Human human;
    Animation anim;

    float baseShadowOpacity;
    Quaternion rotationTarget;
    Vector2 initialLocalBodyPosition;
    Vector2 initialShadowScale;

    SortingGroup sg;

    private void Awake()
    {
        human = GetComponent<Human>();
        anim = GetComponent<Animation>();

        baseShadowOpacity = shadow.color.a;
        initialLocalBodyPosition = body.transform.localPosition;
        initialShadowScale = shadow.transform.localScale;

        sg = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        human.onDrag.AddListener(Dragging);
        human.onDragBegin.AddListener(BeginDrag);
        human.onDragEnd.AddListener(EndDrag);
        human.OnMutate.AddListener(Mutate);
        human.onBreedHover.AddListener(BreedHover);
        human.onBreedUnhover.AddListener(BreedUnhover);
        human.OnDeath.AddListener(Death);
    }

    private void Death()
    {
        body.material.DOFloat(1, "_death", human.deathDuration);
        shadow.DOFade(0, human.deathDuration);
    }

    private void BreedUnhover(Human arg0, Human arg1)
    {
        loveParticles.Stop();
    }

    private void BreedHover(Human arg0, Human arg1)
    {
        if(arg0.canMate && arg1.canMate && Genome.Relation(arg0.genome,arg1.genome) <= State.config.human.relationDifferenceRequiredForMating)
        {
            loveParticles.Play();
        }
    }

    private void Mutate(Genome oldGenome, Genome newGenome)
    {
        anim.Play("Mutate");
    }
  private void Update()
    {
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, rotationTarget, rotationAccelerationSpeed);

        float wave = (1 + Mathf.Sin(Time.time * walkSineSpeed)) * human.velocity.magnitude;
        body.transform.localPosition = initialLocalBodyPosition + Vector2.up * wave * walkSineAmplitude;

        shadow.transform.localScale = initialShadowScale * (1 + wave * walkShadowSineAmplitude);

        if(!human.dead)
            body.material.SetFloat("_death", deathOverAge.Evaluate(human.Age));
    }

    private void EndDrag()
    {
        shadow.DOFade(baseShadowOpacity, 0.5f);
        rotationTarget = Quaternion.identity;
        body.transform.DOScale(1, 0.5f);

        if (!human.dead)
            sg.sortingOrder = 0;
    }

    private void BeginDrag()
    {
        shadow.DOFade(0, 0.5f);
        body.transform.DOScale(1f*pickupScale, 0.5f);

        sg.sortingOrder = 999;
    }

    private void Dragging()
    {
        rotationTarget = Quaternion.Euler(0,0, Mathf.Clamp((-rotationAmplitude * human.moveVelocity.x)/Time.deltaTime, -90, 90));
    }
}
