using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
    }

    protected override void OnUpdate()
    {
        // Search for player
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