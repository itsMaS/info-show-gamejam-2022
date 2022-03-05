using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Human : Interactable
{
    public UnityEvent<Genome, Genome> OnMutate;

    GameConfigSO.Human config => State.config.human;

    Genome genome;

    [SerializeField] GeneSO speedGene;

    [SerializeField] float accelerationSpeed;

    Vector2 moveTarget;
    public Vector2 velocity;

    Coroutine movementCoroutine;

    private void Start()
    {
        movementCoroutine = StartCoroutine(Movement());

        genome = new Genome();
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
    IEnumerator Movement()
    {
        moveTarget = transform.position;
        while(true)
        {
            if(Vector2.Distance(transform.position, moveTarget) < 0.01f)
            {
                yield return new WaitForSeconds(config.waitTime.PickRandomFromRange());
                do
                {
                    moveTarget = (Vector2)transform.position + Random.insideUnitCircle.normalized * config.targetDistance.PickRandomFromRange();
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
            genome.TryGetGeneValue(speedGene, out float maxSpeed);
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
