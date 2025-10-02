using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* FloatState - State used by InteractableStateController when an interactable is selected and moving around the game world.
 * Right click will cancel the float selection, switching to IdleState.
 * Moving the mouse near a placement spot will trigger the PushedState.
 */
public class FloatState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in HoverState");

        // Update physics
        sc.rb.useGravity = false;
        sc.rb.drag = 4;
        sc.rb.isKinematic = false;
        sc.SetCollidersAsTrigger(false);

        // Cancel movement lock-ons
        sc.ps = null;

        // Update particles
        sc.ToggleParticles("FLOAT");

        // Update held item
        sc.playerInteraction.isHolding = true;
        sc.playerInteraction.itemHeld = sc.GetComponent<InteractableStateController>();
        sc.playerInteraction.EnablePlacementPointColliders();
        sc.tag = "Held Item";

        // Update layer
        sc.SetNewLayer(sc.layerWhenSelected);
    }

    protected override void OnUpdate()
    {
        if (!sc.isInteractive) return; // Cancel if not currently interactive 

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Move to basketball hoop (if elligible)
        if (Physics.Raycast(ray, out hit, 100f, sc.basketballLayer))
        {
            if (hit.collider.TryGetComponent<BasketballHoop>(out var bbhoop))
            {
                bbhoop.HoopIt(sc.GetComponent<InteractableStateController>());
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
    protected override void OnRightClick()
    {
        base.OnRightClick();
    }
    protected override void OnExit()
    {
        // "Must've been the wind"
        sc.ToggleParticles();

        sc.SetNewLayer(sc.layerWhenUnselected);

        if (sc.playerInteraction.itemHeld == sc)
        {
            sc.playerInteraction.itemHeld = null;
        }
    }

    public override void OnHoverEnter()
    {
        // Do nothing
    }
    public override void OnHoverExit()
    {
        // Do nothing
    }
    public override void OnClick()
    {
        // Do nothing
    }
    public override void OnRelease()
    {
        /*if (sc.moveComplete)
        {
            sc.ChangeState(sc.idleState);
        }
        else
        {
            sc.moveComplete = true;
            //cursor?.ChangeVisual(0);
            CursorScript.instance.UpdateCursor("Default");
        }*/

        base.OnRelease();
    }
}