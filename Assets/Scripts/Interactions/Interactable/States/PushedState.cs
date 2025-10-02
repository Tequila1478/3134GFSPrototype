using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;

public class PushedState : State
{
    protected override void OnEnter()
    {
        // "What was that!?"
        Debug.Log("Ran OnEnter in PushedState");

        // Update physics
        sc.rb.useGravity = false;
        sc.rb.drag = 4;
        sc.rb.isKinematic = true;
        sc.SetCollidersAsTrigger(true);

        // Update particles
        sc.ToggleParticles("FLOAT");

        // Update held item
        if (sc.playerInteraction.itemHeld != sc)
        {
            sc.playerInteraction.itemHeld = sc;
        }

        // Update layer
        sc.SetNewLayer(sc.layerWhenSelected);


        StartMoveToSetSpot(sc.ps);
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
        } else // If illegible, go back to flaoting
        {
            sc.ChangeState(sc.floatState);
        }

        // Move to target placement spot
        Vector3 newPoint = sc.ps.transform.position;
        Quaternion newAngle = sc.ps.transform.rotation;
        sc.transform.position = Vector3.MoveTowards(sc.transform.position, newPoint, sc.followRate * Vector3.Distance(sc.transform.position, newPoint));
        sc.transform.rotation = Quaternion.RotateTowards(sc.transform.rotation, newAngle, 10);
        
    }
    protected override void OnHurt()
    {
        // Transition to Hurt State
    }
    protected override void OnRightClick() // Instead of dropping object, activate left-click behaviour
    {
        OnLeftClick();
    }
    protected override void OnLeftClick() // In PushedState, left-click will confirm place the interactable in its current placement spot.
    {
        if (!sc.isInteractive) return; // Cancel if not currently interactive 

        sc.ps.claimed = true; // Set placement spot as claimed

        sc.playerInteraction.DisablePlacementPointColliders(); // Disable placement point colliders

        sc.ChangeState(sc.poppedState); // Change state to "popped"

        base.OnLeftClick();
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

    public bool StartMoveToSetSpot(PlacementSpot placementSpot, bool forceMove = false) // Will return bool of whether object has started moving to set spot
    {
        if (!sc.isInteractive) return false; // Cancel if not currently interactive 

        Debug.Log("Started StartMoveToSetSpot");

        sc.ps = placementSpot;
        //sc.oi?.ClearPlacementSpots();

        //if (!sc.hasSetSpot && !forceMove) return false;

        //sc.moveCoroutine = sc.StartCoroutine(MoveDirectlyToSpot(sc.ps.transform.position));
        sc.movingToSetSpot = true;
        sc.sfx_AM?.PlaySFX(sc.putDown);
        //sc.ToggleParticles();

        return true;

    }

    /*private IEnumerator MoveDirectlyToSpot(Vector3 targetPos)
    {
        // Ensure rigidbody doesn't interfere
        sc.rb.useGravity = false;
        sc.rb.isKinematic = true;

        while (Vector3.Distance(sc.transform.position, targetPos) > 0.05f) // threshold
        {
            sc.transform.position = Vector3.MoveTowards(
                sc.transform.position,
                targetPos,
                sc.speed * Time.deltaTime
            );
            yield return null;
        }

        // Snap to final position
        sc.transform.position = targetPos;

        // If the placement spot wants alignment, also set rotation
        if (sc.ps != null)
        {
            sc.transform.rotation = Quaternion.LookRotation(sc.ps.direction);

            //sc.transform.SetParent(sc.ps.transform, true);
        }

        // Mark as complete
        sc.movingToSetSpot = false;
        sc.moveComplete = true;
        sc.isAtSetSpot = true;
        sc.coroutineFinished = true;

        sc.moveCoroutine = null;
    }*/
}