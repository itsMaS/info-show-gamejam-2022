using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] float dragThreshold;

    Interactable current;

    float dragElapsed = 0;
    bool dragging = false;

    void Update()
    {
        Camera cam = Camera.main;
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        
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
        else if (hit.collider != null && hit.collider.TryGetComponent(out Interactable interactable))
        {
            current = interactable;

            if(Input.GetMouseButton(0))
            {
                dragElapsed += Time.deltaTime;
                if(dragElapsed >= dragThreshold)
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
