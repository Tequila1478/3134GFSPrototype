using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;

/* FloatState - State used by InteractableStateController when an interactable is selected and moving around the game world.
 * Right click will cancel the float selection, switching to IdleState.
 * Moving the mouse near a placement spot's basketball hoop will trigger the PushedState.
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

        // Reset rayOffset to match current position in scene
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Set up raycast stuff with current mouse position
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, sc.interactionLayer))
        {
            sc.rayOffset = hit.distance; // Update offset to match ray hit distance
        }
    }

    protected override void OnUpdate()
    {
        if (!sc.isInteractive) return; // Cancel if not currently interactive

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Set up raycast stuff with current mouse position
        RaycastHit hit;

        // Move to basketball hoop (if elligible)
        if (!EventSystem.current.IsPointerOverGameObject()) // Do if mouse isn't over UI
        if (Physics.Raycast(ray, out hit, 100f, sc.basketballLayer))
        {
            // Check if this hoop or any of its parents belong to an active PlacementSpot
            PlacementSpot ps = hit.collider.GetComponentInParent<PlacementSpot>();
            if (ps != null && ps.isActive) // Only do if PlacementSpot exists and is inactive
            if (hit.collider.TryGetComponent<BasketballHoop>(out var bbhoop))
            {
                bbhoop.HoopIt(sc); // Run basketball hoop's HoopIt() function to kickstart push state
            }
        }
        // Move to mouse position within world
        if (Physics.Raycast(ray, out hit, 100f, sc.interactionLayer))
        {
            sc.maxRayOffset = hit.distance - sc.minRayOffset; // Update maximum offset to match ray hit distance
            sc.rayOffset = Mathf.Max(sc.rayOffset + Input.mouseScrollDelta.normalized.y, sc.minRayOffset); // Update internal offset based on mouse scroll
            sc.rayVisualOffset = Mathf.Clamp(sc.rayOffset, sc.minRayOffset, sc.maxRayOffset); // Update visible offset
            // Reset internal offset to match visible offset if scrolling
            if (Input.mouseScrollDelta.y < 0)
            {
                sc.rayOffset = sc.rayVisualOffset;
            }
            if (Input.mouseScrollDelta.y > 0 && sc.rayOffset > sc.rayVisualOffset)
            {
                sc.rayOffset = sc.rayVisualOffset;
            }

            // Move to mouse position within world using updated offsets
            Vector3 newPoint = ray.GetPoint(hit.distance - (sc.maxRayOffset + sc.minRayOffset - sc.rayVisualOffset));
            sc.rb.MovePosition
            (
                Vector3.MoveTowards(sc.transform.position, newPoint, sc.followRate * Vector3.Distance(sc.transform.position, newPoint) * Time.deltaTime)
            );
        }
    }
    protected override void OnHurt()
    {
        // Transition to Hurt State
    }
    protected override void OnRightClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Cancel if mouse is over UI
        base.OnRightClick();
    }
    protected override void OnExit()
    {
        // "Must've been the wind"
        sc.ToggleParticles();

        // Update layer
        sc.SetNewLayer(sc.layerWhenUnselected);

        // Deselect item
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
            //CursorScript.instance.UpdateCursor("Default");
        }*/

        base.OnRelease();
    }
}