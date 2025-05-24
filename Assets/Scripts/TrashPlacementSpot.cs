using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPlacementSpot : PlacementSpot, IHoverable, IClickable
{
    protected override void Update()
    {
        if (otherObject == null) return;

        var interactable = otherObject.GetComponent<Interactable>();
        if (interactable == null) return;

        if (withinRange && !interactable.movingToSetSpot)
        {
            startingPosition = otherObject.transform.position;
            CreateBezierCurvePoints(interactable);
            interactable.routes = controlPoints;
        }

        if (interactable.movingToSetSpot && interactable.moveComplete)
        {
            // Apply gravity and drop
            var rb = interactable.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            // Allow reuse by not claiming
            placementVisualisation.GetComponent<MeshFilter>().mesh = null;
            otherObject = null;
            withinRange = false;
        }
    }

    public void OnClick()
    {
        if (player.itemHeld == null)
            return;

        if (player.itemHeld.hasSetSpot)
        {
            player.itemHeld.moveComplete = false;
            player.itemHeld.floating = false;
            player.isHolding = false;

            player.itemHeld.StartMoveToSetSpot();
            player.itemHeld = null;
        }
        else
        {
            // Free drop
            player.itemHeld.floating = false;
            player.itemHeld.moveComplete = true;
            var rb = player.itemHeld.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            player.itemHeld = null;
            player.isHolding = false;

            Debug.Log("Dropped freely into trash.");
        }

        Invoke(nameof(DisablePlacementPointCollidersSafely), 0.1f);
    }

    public void OnHoverExit()
    {
        DeselectObject();
        withinRange = false;
        cursor.ChangeVisual(0);
    }

    // Prevent claiming logic from parent
    public void SelectObject(Collider other = null)
    {
        if (other != null && other.CompareTag("Held Item"))
        {
            var interactable = other.GetComponent<Interactable>();
            if (interactable == null) return;

            otherObject = other;
            withinRange = true;
            interactable.hasSetSpot = true;
            interactable.newDirection = direction;
            ApplyVisualisation(interactable.visualisationObj, interactable);
        }
        else if (player.isHolding)
        {
            otherObject = player.itemHeld.GetComponent<Collider>();
            var interactable = otherObject?.GetComponent<Interactable>();
            if (interactable == null) return;

            withinRange = true;
            interactable.hasSetSpot = true;
            interactable.newDirection = direction;
            ApplyVisualisation(interactable.visualisationObj, interactable);
        }
    }

    protected override void DeselectObject(Collider other = null)
    {
        //base.DeselectObject(other);
        if (other != null)
        {
            var interactable = other.GetComponent<Interactable>();
            if (interactable != null)
                interactable.hasSetSpot = false;

            if (other == otherObject)
            {
                otherObject = null;
                withinRange = false;
                placementVisualisation.GetComponent<MeshFilter>().mesh = null;
            }
        }
        else if (otherObject != null)
        {
            var interactable = otherObject.GetComponent<Interactable>();
            if (interactable != null)
                interactable.hasSetSpot = false;

            otherObject = null;
            withinRange = false;
            placementVisualisation.GetComponent<MeshFilter>().mesh = null;
        }
    }
}
