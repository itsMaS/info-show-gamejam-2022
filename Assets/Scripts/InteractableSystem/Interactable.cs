using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent, SelectionBase]
public class Interactable : MonoBehaviour, IPivotable
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

    public Vector2 pickupPosition => Vector2.Scale(dragPointOffset, transform.localScale);

    Collider2D col;

    public virtual void Awake()
    {
        col = GetComponent<Collider2D>();
    }

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
                transform.position = Vector2.Lerp(transform.position, dragTarget - pickupPosition, 20 * Time.deltaTime);

            }
        }

        moveVelocity = (Vector2)transform.position - lastPosition;
        lastPosition = transform.position;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawSphere((Vector2)transform.position + pickupPosition, 0.05f);
    }

    public virtual void BeginDrag(Vector2 cursorPosition)
    {
        col.enabled = false;

        isBeingDragged = true;
        dragPickupOffset = (Vector2)transform.position - cursorPosition;
        onDragBegin.Invoke();

    }
    public virtual void EndDrag()
    {
        col.enabled = true;

        isBeingDragged = false;
        onDragEnd.Invoke();
    }
}
