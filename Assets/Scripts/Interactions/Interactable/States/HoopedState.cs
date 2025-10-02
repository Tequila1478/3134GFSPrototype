using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopedState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in HoopedState");

        // Update physics
        sc.rb.useGravity = true;
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
}