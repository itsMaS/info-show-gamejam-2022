using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractableDragTarget : MonoBehaviour, IPivotable
{
    public UnityEvent<Interactable> onInteractableDraggedOn;
    public UnityEvent<Interactable> onInteractableDragHover;
    public UnityEvent<Interactable> onInteractableDragUnhover;

    public Vector2 dragPointOffset;
    public Vector2 pickupPosition => dragPointOffset;

    public virtual void InteractableDragged(Interactable interactable)
    {
        //Debug.Log($"{interactable.gameObject.name} dragged on {gameObject.name}");
        onInteractableDraggedOn.Invoke(interactable);
    }
    public virtual void InteractableDragHover(Interactable interactable)
    {
        //Debug.Log($"{interactable.gameObject.name} hover drag on {gameObject.name}");
        onInteractableDragHover.Invoke(interactable);
    }
    public virtual void InteractableDragUnhover(Interactable interactable)
    {
        //Debug.Log($"{interactable.gameObject.name} unhover drag on {gameObject.name}");
        onInteractableDragUnhover.Invoke(interactable);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawSphere((Vector2)transform.position + dragPointOffset, 0.05f);
    }
}
