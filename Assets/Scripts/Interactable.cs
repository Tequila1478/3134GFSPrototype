using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour, IHoverable, IClickable
{
    [Header("Interaction Settings")]
    private bool isHovered = false;

    [Header("Interaction Settings")]
    public string taskType;
    public bool isRequired;

    [Header("Materials")]
    public Material outlineMat;
    public Material originalMat;
    public GameObject materialObj;
    public GameObject visualisationObj;

    [Header("Floating Settings")]
    public float speed = 2f;
    public float height = 0.01f;
    public float rotation = 0.1f;

    [Header("Movement")]
    public Vector3[] routes;
    public ObjectInteractions oi;

    [Header("References")]
    public CustomCursor cursor;

    private PlayerInteraction playerInteraction;
    private Rigidbody rb;
    private Renderer objectRenderer;
    private CharacterController charController;
    public ParticleSystem ghostParticles;
    public AudioClip pickUp;
    public AudioClip putDown;

    public bool floating = false;
    private bool isMoving = false;
    public bool moveComplete = false;
    public bool hasSetSpot = false;
    public bool movingToSetSpot = false;

    public Vector3 newDirection;
    public Vector3 edgeOfObject;

    private int routeToGo = 0;
    private float tParam = 0f;
    private float speedModifier = 0.5f;
    private Coroutine moveCoroutine = null;

    private AudioManager sfx_AM;


    private void Awake()
    {
        sfx_AM = FindObjectOfType<AudioManager>();
        if (sfx_AM == null)
        {
            Debug.Log("No audio manager");
        }
    }
    private void Start()
    {
        CacheComponents();
        ValidateSetup();

        edgeOfObject = objectRenderer.localBounds.extents * transform.localScale.magnitude;

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
        if(ghostParticles == null)
        ghostParticles = GetComponent<ParticleSystem>();

        if (materialObj != null)
        {
            objectRenderer = materialObj.GetComponent<Renderer>();
        }
        else
        {
            Debug.LogError("Material object is not assigned in " + gameObject.name);
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

    private void Update()
    {
        HandleFloating();
        HandleInput();
        RotateToDirectionIfNeeded();
    }

    private void HandleFloating()
    {
        if (!floating) return;

        if (moveComplete && !isMoving)
        {
            float floatY = Mathf.Sin(Time.time * speed) * height;
            transform.position += new Vector3(0, floatY, 0);
            transform.Rotate(0, rotation * Time.deltaTime, 0);
        }
        else if (!isMoving)
        {
            float targetY = transform.position.y + height * 10;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), speed * 50 * Time.deltaTime);
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
        Debug.Log("Hovering over object");
        if (isHovered) return;
        isHovered = true;

        if (!playerInteraction.isHolding)
        {
            HighlightObject();
            cursor?.ChangeVisual(1);
        }
        if (playerInteraction.itemHeld == this)
        {
            cursor?.ChangeVisual(1);
        }
    }

    public void OnHoverExit()
    {
        if (!isHovered) return;
        isHovered = false;

        if (!playerInteraction.isHolding)
        {
            UnhighlightObject();
            cursor?.ChangeVisual(0);
        }
        if (playerInteraction.itemHeld == this)
        {
            cursor?.ChangeVisual(1);
        }
    }

    public void OnClick()
    {
        if (!floating && playerInteraction.itemHeld == null)
        {
            EnableFloating();
            sfx_AM.PlaySFX(pickUp);
        }
        movingToSetSpot = false;
        gameObject.layer = 9;

        Debug.Log("Clicked: " + this);
    }

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
            cursor?.ChangeVisual(0);
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
        ghostParticles.Play();
    }

    private void DropObject(bool forceDrop = false)
    {
        floating = false;
        moveComplete = false;
        playerInteraction.isHolding = false;
        playerInteraction.itemHeld = null;
        playerInteraction.DisablePlacementPointColliders();
        tag = "Interactable";
        sfx_AM?.PlaySFX(putDown);
        ghostParticles.Stop();

        oi?.ClearPlacementSpots();

        if (forceDrop)
        {
            hasSetSpot = false;
        }

        if (hasSetSpot)
        {
            StartMoveToSetSpot();
        }
        else
        {
            rb.useGravity = true;
            rb.drag = 0;
        }
    }

    public void StartMoveToSetSpot()
    {
        oi?.ClearPlacementSpots();

        if (!hasSetSpot) return;

        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(GoByTheRoute(routeToGo));
            movingToSetSpot = true;
            sfx_AM?.PlaySFX(putDown);
            ghostParticles.Stop();
        }
        
    }

    private IEnumerator GoByTheRoute(int routeNum)
    {
        tParam = 0;
        Vector3 p0 = routes[0];
        Vector3 p1 = routes[1];
        Vector3 p2 = routes[2];
        Vector3 p3 = routes[3];

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;
            Vector3 position = Mathf.Pow(1 - tParam, 3) * p0 +
                               3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                               3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                               Mathf.Pow(tParam, 3) * p3;

            transform.position = position;
            yield return null;
        }

        rb.isKinematic = true;
        moveCoroutine = null;
    }

    private void HighlightObject()
    {
        if (outlineMat != null && objectRenderer != null)
        {
            //outlineMat.SetTexture("_Texture2D", objectRenderer.material.mainTexture);
           // objectRenderer.material = outlineMat;
        }
    }

    private void UnhighlightObject()
    {
        if (originalMat != null && objectRenderer != null)
        {
            //objectRenderer.material = originalMat;
        }
    }


}