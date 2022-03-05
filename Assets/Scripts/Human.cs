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

    GameConfigSO.Human config => State.config.human;

    public Genome genome { get; private set;}

    [SerializeField] GeneSO speedGene;
    [SerializeField] GeneSO fertilityGene;

    [SerializeField] float accelerationSpeed;

    Vector2 moveTarget;
    public Vector2 velocity;

    Coroutine movementCoroutine;

    public InteractableDragTarget target { get; private set; }

    public override void Awake()
    {
        base.Awake();

        target = GetComponent<InteractableDragTarget>();
    }
    private void Start()
    {
        movementCoroutine = StartCoroutine(Movement());

        genome = new Genome();

        target.onInteractableDraggedOn.AddListener(Breed);
    }

    public void Spawn(Genome genome)
    {
        this.genome = genome;
        OnSpawn.Invoke(genome);
    }

    private void Breed(Interactable interactable)
    {
        Human human = (Human)interactable;
        Destroy(human.gameObject);
        Destroy(gameObject);

        if(genome.TryGetGeneValue(fertilityGene, out float fertility))
        {
            int offspring = Mathf.RoundToInt(config.offspringOverFertility.Evaluate(fertility));
            for (int i = 0; i < offspring; i++)
            {
                State.Instance.SpawnHuman(transform.position, new Genome(genome, human.genome));
            }
        }


        OnBreed.Invoke(this, human);
    }

    public virtual void Mutate(float radioctivity)
    {
        Genome oldGenome = genome;
        Genome newGenome = new Genome(oldGenome, radioctivity);
        OnMutate.Invoke(oldGenome, newGenome);

        genome = newGenome;
    }
    public override void EndDrag()
    {
        base.EndDrag();

        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(Movement());
    }
    public override void BeginDrag(Vector2 cursorPosition)
    {
        base.BeginDrag(cursorPosition);
        velocity = Vector3.zero;
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
        
        if(!isBeingDragged)
        {
            genome.TryGetGeneValue(speedGene, out float speedGeneValue);
            float maxSpeed = config.walkSpeedOverGene.Evaluate(speedGeneValue);

            transform.position = Vector2.SmoothDamp(transform.position, moveTarget, ref velocity, 1/config.accelerationSpeed, maxSpeed);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, config.targetDistance.x);
        Gizmos.DrawWireSphere(transform.position, config.targetDistance.y);
    }
}
