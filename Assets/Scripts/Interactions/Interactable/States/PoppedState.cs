using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppedState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
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