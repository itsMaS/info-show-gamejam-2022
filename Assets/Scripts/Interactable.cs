using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent, SelectionBase]
public class Interactable : MonoBehaviour
{
    public int priority;

    public bool draggable = false;

    public bool positionChangeOverDrag = true;
    public bool usesDragOffset = true;
    public Vector2 dragPointOffset;

    public UnityEvent onClick;
    public UnityEvent onDragBegin;
    public UnityEvent onDragEnd;
    public UnityEvent onDrag;

    public Vector2 dragTarget { get; set; }
    public bool isBeingDragged { get; set; }
    public Vector2 dragPickupOffset { get; set; }


    private Vector2 lastPosition;
    public Vector2 moveVelocity { get; set; }

    public virtual void Click()
    {
        onClick.Invoke();
    }


    protected virtual void Update()
    {
        if(isBeingDragged)
        {
            onDrag.Invoke();
            if (positionChangeOverDrag)
            {
                Vector2 target = usesDragOffset ? dragTarget + dragPickupOffset : dragTarget;
                transform.position = Vector2.Lerp(transform.position, target - dragPointOffset, 20 * Time.deltaTime);

            }
        }

        moveVelocity = (Vector2)transform.position - lastPosition;
        lastPosition = transform.position;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawSphere((Vector2)transform.position + dragPointOffset, 0.05f);
    }

    public void BeginDrag(Vector2 cursorPosition)
    {
        isBeingDragged = true;
        dragPickupOffset = (Vector2)transform.position - cursorPosition;
        onDragBegin.Invoke();
    }
    public void EndDrag()
    {
        isBeingDragged = false;
        onDragEnd.Invoke();
    }
}
