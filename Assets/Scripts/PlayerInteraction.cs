using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool isHolding = false;
    public Interactable itemHeld;

    public List<GameObject> allPlacementPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        UpdatePlacementPoints();
        DisablePlacementPointColliders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlacementPoints()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("PlacementPoint");
        foreach (GameObject obj in objs)
        {
            if(obj.GetComponent<PlacementSpot>() != null)
            {
                if (!obj.GetComponent<PlacementSpot>().claimed)
                {
                    allPlacementPoints.Add(obj);
                }
            }
        }
    }

    public void UpdateAllPlacementPointClaimExcept(GameObject ps = null)
    {
        foreach (GameObject obj in allPlacementPoints)
        {
            if (ps != obj)
            {
                obj.GetComponent<PlacementSpot>().claimed = false;
            }
        }
    }

    public void DisablePlacementPointColliders()
    {
        foreach (GameObject obj in allPlacementPoints)
        {
            obj.GetComponent<Collider>().enabled = false;
        }
    }

    public void EnablePlacementPointColliders()
    {
        foreach (GameObject obj in allPlacementPoints)
        {
            obj.GetComponent<Collider>().enabled = true;
        }
    }
}
