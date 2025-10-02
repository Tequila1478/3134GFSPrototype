using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballHoop : MonoBehaviour
{
    public PlacementSpot placementSpot;

    private void OnTriggerEnter(Collider other)
    {
        // HoopIt() if other object is an InteractableStateController.cs
        /*InteractableStateController interactableSC = other.GetComponent<InteractableStateController>();
        if (interactableSC != null) HoopIt(interactableSC);*/
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
                interactable.ps = placementSpot;
                interactable.DropObject(interactable.ps); // Pass onto Interactable's DropObject function for object placement behaviour
                return;
            }
        }
        Debug.Log("POOP BasketballHoop fail triggered by: " + interactable.gameObject);
    }
}
