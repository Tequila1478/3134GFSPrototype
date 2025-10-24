using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballHoop : MonoBehaviour
{
    public PlacementSpot placementSpot;

    private void OnTriggerEnter(Collider other)
    {
        // This code is for auto-dunking idle trash into a bin
        var isc = other.GetComponent<InteractableStateController>();
        if (isc && placementSpot) // Do if triggering object is interactable and this hoop's placement spot still exists
        {
            if
            (
                placementSpot.isTrashcan // Check if placement spot is for dunking
                && isc.taskType == placementSpot.spotType.ToString() // Check if task types match
                && isc.currentState == isc.idleState // Check if interactable is idle
            )
            {
                isc.ChangeState(isc.dunkedState); // Start dunk state for interactable
            }
        }
    }

    public void HoopIt(InteractableStateController interactable)
    {
        if (placementSpot != null) // Only do stuff if this script's placementSpot exists
        {
            if 
            (
                placementSpot.spotType.ToString() == interactable.taskType // Only do if task types match between interactable and placementSpot
                && interactable.ps != placementSpot // Only do if interactable isn't already at placement spot
                && !placementSpot.claimed // Only do if placement spot isn't already claimed
            )
            {
                Debug.Log("POOP BasketballHoop success triggered by: " + interactable.gameObject);
                interactable.PushObject(placementSpot); // Pass onto Interactable's DropObject function for object placement behaviour
                return;
            }
        }
        Debug.Log("POOP BasketballHoop fail triggered by: " + interactable.gameObject);
    }
}
