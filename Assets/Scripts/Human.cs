using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableDragTarget))]
public class Human : Interactable
{
    public UnityEvent<Genome, Genome> OnMutate;
    public UnityEvent<Human, Human> OnBreed;
    public UnityEvent<Genome> OnSpawn;
    public UnityEvent<Human, Human> onBreedHover;
    public UnityEvent<Human, Human> onBreedUnhover;
    public UnityEvent OnDeath;

    GameConfigSO.Human config => State.config.human;

    public Genome genome { get; private set; }

    [SerializeField] GeneSO speedGene;
    [SerializeField] GeneSO fertilityGene;
    [SerializeField] GeneSO strengthGene;
   

    [SerializeField] Collider2D shadowCollider;
    [SerializeField] float accelerationSpeed;
    public float deathDuration;

    float Size
    {
        get
        {
            genome.TryGetGeneValue(strengthGene, out float strength);
            float size = config.sizeOverAge.Evaluate(Age)*config.sizeOverStrength.Evaluate(strength);

            return size;
        }
    }
    public float Age { get; private set; }

    Vector2 moveTarget;
    
    public Vector2 velocity;

    Coroutine movementCoroutine;

    public InteractableDragTarget target { get; private set; }

    public bool canMate => oldEnoughToMate && matingCooldown <= 0;
    public bool oldEnoughToMate => Age > config.ageRequiredToMate;

    public float matingCooldown { get; private set; }
    public float ageingSpeed => 1 / config.baseLifespan;

    public bool dead = false;

    public override void Awake()
    {
        base.Awake();

        target = GetComponent<InteractableDragTarget>();
    }
    private void Start()
    {
        movementCoroutine = StartCoroutine(Movement());
        target.onInteractableDraggedOn.AddListener(DraggedOn);

        target.onInteractableDragHover.AddListener(DragHover);
        target.onInteractableDragUnhover.AddListener(DragUnhover);
    }

    private void DraggedOn(Interactable arg0)
    {
        if (arg0 is Human) Breed((Human)arg0);
    }

    public void Spawn(Genome genome, float age)
    {
        this.Age = age;
        this.genome = genome;
        OnSpawn.Invoke(genome);
    }

    public void DragHover(Interactable interactable)
    {
        if(interactable is Human)
        {
            Human human = (Human)interactable;

            if(!human.dead)
            {
                onBreedHover.Invoke(this, human);
            }
        }
    }
    public void DragUnhover(Interactable interactable)
    {
        if (interactable is Human)
        {
            onBreedUnhover.Invoke(this, (Human)interactable);
        }
    }

    public int OffspringAmount(Human other)
    {
        genome.TryGetGeneValue(fertilityGene, out float f1);
        other.genome.TryGetGeneValue(fertilityGene, out float f2);
        float fertility = Mathf.Lerp(f1, f2, 0.5f);
        return Mathf.RoundToInt(config.offspringOverFertility.Evaluate(fertility));
    }

    private void Breed(Human human)
    {
        if(!canMate || !human.canMate)
        {
            return;
        }

        if(Genome.Relation(human.genome, genome) > config.relationDifferenceRequiredForMating)
        {
            return;
        }


        int offspring = OffspringAmount(human);
        for (int i = 0; i < offspring; i++)
        {
            State.Instance.SpawnHuman(transform.position, new Genome(genome, human.genome));
        }

        human.MatedWith();
        matingCooldown = config.matingCooldown;
        OnBreed.Invoke(this, human);
    }

    private void MatedWith()
    {
        matingCooldown = config.matingCooldown;
    }

    public virtual void Die()
    {
        dead = true;
        OnDeath.Invoke();
        Destroy(gameObject, deathDuration);
        StopAllCoroutines();
        moveTarget = transform.position;
    }

    public virtual void Mutate(float radioctivity)
    {
        Genome oldGenome = genome;
        Genome newGenome = new Genome(oldGenome, radioctivity);
        OnMutate.Invoke(oldGenome, newGenome);

        genome = newGenome;

        Age += config.lifeTimeLossPerRadiation;
    }
    public override void EndDrag()
    {
        base.EndDrag();

        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(Movement());

        shadowCollider.enabled = true;
    }
    public override void BeginDrag(Vector2 cursorPosition)
    {
        base.BeginDrag(cursorPosition);
        velocity = Vector3.zero;

        shadowCollider.enabled = false;
    }
    IEnumerator Movement()
    {
        moveTarget = transform.position;
        while(true)
        {
            if(Vector2.Distance(transform.position, moveTarget) < 0.01f)
            {
                yield return new WaitForSeconds(config.waitTime.PickRandom());
                do
                {
                    moveTarget = (Vector2)transform.position + Random.insideUnitCircle.normalized * config.targetDistance.PickRandom();
                    Debug.DrawLine(transform.position, (Vector2)transform.position+moveTarget, Color.yellow);
                    yield return null;
                } while (!State.Instance.PointWithinBounds(moveTarget));
            }
            yield return null;
        }
    }

    protected override void Update()
    {
        base.Update();
        Ageing();

        matingCooldown -= Time.deltaTime;

        if (!isBeingDragged)
        {
            genome.TryGetGeneValue(speedGene, out float speedGeneValue);
            float maxSpeed = config.walkSpeedOverGene.Evaluate(speedGeneValue);

            transform.position = Vector2.SmoothDamp(transform.position, moveTarget, ref velocity, 1/config.accelerationSpeed, maxSpeed);
        }
    }

    private void Ageing()
    {
        Age += Time.deltaTime * ageingSpeed;
        transform.localScale = Vector3.one * Size;

        if (Age >= 1) Die();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
