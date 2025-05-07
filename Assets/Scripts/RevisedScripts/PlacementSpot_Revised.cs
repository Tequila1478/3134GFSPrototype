using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSpot_Revised : MonoBehaviour
{
    public Vector3 placementPoint;          // The point where the object will be placed
    public Vector3 direction;               // The direction the object is facing
    public int arrowLength = 2;             // Length of the arrow for gizmo visualization

    public bool claimed = false;            // Whether the placement spot is currently claimed by an object
    private Vector3 gizmosPosition;         // Used for drawing the Bezier curve in gizmos
    public Vector3[] controlPoints = new Vector3[4]; // Control points for the Bezier curve
    public Vector3 offset = new Vector3(0, 1, 0); // Offset applied for curve manipulation
    public float maxHeightAbovePoint;       // Maximum height the object can be placed above the point
    public Vector3 startingPosition;        // The starting position of the object being moved

    public bool withinRange = false;        // Whether the object is within range for placement
    public Collider otherObject;            // The object currently interacting with the placement spot
    public GameObject placementVisualisation; // Visual representation of placement
    public PlayerInteraction player;        // Reference to the player interaction script

    private Vector3 hell;                   // Temporary vector used for curve calculations

    // Start is called before the first frame update
    void Start()
    {
        // Find the PlayerInteraction script if not already set
        if (player == null)
        {
            player = FindObjectOfType<PlayerInteraction>();
        }

        // Initialize placement point and direction
        placementPoint = this.gameObject.transform.position;
        direction = this.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if another object is interacting with the placement spot
        if (otherObject != null)
        {
            // If within range and the object is not moving to the set spot, calculate the Bezier curve points
            if (withinRange && !otherObject.GetComponent<Interactable>().movingToSetSpot)
            {
                startingPosition = otherObject.gameObject.transform.position;
                createBezierCurvePoints();
                otherObject.GetComponent<Interactable>().routes = controlPoints;
            }

            // If the object is moving to the set spot, mark it as claimed and remove visualization
            if (otherObject.GetComponent<Interactable>().movingToSetSpot)
            {
                claimed = true;
                otherObject = null;
                placementVisualisation.GetComponent<MeshFilter>().mesh = null;
            }
        }
    }

    // Create the Bezier curve control points based on the position and direction of the object
    public void createBezierCurvePoints()
    {
        Vector3 modifyingVector = otherObject.GetComponent<Interactable>().edgeOfObject;
        hell = new Vector3(modifyingVector.x * direction.x, modifyingVector.y * direction.y, modifyingVector.z * direction.z);
        controlPoints[3] = this.transform.position + hell; // End point of the curve
        controlPoints[2] = this.transform.position + hell + offset * maxHeightAbovePoint; // Control point 2
        controlPoints[1] = startingPosition - offset * maxHeightAbovePoint; // Control point 1
        controlPoints[0] = startingPosition; // Start point of the curve
    }

    // Draw the Bezier curve for visualization in the scene view
    public void DrawBezierCurve()
    {
        // Draw the Bezier curve by iterating t from 0 to 1
        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0]
                + 3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1]
                + 3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2]
                + Mathf.Pow(t, 3) * controlPoints[3];
            Gizmos.DrawSphere(gizmosPosition, 0.15f);  // Draw each point on the curve
        }

        // Draw lines connecting the control points for better visualization
        Gizmos.DrawLine(controlPoints[0], controlPoints[1]);
        Gizmos.DrawLine(controlPoints[2], controlPoints[3]);
    }

    // Called when an object enters the trigger area (placement spot)
    private void OnTriggerEnter(Collider other)
    {
        SelectObject(other);
        this.gameObject.layer = 2;  // Set the layer to interactable during selection
    }

    // Called when an object exits the trigger area
    private void OnTriggerExit(Collider other)
    {
        DeselectObject(other);
        this.gameObject.layer = 0;  // Reset the layer when deselected
        if (other.GetComponent<Interactable>())
        {
            other.GetComponent<Interactable>().hasSetSpot = false;
        }
        claimed = false;  // Reset claimed flag
    }

    // Select the object and start visualizing the placement
    public void SelectObject(Collider other = null)
    {
        if (other != null)
        {
            if (other.CompareTag("Held Item") && !claimed)
            {
                otherObject = other;
                withinRange = true;
                other.GetComponent<Interactable>().hasSetSpot = true;
                other.GetComponent<Interactable>().newDirection = direction;

                // Set the placement visualization
                Vector3 modifyingVector = otherObject.GetComponent<Interactable>().edgeOfObject;
                hell = new Vector3(modifyingVector.x * direction.x, modifyingVector.y * direction.y, modifyingVector.z * direction.z);
                placementVisualisation.transform.position = this.transform.position + hell;
                placementVisualisation.GetComponent<MeshFilter>().mesh = other.GetComponent<MeshFilter>().mesh;
                placementVisualisation.transform.localScale = other.transform.localScale;
            }
        }
        else
        {
            if (!claimed && player.isHolding)
            {
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

    // Deselect the object and hide the visualization
    public void DeselectObject(Collider other = null)
    {
        if (other != null)
        {
            if (other.CompareTag("Held Item") && !claimed)
            {
                otherObject = null;
                withinRange = false;
                other.GetComponent<Interactable>().hasSetSpot = false;

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
                otherObject = null;
                withinRange = false;

                placementVisualisation.transform.position = Vector3.zero;
                placementVisualisation.GetComponent<MeshFilter>().mesh = null;
            }
        }
    }

    // Called when the mouse enters the collider of the object
    private void OnMouseEnter()
    {
        SelectObject();
        startingPosition = player.itemHeld.transform.position;
    }

    // Called when the mouse exits the collider of the object
    private void OnMouseExit()
    {
        DeselectObject();
        claimed = false;
        withinRange = false;
    }

    // Called when the mouse button is released on the placement spot
    private void OnMouseUp()
    {
        if (player.itemHeld.hasSetSpot)
        {
            player.itemHeld.moveComplete = false;
            player.itemHeld.floating = false;
            player.isHolding = false;
            player.DisablePlacementPointColliders();

            // Start moving the object to its set spot
            player.itemHeld.StartMoveToSetSpot();
            player.itemHeld = null;
        }
    }
}
