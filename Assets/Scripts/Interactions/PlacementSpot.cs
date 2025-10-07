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

    public float maxHeightAbovePoint;
    public GameObject selectionHighlight;
    public Vector3 placementRescale = new Vector3 (1f, 1f, 1f);
    public Vector3 highlightRescale = new Vector3 (1f, 1f, 1f);
    public PlayerInteraction player;

    public bool claimed = false;

    //public bool highlightSpots = false;


    public SpotType spotType = SpotType.Any;


    public Vector3 placementPoint;
    public Vector3 direction;
    public Vector3 startingPosition;
    public Vector3 placementOffset;

    public CustomCursor cursor;
    public Collider otherObject;
    public int numOfTrash = 0;

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

        placementPoint = transform.position;
        direction = transform.forward;
        cursor = FindObjectOfType<CustomCursor>();

        //placementVisualisation = transform.GetChild(0).gameObject;
        //placementVisualisation.GetComponent<MeshFilter>().mesh = null;

        //placementVisualisation.SetActive(false);
        selectionHighlight = transform.GetChild(1).gameObject;
        selectionHighlight.layer = 14;
        selectionHighlight.SetActive(false);
    }

    protected virtual void Update()
    {
        if (otherObject == null) return;

        var interactable = otherObject.GetComponent<InteractableStateController>();
        if (interactable == null) return;
    }


    protected virtual void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.forward);
        Gizmos.DrawSphere(transform.position, 0.15f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        SelectObject(other);
        SetLayer(2); // Intentional: Ignore Raycast
    }

    public void OnTriggerExit(Collider other)
    {
        isActive = true;
        DeselectObject(other);
        SetLayer(8);
        //claimed = false;
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
        //highlightSpots = true;
    }

    public void UpdateHighlightForHeldItem(InteractableStateController heldItem)
    {
        bool valid = false;
        Debug.Log("POOP Updating Highlights");

        if (heldItem != null && isActive && !claimed)
        {
            if (spotType == SpotType.Any || heldItem.taskType == spotType.ToString())
            {
                valid = true;
                Debug.Log("POOP valid = true");
            }
            else
            {
                Debug.Log("POOP valid = false");
            }
        }
        else
        {
            Debug.Log("POOP valid = false");
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
        if (other != null)
        {
            if (!claimed || other == otherObject)
            {
                otherObject = null;
            }
        }
        else if (otherObject != null && !claimed)
        {
            otherObject = null;
        }
    }

    protected virtual void ApplyVisualisation(GameObject obj, InteractableStateController interactable)
    {
        var meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        // Set mesh
        //placementVisualisation.GetComponent<MeshFilter>().mesh = meshFilter.mesh;

        // Set scale
        //placementVisualisation.transform.localScale = new Vector3(obj.transform.localScale.x * placementRescale.x, obj.transform.localScale.y * placementRescale.y, obj.transform.localScale.z * placementRescale.z);

        if (isTrashcan)
        {
            // Trashcan: center placement visualisation on bin, ignore offset
            //placementVisualisation.transform.position = transform.position;
        }
        else
        {
            // Normal spots: apply offset
            placementOffset = GetModifiedOffsetPosition(interactable.edgeOfObject);
            //placementVisualisation.transform.position = transform.position + placementOffset;
        }
    }

    protected virtual Vector3 GetModifiedOffsetPosition(Vector3 baseOffset)
    {
        return Vector3.zero;
        //return new Vector3(baseOffset.x * direction.x, baseOffset.y * direction.y, baseOffset.z * direction.z);
    }

    public void OnHoverEnter()
    {
        if (player.itemHeld == null) return;

        //Debug.Log("Mouse is over " + gameObject.name);
        SelectObject();
        startingPosition = player.itemHeld.transform.position;
        //cursor.ChangeVisual(1);
        CursorScript.instance.UpdateCursor("Interact");
    }

    public void OnHoverExit()
    {
        DeselectObject();
        CursorScript.instance.UpdateCursor("Default");
        SetLayer(8);
    }

    public void OnClick()
    {
        Debug.Log("Placement Spot running OnClick");
    }

    public void OnRelease()
    {

    }

    protected virtual void DisablePlacementPointCollidersSafely()
    {
        player.DisablePlacementPointColliders();
    }

    public void SetLayer(int layerNum)
    {
        if (isTrashcan) return; //trashcan should never change layer
        this.gameObject.layer = layerNum;
    }
}

	public static class DrawArrow
{
	public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.DrawRay(pos, direction);

		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}

	public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.color = color;
		Gizmos.DrawRay(pos, direction);

		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}

	public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction);

		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength);
		Debug.DrawRay(pos + direction, left * arrowHeadLength);
	}
	public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction, color);

		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
		Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
	}
}
