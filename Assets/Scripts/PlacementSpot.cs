using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSpot : MonoBehaviour, IHoverable, IClickable
{
    public bool isTrashcan = false;

    public Vector3 offset = new Vector3(0, 1, 0);
    public float maxHeightAbovePoint;
    public GameObject placementVisualisation;
    public GameObject highlightVisualisation;
    public PlayerInteraction player;

    public bool claimed = false;
    public bool withinRange = false;

    //public bool highlightSpots = false;



    public Vector3[] controlPoints = new Vector3[4];
    public Vector3 placementPoint;
    public Vector3 direction;
    public Vector3 startingPosition;
    public Vector3 placementOffset;

    public CustomCursor cursor;
    public Collider otherObject;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerInteraction>();

        placementPoint = transform.position;
        direction = transform.forward;
        cursor = FindObjectOfType<CustomCursor>();

        placementVisualisation = transform.GetChild(0).gameObject;
        placementVisualisation.GetComponent<MeshFilter>().mesh = null;

        //placementVisualisation.SetActive(false);
        highlightVisualisation = transform.GetChild(1).gameObject;
        highlightVisualisation.layer = 14;
        highlightVisualisation.SetActive(false);
    }

    protected virtual void Update()
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

        if (interactable.movingToSetSpot)
        {
            claimed = true;
            otherObject = null;
            placementVisualisation.GetComponent<MeshFilter>().mesh = null;
            //placementVisualisation.SetActive(false);
        }
    }

    protected virtual void CreateBezierCurvePoints(Interactable interactable)
    {
        placementOffset = GetModifiedOffsetPosition(interactable.edgeOfObject);
        controlPoints[3] = transform.position + placementOffset;
        controlPoints[2] = controlPoints[3] + offset * maxHeightAbovePoint;
        controlPoints[1] = startingPosition - offset * maxHeightAbovePoint;
        controlPoints[0] = startingPosition;
    }

    protected virtual void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.forward);
        DrawBezierCurve();
        Gizmos.DrawSphere(transform.position, 0.15f);
    }

    public void OnTriggerEnter(Collider other)
    {
        SelectObject(other);
        gameObject.layer = 2; // Intentional: Ignore Raycast
    }

    public void OnTriggerExit(Collider other)
    {
        DeselectObject(other);
        gameObject.layer = 0;
        if (other.GetComponent<Interactable>())
            other.GetComponent<Interactable>().hasSetSpot = false;
        claimed = false;
    }

    protected virtual void DrawBezierCurve()
    {
        for (float t = 0; t <= 1; t += 0.05f)
        {
            Vector3 position = Mathf.Pow(1 - t, 3) * controlPoints[0] +
                               3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1] +
                               3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2] +
                               Mathf.Pow(t, 3) * controlPoints[3];
            Gizmos.DrawSphere(position, 0.15f);
        }

        Gizmos.DrawLine(controlPoints[0], controlPoints[1]);
        Gizmos.DrawLine(controlPoints[2], controlPoints[3]);
    }

    protected virtual void SelectObject(Collider other = null)
    {
        if (claimed) return;

        if (other != null && other.CompareTag("Held Item"))
        {
            var interactable = other.GetComponent<Interactable>();
            if (interactable == null) return;

            otherObject = other;
            withinRange = true;
            interactable.hasSetSpot = true;
            interactable.newDirection = direction;
            ApplyVisualisation(other.gameObject.GetComponent<Interactable>().visualisationObj, interactable);
        }
        else if (player.isHolding)
        {
            otherObject = player.itemHeld.GetComponent<Collider>();
            var interactable = otherObject?.GetComponent<Interactable>();
            if (interactable == null) return;

            withinRange = true;
            interactable.hasSetSpot = true;
            interactable.newDirection = direction;
            ApplyVisualisation(otherObject.gameObject.GetComponent<Interactable>().visualisationObj, interactable);
        }
        //highlightSpots = true;
    }

    protected virtual void DeselectObject(Collider other = null)
    {
        if (other != null)
        {
            var interactable = other.GetComponent<Interactable>();
            if (interactable != null)
                interactable.hasSetSpot = false;

            if (!claimed || other == otherObject)
            {
                otherObject = null;
                withinRange = false;
                placementVisualisation.GetComponent<MeshFilter>().mesh = null;
                //placementVisualisation.SetActive(false);
            }
        }
        else if (otherObject != null && !claimed)
        {
            var interactable = otherObject.GetComponent<Interactable>();
            if (interactable != null)
                interactable.hasSetSpot = false;

            otherObject = null;
            withinRange = false;
            placementVisualisation.GetComponent<MeshFilter>().mesh = null;
            //placementVisualisation.SetActive(false);
        }
    }

    protected virtual void ApplyVisualisation(GameObject obj, Interactable interactable)
    {
        placementOffset = GetModifiedOffsetPosition(interactable.edgeOfObject);
        placementVisualisation.transform.position = transform.position + placementOffset;

        var meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            //placementVisualisation.SetActive(true);
            placementVisualisation.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
        }

        placementVisualisation.transform.localScale = obj.transform.localScale;
    }

    protected virtual Vector3 GetModifiedOffsetPosition(Vector3 baseOffset)
    {
        return new Vector3(baseOffset.x * direction.x, baseOffset.y * direction.y, baseOffset.z * direction.z);
    }

    public void OnHoverEnter()
    {
        if (player.itemHeld == null) return;

        //Debug.Log("Mouse is over " + gameObject.name);
        SelectObject();
        startingPosition = player.itemHeld.transform.position;
        cursor.ChangeVisual(1);
    }

    public void OnHoverExit()
    {
        DeselectObject();
        claimed = false;
        withinRange = false;
        cursor.ChangeVisual(0);
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

            // Move to placement spot
            player.itemHeld.StartMoveToSetSpot();
            player.itemHeld = null;
            //highlightSpots = false;
        }
        else
        {
            // Free drop
            player.itemHeld.floating = false;
            player.itemHeld.moveComplete = true;
            player.itemHeld.GetComponent<Rigidbody>().isKinematic = false;

            player.itemHeld = null;
            player.isHolding = false;

            Debug.Log("Dropped freely.");
        }

        // Disable colliders AFTER object is released and trigger updates
        Invoke(nameof(DisablePlacementPointCollidersSafely), 0.1f);
    }

    public void OnRelease()
    {

    }

    protected virtual void DisablePlacementPointCollidersSafely()
{
    player.DisablePlacementPointColliders();
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
