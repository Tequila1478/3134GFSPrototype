using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Update particles
        sc.ToggleParticles();
    }

    protected override void OnUpdate()
    {
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
    protected override void OnExit()
    {
        // "Must've been the wind"
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
        if (sc.ps != null) sc.ps.claimed = false;

        sc.transform.SetParent(null, true);
        if (sc.moveCoroutine != null)
        {
            sc.StopCoroutine(sc.moveCoroutine); // Prevent object from continuing to move towards a placement spot
            sc.moveCoroutine = null;         // Forget placement movement
            sc.movingToSetSpot = false;
            sc.isAtSetSpot = false;
            sc.coroutineFinished = false;
        }

        if (!sc.floating && sc.playerInteraction.itemHeld == null)
        {
            sc.ChangeState(sc.floatState);
            sc.sfx_AM.PlaySFX(sc.pickUp);
            sc.ps = null;
        }

        Debug.Log("Clicked: " + this);

        base.OnClick();
    }
    public override void OnRelease()
    {
        // Do nothing
    }
}