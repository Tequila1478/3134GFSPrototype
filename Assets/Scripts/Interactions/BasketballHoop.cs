using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballHoop : MonoBehaviour
{
    public PlacementSpot placementSpot;

    private void OnTriggerEnter(Collider other)
    {
        // HoopIt() if other object is an Interactable.cs
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null) HoopIt(interactable);
    }

    public void PushIt(Interactable interactable)
    {
        /*
         * If not trash: PopIt()
         * If is trash: HoopIt()
        */
    }

    public void PopIt(Interactable interactable)
    {

    }

    public void HoopIt(Interactable interactable)
    {
        if (placementSpot != null) // Only do stuff if this script's placementSpot exists
        {
            if (!interactable.hasSetSpot && !interactable.isAtSetSpot && placementSpot.spotType.ToString() == interactable.taskType) // Only do task type is shared between other object and placementSpot
            {
                Debug.Log("POOP BasketballHoop success triggered by: " + interactable.gameObject);
                interactable.hasSetSpot = true;
                interactable.DropObject(placementSpot); // Pass onto Interactable's DropObject function for object placement behaviour
                return;
            }
        }
        Debug.Log("POOP BasketballHoop fail triggered by: " + interactable.gameObject);
    }
}
