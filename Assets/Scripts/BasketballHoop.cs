using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballHoop : MonoBehaviour
{
    public PlacementSpot placementSpot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null && placementSpot != null) // Only do if other object has Interactable.cs, and same task type as placementSpot
        {
            if (!interactable.hasSetSpot && !interactable.isAtSetSpot && placementSpot.spotType.ToString() == interactable.taskType)
            {
                Debug.Log("POOP BasketballHoop success triggered by: " + other.gameObject);
                interactable.hasSetSpot = true;
                interactable.DropObject(placementSpot); // Pass onto Interactable's DropObject function
                return;
            }
        }
        Debug.Log("POOP BasketballHoop fail triggered by: " + other.gameObject);
    }
}
