using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpotType
{
    Book,
    Pillow,
    Trash,
    Any,  //for spots that accept any type
    Stationary,
    Shoe,
    Cup,
}


public class PlacementSpot : MonoBehaviour, IHoverable, IClickable
{
    [Tooltip("Setting isTrashcan to true will cause any object placed here to shrink and be destroyed.")]
    public bool isTrashcan = false;
    public bool isActive = true;

    [Header("PLacement Settings")]
    public float maxHeightAbovePoint;
    public GameObject selectionHighlight;
    public Vector3 placementRescale = new Vector3 (1f, 1f, 1f);
    public Vector3 highlightRescale = new Vector3 (1f, 1f, 1f);
    public PlayerInteraction player;

    public bool claimed = false;

    [Header("Spot Type")]
    public SpotType spotType = SpotType.Any;

    [Header("placement Offset Settings")]
    public Transform offsetSocket;
    private Vector3 direction;
    public Vector3 placementOffset;

    [Header("Other Settings")]
    public Collider otherObject;
    public int numOfTrash = 0;

    //Increases the number of trash tasks completed
    public void IncrementTrash()
    {
        if (isTrashcan)
        {
            numOfTrash++;
        }
    }

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerInteraction>();

        direction = transform.forward;
        selectionHighlight = transform.GetChild(0).gameObject;
        selectionHighlight.layer = 14;
        selectionHighlight.SetActive(false);

        offsetSocket = gameObject.transform.Find("Offset Socket");
    }
        
    protected virtual void SelectObject(Collider other = null)
    {
        if (claimed || !isActive) return;

        InteractableStateController interactable = null;

        if (other != null && other.CompareTag("Held Item"))
            interactable = other.GetComponent<InteractableStateController>();
        else if (player.isHolding)
            interactable = player.itemHeld.GetComponent<InteractableStateController>();

        if (interactable == null) return;

        if (spotType != SpotType.Any && interactable.taskType != spotType.ToString())
        {
            Debug.Log("This item cannot be placed here: " + interactable.taskType + " → " + spotType);
            return;
        }

        if (other != null && other.CompareTag("Held Item"))
        {
            otherObject = other;
            interactable.newDirection = direction;

            ApplyVisualisation(other.gameObject.GetComponent<InteractableStateController>().visualisationObj, interactable);
        }
        else if (player.isHolding)
        {
            otherObject = player.itemHeld.GetComponent<Collider>();

            interactable.newDirection = direction;
            ApplyVisualisation(otherObject.gameObject.GetComponent<InteractableStateController>().visualisationObj, interactable);
        }
    }

    public void UpdateHighlightForHeldItem(InteractableStateController heldItem)
    {
        bool valid = false;

        if (heldItem != null && isActive && !claimed)
        {
            if (spotType == SpotType.Any || heldItem.taskType == spotType.ToString())
            {
                valid = true;
            }
        }

        selectionHighlight.SetActive(valid);

        if (!valid) return;

        var meshFilter = selectionHighlight.GetComponent<MeshFilter>();
        var heldMeshFilter = heldItem.visualisationObj.GetComponent<MeshFilter>();

        if (meshFilter != null && heldMeshFilter != null)
        {
            meshFilter.mesh = heldMeshFilter.mesh;
            selectionHighlight.transform.localScale = new Vector3(heldItem.visualisationObj.transform.localScale.x * highlightRescale.x, heldItem.visualisationObj.transform.localScale.y * highlightRescale.y, heldItem.visualisationObj.transform.localScale.z * highlightRescale.z);

            // Align highlight position with placementVisualisation
            selectionHighlight.transform.position = isTrashcan
                ? transform.position
                : transform.position + GetModifiedOffsetPosition(heldItem.edgeOfObject);
        }
    }

    protected virtual void DeselectObject(Collider other = null)
    {
        if (!claimed && (other != null || other == otherObject))
        {
            otherObject = null;
        }
        
    }

    protected virtual void ApplyVisualisation(GameObject obj, InteractableStateController interactable)
    {
        var meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        if (!isTrashcan)
        {
            placementOffset = GetModifiedOffsetPosition(interactable.edgeOfObject);
        }
    }

    protected virtual Vector3 GetModifiedOffsetPosition(Vector3 baseOffset)
    {
        return Vector3.zero;
    }

    public void OnHoverEnter()
    {
        if (player.itemHeld == null) return;

        SelectObject();
        CursorScript.instance.UpdateCursor("Interact");
    }

    public void OnHoverExit()
    {
        DeselectObject();
        CursorScript.instance.UpdateCursor("Default");
    }

    public void OnClick()
    {
        Debug.Log("Placement Spot running OnClick");
    }

    public void OnRelease()
    {

    }
}
