using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool isHolding = false;
    public Interactable itemHeld;

    public List<GameObject> allPlacementPoints = new List<GameObject>();

    public LayerMask interactionLayer; // Set in inspector to only hit interactable objects
    private IHoverable currentHover;
    private IClickable currentClickTarget;

    public TextMeshProUGUI debugText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("POOP START PLAYERINTERACTION");
        UpdatePlacementPoints();
        DisablePlacementPointColliders();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugText != null) // Display debug information
        {
            debugText.text = string.Concat(Input.mousePosition.ToString(), "\n", (currentHover != null));
        }

        HandleHover();

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleRelease();
        }
    }

    private void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        IHoverable hoverTarget = null;

        if (Physics.Raycast(ray, out hit, 100f, interactionLayer))
        {
            hoverTarget = hit.collider.GetComponent<IHoverable>();
            if(hoverTarget == null){
                hoverTarget = hit.collider.GetComponentInParent<IHoverable>();
            }
        }

        if (hoverTarget != currentHover)
        {
            // Notify previous object
            if (currentHover != null)
                currentHover.OnHoverExit();

            // Notify new object
            if (hoverTarget != null)
                hoverTarget.OnHoverEnter();

            currentHover = hoverTarget;
        }
    }
    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactionLayer))
        {
            IClickable clickTarget = hit.collider.GetComponent<IClickable>();
            if (clickTarget != null)
            {
                currentClickTarget = clickTarget;
                clickTarget.OnClick();
            }
        }
    }

    void HandleRelease()
    {
        if (currentClickTarget != null)
        {
            currentClickTarget.OnRelease();
            currentClickTarget = null;
        }
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
            //obj.GetComponentInChildren<MeshFilter>().mesh = null;
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
