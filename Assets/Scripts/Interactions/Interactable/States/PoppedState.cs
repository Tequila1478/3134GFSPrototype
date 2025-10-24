using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* PoppedState - State used by InteractableStateController when an interactable has been placed.
 * Uses some logic from IdleState so that the interactable can be picked up again.
 */
public class PoppedState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in PoppedState");

        // Teleport to placement spot
        // Target location and rotation
        Vector3 offsetSocketPoint = sc.ps.offsetSocket.position;
        Quaternion offsetSocketAngle = sc.ps.offsetSocket.rotation;

        //Offset location and rotation
        Vector3 offsetPlugPoint = sc.offsetPlug.localPosition;
        Quaternion offsetPlugAngle = sc.offsetPlug.localRotation;

        //Final position
        Quaternion newAngle = offsetSocketAngle * Quaternion.Inverse(offsetPlugAngle);
        Vector3 newPoint = offsetSocketPoint - newAngle * offsetPlugPoint;

        sc.transform.rotation = newAngle;
        sc.transform.position = newPoint;

        


        // Update physics
        sc.rb.useGravity = false;
        sc.rb.drag = 0;
        sc.rb.isKinematic = false;
        sc.SetCollidersAsTrigger(true);

        // Update particles
        sc.ToggleParticles();

        // Update held item
        if (sc.playerInteraction.itemHeld == sc)
        {
            sc.playerInteraction.itemHeld = null;
        }

        // Update layer
        sc.SetNewLayer(sc.layerWhenUnselected);

        // Set parent
        sc.transform.SetParent(sc.ps.transform, true);

        // Yield interactivity when first entering this state
        sc.StartCoroutine(sc.HaltInteractions(0.1f));

        sc.sfx_AM?.PlaySFX(sc.putDown);
        sc.ToggleParticles("PLACE");
    }

    protected override void OnUpdate()
    {
        // Search for player
    }
    protected override void OnHurt()
    {
        // Transition to Hurt State
    }
    protected override void OnRightClick()
    {
        // Do nothing
    }
    protected override void OnExit()
    {
        // "Must've been the wind"
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
        if (!sc.isInteractive) return; // Cancel if not currently interactive 
        if (EventSystem.current.IsPointerOverGameObject()) return; // Cancel if mouse is over UI

        if (sc.ps != null) sc.ps.claimed = false; // Unclaim placement spot

        sc.transform.SetParent(null, true);

        if (sc.playerInteraction.itemHeld == null) // Check if an item isn't being held
        {
            sc.ChangeState(sc.floatState); // Change to float state
            sc.sfx_AM.PlaySFX(sc.pickUp); // Play pick up sound
            sc.ps = null;
        }

        Debug.Log("Clicked: " + this);

        base.OnClick();
    }
}