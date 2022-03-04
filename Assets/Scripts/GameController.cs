using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] float dragThreshold;

    Interactable current;

    bool dragging = false;

    Vector2 dragStartPoint;

    void Update()
    {
        Camera cam = Camera.main;
        Vector2 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] Hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        List<Interactable> HitInteractables = new List<Interactable>();

        float minDistance = float.MaxValue;
        Interactable hit = null;
        foreach (var col in Hits)
        {
            if(col.collider.TryGetComponent(out Interactable hitInteractable))
            {
                float distance = Vector2.Distance(worldPoint, hitInteractable.pickupPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    hit = hitInteractable;
                }
                HitInteractables.Add(hitInteractable);
            }
        }

        if(dragging)
        {
            current.dragTarget = cam.ScreenToWorldPoint(Input.mousePosition);

            Debug.DrawLine(current.transform.position, current.dragTarget);

            if(Input.GetMouseButtonUp(0))
            {
                current.EndDrag();

                dragging = false;
                current = null;
            }
        }
        else if (hit)
        {
            current = hit;

            if(Input.GetMouseButtonDown(0))
            {
                dragStartPoint = worldPoint;
            }

            if(Input.GetMouseButton(0))
            {
                if(Vector2.Distance(dragStartPoint, worldPoint) >= dragThreshold)
                {
                    Vector2 cursorPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                    current.BeginDrag(cursorPosition);
                    dragging = true;
                }
            }

            if(Input.GetMouseButtonDown(0))
            {
                current.Click();
            }
        }
        else
        {
            current = null;
        }
    }
}
