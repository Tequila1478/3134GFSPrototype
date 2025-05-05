using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSpot : MonoBehaviour
{
	public Vector3 placementPoint;
	public Vector3 direction;
	public int arrowLength = 2;

	public bool claimed = false;

	private Vector3 gizmosPosition;
	public Vector3[] controlPoints = new Vector3[4];
	public Vector3 offset = new Vector3(0, 1, 0);
	public float maxHeightAbovePoint;
	public Vector3 startingPosition;

	public bool withinRange = false;

	public Collider otherObject;

	public GameObject placementVisualisation;

	public PlayerInteraction player;

	private CustomCursor cursor;
	private Vector3 hell;

	// Start is called before the first frame update
	void Start()
	{
		if(player == null)
		{
			player = FindObjectOfType<PlayerInteraction>();
		}
		placementPoint = this.gameObject.transform.position;
		direction = this.transform.forward;
		cursor = FindObjectOfType<CustomCursor>();
	}

	// Update is called once per frame
	void Update()
	{
		if (otherObject != null)
		{
			if (withinRange && !otherObject.GetComponent<Interactable>().movingToSetSpot)
			{
				startingPosition = otherObject.gameObject.transform.position;
				createBezierCurvePoints();
				otherObject.GetComponent<Interactable>().routes = controlPoints;

			}

			if (otherObject.GetComponent<Interactable>().movingToSetSpot)
			{
				claimed = true;
				otherObject = null;
				placementVisualisation.GetComponent<MeshFilter>().mesh = null;
			}
		}
	}

	public void createBezierCurvePoints()
	{
		Vector3 modifyingVector = otherObject.GetComponent<Interactable>().edgeOfObject;
		hell = new Vector3(modifyingVector.x * direction.x, modifyingVector.y * direction.y, modifyingVector.z * direction.z);
		controlPoints[3] = this.transform.position + hell;
		controlPoints[2] = this.transform.position + hell + offset * maxHeightAbovePoint;
		controlPoints[1] = startingPosition - offset * maxHeightAbovePoint;
		controlPoints[0] = startingPosition;
	}
	private void OnDrawGizmos()
	{
		DrawArrow.ForGizmo(this.gameObject.transform.position, this.transform.forward);
		DrawBezierCurve();
		Gizmos.DrawSphere(this.transform.position, 0.15f);
	}

	private void OnTriggerEnter(Collider other)
	{
		SelectObject(other);
        this.gameObject.layer = 2;
    }

    private void OnTriggerExit(Collider other)
    {
		DeselectObject(other);
        this.gameObject.layer = 0;
		if (other.GetComponent<Interactable>()) other.GetComponent<Interactable>().hasSetSpot = false;
		claimed = false;
    }

    public void DrawBezierCurve()
	{
		for (float t = 0; t <= 1; t += 0.05f)

		{
			gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0] + 3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1] + 3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2] + Mathf.Pow(t, 3) * controlPoints[3];
			Gizmos.DrawSphere(gizmosPosition, 0.15f);
		}

		Gizmos.DrawLine(new Vector3(controlPoints[0].x, controlPoints[0].y, controlPoints[0].z), new Vector3(controlPoints[1].x, controlPoints[1].y, controlPoints[1].z));
		Gizmos.DrawLine(new Vector3(controlPoints[2].x, controlPoints[2].y, controlPoints[2].z), new Vector3(controlPoints[3].x, controlPoints[3].y, controlPoints[3].z));

	}

	public void SelectObject(Collider other = null)
	{
		if (other != null)
		{
			if (other.CompareTag("Held Item") && !claimed)
			{
				Debug.Log("Yay Im here!");
				otherObject = other;
				withinRange = true;
				other.GetComponent<Interactable>().hasSetSpot = true;
				other.GetComponent<Interactable>().newDirection = direction;

				Vector3 modifyingVector = otherObject.GetComponent<Interactable>().edgeOfObject;
				hell = new Vector3(modifyingVector.x * direction.x, modifyingVector.y * direction.y, modifyingVector.z * direction.z);
				placementVisualisation.transform.position = this.transform.position + hell;
				placementVisualisation.GetComponent<MeshFilter>().mesh = other.GetComponent<MeshFilter>().mesh;
				placementVisualisation.transform.localScale = other.transform.localScale;
				//placementVisualisation.GetComponent <MeshCollider>().sharedMesh = other.GetComponent<MeshFilter>().mesh;

            }
		}
		else
		{
            Debug.Log("HI");
            if (!claimed && player.isHolding)
            {				
                Debug.Log("Yay Im here!");
                otherObject = player.itemHeld.GetComponent<Collider>();
                withinRange = true;
                otherObject.GetComponent<Interactable>().hasSetSpot = true;
                otherObject.GetComponent<Interactable>().newDirection = direction;

                Vector3 modifyingVector = otherObject.GetComponent<Interactable>().edgeOfObject;
                hell = new Vector3(modifyingVector.x * direction.x, modifyingVector.y * direction.y, modifyingVector.z * direction.z);
                placementVisualisation.transform.position = this.transform.position + hell;
                placementVisualisation.GetComponent<MeshFilter>().mesh = otherObject.GetComponent<MeshFilter>().mesh;
                placementVisualisation.transform.localScale = otherObject.transform.localScale;
            }
        }


    }

    public void DeselectObject(Collider other = null)
	{
		if (other != null)
		{
			if (other.CompareTag("Held Item") && !claimed)
			{
				Debug.Log("Goodbye!");
				otherObject = null;
				withinRange = false;
				other.GetComponent<Interactable>().hasSetSpot = false;

				//placementVisualisation.transform.position = Vector3.zero;
				placementVisualisation.GetComponent<MeshFilter>().mesh = null;


			}
			if (other == otherObject && claimed)
			{
                other.GetComponent<Interactable>().hasSetSpot = false;
                otherObject.GetComponent<Interactable>().hasSetSpot = false;
                other = null;
				otherObject = null;
				claimed = false;
			}
		}
		else
		{
            if (!claimed)
            {
                otherObject.GetComponent<Interactable>().hasSetSpot = false;
                Debug.Log("Goodbye!");
                otherObject = null;
                withinRange = false;

                placementVisualisation.transform.position = Vector3.zero;
                placementVisualisation.GetComponent<MeshFilter>().mesh = null;

            }
        }

    }


    private void OnMouseEnter()
    {
        Debug.Log("Mouse is currently over " + gameObject.name);
		SelectObject();
        startingPosition = player.itemHeld.transform.position;
		cursor.ChangeVisual(1);
    }

    private void OnMouseExit()
    {
        DeselectObject();
		claimed = false;
		withinRange=false;
		cursor.ChangeVisual(0);
    }

    private void OnMouseUp()
    {
        if (player.itemHeld.hasSetSpot)
		{
            player.itemHeld.moveComplete = false;
            player.itemHeld.floating = false;
            player.isHolding = false;
            player.DisablePlacementPointColliders();
            //player.itemHeld = null;
            this.tag = "Interactable";
            Debug.Log("Object dropped");

            player.itemHeld.StartCoroutineMoveToLocation();
			player.itemHeld = null;
            Debug.Log("Sup");
		}
        Debug.Log("Selected: " + gameObject.name);
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
