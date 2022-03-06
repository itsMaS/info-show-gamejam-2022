using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] float dragThreshold;

    Interactable currentInteractable;
    InteractableDragTarget currentDragTarget;

    bool dragging = false;
    Vector2 dragStartPoint;

    bool TryRaycastAndFindClosest<T>(Vector2 worldPosition, out T hit) where T : Component, IPivotable
    {
        RaycastHit2D[] Hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);
        List<T> HitInteractables = new List<T>();

        float minDistance = float.MaxValue;
        hit = null;
        foreach (var col in Hits)
        {
            if (col.collider.TryGetComponent(out T hitInteractable))
            {
                float distance = Vector2.Distance(worldPosition, hitInteractable.pickupPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    hit = hitInteractable;
                }
                HitInteractables.Add(hitInteractable);
            }
        }

        if(hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Update()
    {
        if (dragging && !currentInteractable) dragging = false;

        Camera cam = Camera.main;
        Vector2 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);

        TryRaycastAndFindClosest(worldPoint, out Interactable hit);

        if(dragging)
        {
            currentInteractable.dragTarget = cam.ScreenToWorldPoint(Input.mousePosition);
            if(TryRaycastAndFindClosest(worldPoint, out InteractableDragTarget newDragTarget))
            {
                if(currentDragTarget)
                {
                    if(Vector2.Distance(currentDragTarget.transform.position, worldPoint) > Vector2.Distance(newDragTarget.transform.position, worldPoint))
                    {
                        currentDragTarget.InteractableDragUnhover(currentInteractable);
                        currentDragTarget = newDragTarget;
                        newDragTarget.InteractableDragHover(currentInteractable);
                    }
                }
                else
                {
                    currentDragTarget = newDragTarget;
                    currentDragTarget.InteractableDragHover(currentInteractable);
                }
            }
            else
            {
                if(currentDragTarget)
                {
                    currentDragTarget.InteractableDragUnhover(currentInteractable);
                    currentDragTarget = null;
                }
            }

            Debug.DrawLine(currentInteractable.transform.position, currentInteractable.dragTarget);

            if(Input.GetMouseButtonUp(0))
            {
                if(currentDragTarget)
                {
                    currentDragTarget.InteractableDragged(currentInteractable);
                    currentDragTarget.InteractableDragUnhover(currentInteractable);
                    currentDragTarget = null;
                }

                currentInteractable.EndDrag();

                dragging = false;
                currentInteractable = null;
            }
        }
        else if (hit)
        {
            currentInteractable = hit;

            if(Input.GetMouseButtonDown(0))
            {
                dragStartPoint = worldPoint;
                currentInteractable.Click();
            }

            if(Input.GetMouseButton(0))
            {
                if(Vector2.Distance(dragStartPoint, worldPoint) >= dragThreshold)
                {
                    Vector2 cursorPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                    currentInteractable.BeginDrag(cursorPosition);
                    dragging = true;
                }
            }
        }
        else
        {
            currentInteractable = null;
        }
    }
}
