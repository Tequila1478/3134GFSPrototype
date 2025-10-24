using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* IdleState - The default state used by InteractableStateController.
 * In this state, the interactable uses gravity and isn't controlled by the player.
 */
public class IdleState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in IdleState");

        // Update physics
        sc.rb.useGravity = true;
        sc.rb.drag = 0;
        sc.rb.isKinematic = false;
        sc.SetCollidersAsTrigger(false);

        // Update particles
        sc.ToggleParticles();

        if (sc.pg)
            sc.pg.AddActionAsListener(UnhighlightObject, nameof(UnhighlightObject)); // Set up pause listener
        else Debug.LogError("WHere the fook is the sc.pg");
    }

    protected override void OnUpdate()
    {
        if (!sc.isInteractive) return; // Cancel if not currently interactive 

        // Search for player
        if (sc.isHovered)
        {
            sc.SetNewLayer(sc.layerWhenHovered);
        }
        else
        {
            sc.SetNewLayer(sc.layerWhenUnselected);
        }
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
        Debug.Log("Hi POOP OnExit here");
        sc.pg.RemoveActionAsListener(UnhighlightObject, nameof(UnhighlightObject)); // Set up pause listener
        OnHoverExit();
    }
    public override void OnHoverEnter()
    {
        sc.ToggleParticles("HOVER");
        base.OnHoverEnter();
    }
    public override void OnHoverExit()
    {
        sc.ToggleParticles();
        base.OnHoverExit();
    }
    public override void OnClick()
    {
        if (!sc.isInteractive) return; // Cancel if not currently interactive
        if (EventSystem.current.IsPointerOverGameObject()) return; // Cancel if mouse is over UI

        if (sc.ps != null) sc.ps.claimed = false;

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
    public override void OnRelease()
    {
        //
    }
}