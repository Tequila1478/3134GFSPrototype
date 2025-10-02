using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour, IHoverable, IClickable
{
    [Header("Interaction Settings")]
    private bool isHovered = false;

    [Header("Interaction Settings")]
    public string taskType;
    public bool isRequired;
    public LayerMask interactionLayer; // Set in inspector to only hit interactable objects
    public LayerMask pickupLayer; // Set in inspector to object layers that can be interacted with and picked up
    public LayerMask basketballLayer; // Set in inspector to basketballhoop layers
    private string layerWhenUnselected; // Will be set to gameobject's layer in Awake()
    [Tooltip("Object will temporarily switch to this layer while it is selected in-game.")]
    public string layerWhenSelected; // Must be set in the inspector
    [Tooltip("Put all GameObjects that can change layer here. (This is important for GameObjects with multiple children.) This will always include the GameObject this script is attached to.")]
    public List<GameObject> layerableObjects;
    [Tooltip("Disabling cursor controls will instead allow the object to be controlled using keyboard.")]
    public bool disableCursorControls = false;

    [Header("Materials")]
    public Material outlineMat;
    public Material originalMat;
    public GameObject materialObj;
    public GameObject visualisationObj;
    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private Color[] originalColors;
    private Color hoverColor = Color.white;

    [Header("Floating Settings")]
    [Tooltip("Rate at which the object follows the cursor when selected.")]
    [Range(0.01f, 1f)]
    public float followRate = 0.05f;
    [Tooltip("Speed of the object when moving to a placement spot.")]
    public float speed = 2f;
    public float height = 0.01f;
    public float rotation = 0.1f;
    [Tooltip("Ray Offset controls how far a selected object floats from whatever surfaces you are pointing the cursor at.")]
    public float rayOffset = 2f; // This is the literal offset.
    public float rayVisualOffset = 2f; // This is the offset that can be seen in-game.
    private float inspectorOffset = 2f; // Exclusively used for inspector logic
    private float minRayOffset;
    private float maxRayOffset;

    [Header("Movement")]
    public Vector3[] routes;
    public ObjectInteractions oi;
    public PlacementSpot ps;
    private bool coroutineFinished = false;

    [Header("References")]
    public CustomCursor cursor;

    private PlayerInteraction playerInteraction;
    private Rigidbody rb;
    private Renderer objectRenderer;
    private CharacterController charController;
    public ParticleSystem ghostParticles;
    public ParticleSystem secondaryParticles;
    public ParticleSystem hoverParticles;
    public ParticleSystem placeParticles;

    public GameObject floatingParticles;
    private ParticleSystem[] floatingParticleSystems;
    public AudioClip pickUp;
    public AudioClip putDown;

    public bool floating = false;
    private bool isMoving = false;
    public bool moveComplete = false;
    public bool hasSetSpot = false;
    public bool isAtSetSpot = false;
    public bool movingToSetSpot = false;

    public Vector3 newDirection;
    public Vector3 edgeOfObject;

    private int routeToGo = 0;
    private float tParam = 0f;
    private float speedModifier = 0.5f;
    private Coroutine moveCoroutine = null;

    private AudioManager sfx_AM;



    private void OnValidate()
    {
        if (layerableObjects.Count == 0) // Set a default layerableObjects with the gameObject this script is attached to (do NOT programmatically attach child objects here!)
        {
            layerableObjects = new List<GameObject> { gameObject };
        }
        else if (!layerableObjects.Contains(gameObject)) // Readd attached gameObject if it is removed from list
        {
            layerableObjects.Insert(0, gameObject);
        }

        if (rayVisualOffset != inspectorOffset)
        {
            inspectorOffset = rayVisualOffset;
            rayOffset = rayVisualOffset;
        }
        if (rayOffset != inspectorOffset)
        {
            inspectorOffset = rayOffset;
            rayVisualOffset = rayOffset;
        }
    }

    private void Awake()
    {
        layerWhenUnselected = LayerMask.LayerToName(gameObject.layer);

        minRayOffset = rayOffset; //Update minimum RayOffset to match inspector

        sfx_AM = FindObjectOfType<AudioManager>();
        if (sfx_AM == null)
        {
            Debug.Log("No audio manager");
        }
        if (ghostParticles != null) ghostParticles.Stop();
        if (secondaryParticles != null) secondaryParticles.Stop();
        if (hoverParticles != null) hoverParticles.Stop();
        floatingParticleSystems = floatingParticles.GetComponentsInChildren<ParticleSystem>(true);
        //if (floatingParticles != null) floatingParticles.SetActive(false);
    }
    private void Start()
    {
        //Debug.Log("POOP START INTERACTABLE");
        CacheComponents();
        ValidateSetup();

        edgeOfObject = objectRenderer != null ? objectRenderer.localBounds.extents * transform.localScale.magnitude : new Vector3(1, 1, 1) * transform.localScale.magnitude;

        if (outlineMat != null && objectRenderer != null)
        {
            //outlineMat.SetTexture("_Texture2D", objectRenderer.material.mainTexture);
        }


    }

    private void CacheComponents()
    {
        rb = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        cursor = FindObjectOfType<CustomCursor>();
        oi = GetComponent<ObjectInteractions>();

        if (materialObj != null)
        {
            objectRenderer = materialObj.GetComponent<Renderer>();
        }
        else
        {
            Debug.LogError("Material object is not assigned in " + gameObject.name);
        }

        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
        else
        {
            renderers = new Renderer[] { rend };
        }

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].sharedMaterial; // or .material if you want a unique instance
            if (mat != null && mat.HasProperty("_OutlineColour"))
            {
                originalColors[i] = mat.GetColor("_OutlineColour");
            }
            else
            {
                originalColors[i] = Color.clear; // or some default value
            }
        }
    }

    private void ValidateSetup()
    {
        if (outlineMat == null)
        {
            Debug.LogError("Outline material not assigned for " + gameObject.name);
        }

        if (originalMat == null && objectRenderer != null)
        {
            originalMat = objectRenderer.material;
        }

        if (playerInteraction == null)
        {
            Debug.LogError("PlayerInteraction script not found in scene.");
        }
    }

    private void SetNewLayer(string layerName)
    {
        foreach (GameObject component in layerableObjects)
        {
            component.layer = LayerMask.NameToLayer(layerName);
        }
    }

    private void Update()
    {
        HandleFloating();
        HandleInput();
        RotateToDirectionIfNeeded();

        // Release object with right-click
        if (Input.GetMouseButtonDown(1))
        {
            OnRelease();
        }

        //Update layer
        if (!floating)
        {
            if (isHovered)
            {
                SetNewLayer("HoverOutline");
            } else
            {
                SetNewLayer(layerWhenUnselected);
            }
        }
        else
        {
            SetNewLayer(layerWhenSelected);
        }

        // Move the interactable using raycasts to find position
        if (floating && !disableCursorControls) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Move to basketball hoop (if elligible)
            if (Physics.Raycast(ray, out hit, 100f, basketballLayer))
            {
                if (hit.collider.TryGetComponent<BasketballHoop>(out var bbhoop))
                {
                    bbhoop.HoopIt(this.GetComponent<Interactable>());
                }
            }
            // Move to mouse position within world
            else if (Physics.Raycast(ray, out hit, 100f, interactionLayer))
            {
                maxRayOffset = hit.distance - minRayOffset; // Update maximum offset to match ray hit distance
                rayOffset = Mathf.Max(rayOffset + Input.mouseScrollDelta.normalized.y, minRayOffset);
                rayVisualOffset = Mathf.Clamp(rayOffset, minRayOffset, maxRayOffset);
                if (Input.mouseScrollDelta.y < 0)
                {
                    rayOffset = rayVisualOffset;
                }
                if (Input.mouseScrollDelta.y > 0 && rayOffset > rayVisualOffset)
                {
                    rayOffset = rayVisualOffset;
                }

                Vector3 newPoint = ray.GetPoint(hit.distance - (maxRayOffset - rayVisualOffset));
                transform.position = Vector3.MoveTowards(transform.position, newPoint, followRate * Vector3.Distance(transform.position, newPoint));
            }
        }

        if (coroutineFinished)
        {
            coroutineFinished = false;
            if (ps.isTrashcan)
            {
                Debug.Log("Im a trashcan");
                DropObject(true);
                isAtSetSpot = true;
                rb.useGravity = true;
                rb.drag = 0;
                rb.isKinematic = false;
                ps.claimed = false;
                ps.SetLayer(8);
                //gameObject.SetActive(false);
            }

        }
    }

    public Vector2 NormalizeScroll()
    {
        Debug.Log(Input.mouseScrollDelta);
        if (Input.mouseScrollDelta.y > 0)
        {
            return Vector2.up;
        } else if (Input.mouseScrollDelta.y < 0)
        {
            return Vector2.down;
        } else
        {
            return Vector2.zero;
        }
    }

    public void HandleFloating()
    {
        if (!floating) return;

        if (moveComplete && !isMoving)
        {
            //float floatY = Mathf.Sin(Time.time * speed) * height;
            //transform.position += new Vector3(0, floatY, 0);
            //transform.Rotate(0, rotation * Time.deltaTime, 0);

            if (ps != null && ps.isTrashcan)
            {
                ps.claimed = false;
                DropObject(true);
                rb.useGravity = true;
                rb.freezeRotation = false;
                rb.constraints = RigidbodyConstraints.None;
                rb.drag = 0;
                ps.SetLayer(8);
            }
        }
        else if (!isMoving)
        {
            float targetY = transform.position.y + height * 10;
            var newPosition = new Vector3(transform.position.x, targetY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * 50 * Time.deltaTime );
        }
    }

    private void HandleInput()
    {
        if (playerInteraction == null || playerInteraction.itemHeld != this) return;

        if (IsAnyMovementKeyPressed())
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
            if (charController != null)
                charController.enabled = false;

            //oi?.ClearPlacementSpots();
            //Change this to be when put to spot!
        }

        if (isMoving)
        {
            if (charController != null)
                charController.enabled = true;

            oi?.Move();
        }        
    }

    private IEnumerator ShrinkAndRemove()
    {
        float duration = 3f; // total time to shrink
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = Mathf.Pow(t, 0.5f);

            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        transform.localScale = Vector3.zero;

        // Finally, disable visuals and colliders
        if (objectRenderer != null)
            objectRenderer.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        gameObject.SetActive(false); // or Destroy(gameObject)
    }

    private void RotateToDirectionIfNeeded()
    {
        if (!movingToSetSpot) return;

        float step = speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, newDirection, step, 0));

        if (Vector3.Angle(transform.forward, newDirection) < 1f)
        {
            movingToSetSpot = false;
        }
    }

    private bool IsAnyMovementKeyPressed()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
               Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E);
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hovering over object"); //Debug
        if (isHovered) return;
        isHovered = true;


        if (!playerInteraction.isHolding)
        {
            HighlightObject();
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
            if (hoverParticles != null) hoverParticles.Play();

        }
        if (playerInteraction.itemHeld == this)
        {
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
        }

    }

    public void OnHoverExit()
    {
        if (!isHovered) return;
        isHovered = false;


        if (hoverParticles != null) hoverParticles.Stop();


        if (!playerInteraction.isHolding)
        {
            UnhighlightObject();
            //cursor?.ChangeVisual(0);
            CursorScript.instance.UpdateCursor("Default");
        }
        if (playerInteraction.itemHeld == this)
        {
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
        }
    }

    // This is run by PlayerInteraction.cs to select the interactable
    public void OnClick()
    {
        if(ps != null) ps.claimed = false;

        transform.SetParent(null, true);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine); // Prevent object from continuing to move towards a placement spot
            moveCoroutine = null;         // Forget placement movement
            movingToSetSpot = false;
            isAtSetSpot = false;
            coroutineFinished = false;
        }

        if (!floating && playerInteraction.itemHeld == null)
        {
            EnableFloating();
            sfx_AM.PlaySFX(pickUp);
            ps = null;
        }

        Debug.Log("Clicked: " + this);
    }

    // This is run by PlayerInteraction.cs to deselect the interactable
    public void OnRelease()
    {
        if (!floating) return;

        if (moveComplete)
        {
            DropObject(true);
        }
        else
        {
            moveComplete = true;
            //cursor?.ChangeVisual(0);
            CursorScript.instance.UpdateCursor("Default");
        }
    }

    private void EnableFloating()
    {
        floating = true;
        rb.useGravity = false;
        rb.drag = 4;
        rb.isKinematic = false;

        playerInteraction.isHolding = true;
        playerInteraction.itemHeld = this;
        playerInteraction.EnablePlacementPointColliders();
        tag = "Held Item";
        if (ghostParticles != null) ghostParticles.Play();
        PlayAllFloatingParticles();
        if (secondaryParticles != null) secondaryParticles.Play();
        if (hoverParticles != null) hoverParticles.Stop();
        isAtSetSpot = false;

        ///MOVE THIS ONCE DONE TESTING
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, pickupLayer))
        {
            Debug.Log("POOP distance: " + hit.distance);
            rayOffset = Mathf.Max(hit.distance, minRayOffset);
            rayVisualOffset = Mathf.Clamp(rayOffset, minRayOffset, maxRayOffset);
        } else
        {
            Debug.Log("POOP distance: NO.");
        }
            ///

            SetCollidersTrigger(false);

    }

    public void PlayAllFloatingParticles()
    {
        foreach (var ps in floatingParticleSystems)
        {
            ps.Play();
        }
    }

    public void StopAllFloatingParticles(bool clear = false)
    {
        foreach (var ps in floatingParticleSystems)
        {
            ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);

            if (clear)
            {
                ps.Clear();
            }
        }
    }

    public void DropObject(bool forceDrop = false)
    {
        DropObject(ps, forceDrop);
    }

    public void DropObject(PlacementSpot newPlacementSpot, bool forceDrop = false)
    {
        Debug.Log("Poop3: Started DropObject(" + forceDrop + ")");
        floating = false;
        moveComplete = false;
        playerInteraction.isHolding = false;
        playerInteraction.itemHeld = null;
        playerInteraction.DisablePlacementPointColliders();
        tag = "Interactable";
        sfx_AM?.PlaySFX(putDown);
        if (ghostParticles != null) ghostParticles.Stop();
        //if (floatingParticles != null) floatingParticles.SetActive(false);
        StopAllFloatingParticles();
        if (secondaryParticles != null) secondaryParticles.Stop();


        oi?.ClearPlacementSpots();

        if (forceDrop)
        {
            Debug.Log("Poop3: Doing forceDrop");
            hasSetSpot = false;
            isAtSetSpot = false;
        }

        if (hasSetSpot)
        {
            Debug.Log("Poop3: Moving to set spot");
            StartMoveToSetSpot(newPlacementSpot);
        }
        else
        {
            Debug.Log("Poop3: Reenabling gravity");
            rb.useGravity = true;
            rb.drag = 0;
            isAtSetSpot = false;
        }
    }

    public bool StartMoveToSetSpot(PlacementSpot placementSpot, bool forceMove = false) // Will return bool of whether object has started moving to set spot
    {
        ps = placementSpot;
        oi?.ClearPlacementSpots();

        if (!hasSetSpot && !forceMove) return false;

        if (moveCoroutine == null || forceMove)
        {
            SetCollidersTrigger(true);

            moveCoroutine = StartCoroutine(MoveDirectlyToSpot(ps.transform.position));
            movingToSetSpot = true;
            sfx_AM?.PlaySFX(putDown);
            ghostParticles.Stop();
            //if (floatingParticles != null) floatingParticles.SetActive(false);
            StopAllFloatingParticles();

            return true;
        }

        return false;

    }

    private IEnumerator MoveDirectlyToSpot(Vector3 targetPos)
    {
        // Ensure rigidbody doesn't interfere
        rb.useGravity = false;
        rb.isKinematic = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f) // threshold
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
            yield return null;
        }

        // Snap to final position
        transform.position = targetPos;

        // If the placement spot wants alignment, also set rotation
        if (ps != null)
        {
            transform.rotation = Quaternion.LookRotation(ps.direction);

            transform.SetParent(ps.transform, true);
        }

        // Mark as complete
        movingToSetSpot = false;
        moveComplete = true;
        isAtSetSpot = true;
        coroutineFinished = true;

        moveCoroutine = null;
    }

    private void HighlightObject()
    {
        foreach (var rend in renderers)
        {
            var mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", hoverColor);
            rend.SetPropertyBlock(mpb);
        }

        if (outlineMat != null && objectRenderer != null)
        {
            //outlineMat.SetTexture("_Texture2D", objectRenderer.material.mainTexture);
           // objectRenderer.material = outlineMat;
        }
    }

    private void UnhighlightObject()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            var mpb = new MaterialPropertyBlock();
            var rend = renderers[i];
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", originalColors[i]);
            rend.SetPropertyBlock(mpb);
        }
        if (originalMat != null && objectRenderer != null)
        {
            //objectRenderer.material = originalMat;
        }
    }


    private void SetCollidersTrigger(bool isTrigger)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.isTrigger = isTrigger;
        }
    }

}