using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]


public class Interactable_Revised : MonoBehaviour
{
    // Highlight materials or effects
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer rend;

    // Movement states
    private bool isHovered = false;
    private bool isClicked = false;
    private bool isFloating = false;
    private bool isFalling = false;
    private bool isBeingPickedUp = false;

    // Float animation
    public float floatSpeed = 1f;
    public float floatHeight = 0.25f;
    public float rotationSpeed = 30f;

    private Vector3 originalPosition;
    private float floatTimer;

    // Movement
    public float riseSpeed = 2f;
    public float fallSpeed = 5f;
    private Rigidbody rb;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnMouseEnter()
    {
        if (!isClicked)
        {
            HighlightObject();
        }
        isHovered = true;
    }

    private void OnMouseExit()
    {
        if (!isClicked)
        {
            UnHighlightObject();
        }
        isHovered = false;
    }

    private void OnMouseDown()
    {
        if (!isClicked)
        {
            isClicked = true;
            isBeingPickedUp = true;
        }
        else if (isFloating)
        {
            // If it's floating and clicked again, drop it
            isFloating = false;
            isFalling = true;
            isClicked = false;
            UnHighlightObject();
        }
    }

    private void OnMouseUp()
    {
        if (isBeingPickedUp)
        {
            isBeingPickedUp = false;
            isFloating = true;
            floatTimer = 0f;
        }
    }

    private void Update()
    {
        if (isBeingPickedUp)
        {
            // Move up
            rb.isKinematic = true;
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
        else if (isFloating)
        {
            rb.isKinematic = true;

            floatTimer += Time.deltaTime * floatSpeed;
            float offset = Mathf.Sin(floatTimer) * floatHeight;
            Vector3 floatPos = new Vector3(transform.position.x, originalPosition.y + 1f + offset, transform.position.z);
            transform.position = floatPos;

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (isFalling)
        {
            rb.isKinematic = false; // Re-enable physics
            isFalling = false; // One-time toggle
        }
    }

    public void HighlightObject()
    {
        rend.material = highlightMaterial;
    }

    public void UnHighlightObject()
    {
        rend.material = originalMaterial;
    }
}
