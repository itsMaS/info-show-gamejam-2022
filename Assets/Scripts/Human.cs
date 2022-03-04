using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Mutable
{
    GameConfigSO.Human config => State.config.human;

    public class Gene
    {
        public string name;
        public string description;
    }

    public class Genome
    {
        List<Gene> Genes = new List<Gene>();
    }

    [SerializeField] float maxSpeed;
    [SerializeField] float accelerationSpeed;

    Vector2 moveTarget;
    public Vector2 velocity;

    Coroutine movementCoroutine;

    private void Start()
    {
        movementCoroutine = StartCoroutine(Movement());
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
            transform.position = Vector2.SmoothDamp(transform.position, moveTarget, ref velocity, 1/config.accelerationSpeed, config.maxSpeed);
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
