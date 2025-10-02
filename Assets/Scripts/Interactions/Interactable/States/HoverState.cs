using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in HoverState");

        sc.rb.useGravity = false;
        sc.rb.drag = 4;
        sc.rb.isKinematic = false;
    }

    protected override void OnUpdate()
    {
        if(Input.GetMouseButtonDown(1))
        {
            sc.ChangeState(sc.idleState);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Move to basketball hoop (if elligible)
        if (Physics.Raycast(ray, out hit, 100f, sc.basketballLayer))
        {
            if (hit.collider.TryGetComponent<BasketballHoop>(out var bbhoop))
            {
                bbhoop.HoopIt(sc.GetComponent<Interactable>());
            }
        }
        // Move to mouse position within world
        else if (Physics.Raycast(ray, out hit, 100f, sc.interactionLayer))
        {
            sc.maxRayOffset = hit.distance - sc.minRayOffset; // Update maximum offset to match ray hit distance
            sc.rayOffset = Mathf.Max(sc.rayOffset + Input.mouseScrollDelta.normalized.y, sc.minRayOffset);
            sc.rayVisualOffset = Mathf.Clamp(sc.rayOffset, sc.minRayOffset, sc.maxRayOffset);
            if (Input.mouseScrollDelta.y < 0)
            {
                sc.rayOffset = sc.rayVisualOffset;
            }
            if (Input.mouseScrollDelta.y > 0 && sc.rayOffset > sc.rayVisualOffset)
            {
                sc.rayOffset = sc.rayVisualOffset;
            }

            Vector3 newPoint = ray.GetPoint(hit.distance - (sc.maxRayOffset - sc.rayVisualOffset));
            sc.transform.position = Vector3.MoveTowards(sc.transform.position, newPoint, sc.followRate * Vector3.Distance(sc.transform.position, newPoint));
        }
    }
    protected override void OnHurt()
    {
        // Transition to Hurt State
    }
    protected override void OnExit()
    {
        // "Must've been the wind"
    }
}