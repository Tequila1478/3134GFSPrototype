using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class StateController : MonoBehaviour
{
    State currentState;

    public IdleState idleState = new IdleState();
    public HoverState hoverState = new HoverState();
    public PushedState pushedState = new PushedState();
    public PoppedState poppedState = new PoppedState();
    public HoopedState hoopedState = new HoopedState();

    public Interactable interactable;

    [Header("Interaction Settings")]
    [NonSerialized] public bool isHovered = false;

    public string taskType;
    public bool isRequired;
    public LayerMask interactionLayer; // Set in inspector to only hit interactable objects
    public LayerMask pickupLayer; // Set in inspector to object layers that can be interacted with and picked up
    public LayerMask basketballLayer; // Set in inspector to basketballhoop layers
    [NonSerialized] public string layerWhenUnselected; // Will be set to gameobject's layer in Awake()
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
    [NonSerialized] public Renderer[] renderers;
    [NonSerialized] public MaterialPropertyBlock mpb;
    [NonSerialized] public Color[] originalColors;
    [NonSerialized] public Color hoverColor = Color.white;

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
    [NonSerialized] public float inspectorOffset = 2f; // Exclusively used for inspector logic
    [NonSerialized] public float minRayOffset;
    [NonSerialized] public float maxRayOffset;

    [Header("Movement")]
    public Vector3[] routes;
    public ObjectInteractions oi;
    public PlacementSpot ps;
    [NonSerialized] public bool coroutineFinished = false;

    [Header("References")]
    public CustomCursor cursor;

    [NonSerialized] public PlayerInteraction playerInteraction;
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public Renderer objectRenderer;
    [NonSerialized] public CharacterController charController;
    public ParticleSystem ghostParticles;
    public ParticleSystem secondaryParticles;
    public ParticleSystem hoverParticles;
    public ParticleSystem placeParticles;

    public GameObject floatingParticles;
    [NonSerialized] public ParticleSystem[] floatingParticleSystems;
    public AudioClip pickUp;
    public AudioClip putDown;

    public bool floating = false;
    [NonSerialized] public bool isMoving = false;
    public bool moveComplete = false;
    public bool hasSetSpot = false;
    public bool isAtSetSpot = false;
    public bool movingToSetSpot = false;

    public Vector3 newDirection;
    public Vector3 edgeOfObject;

    [NonSerialized] public int routeToGo = 0;
    [NonSerialized] public float tParam = 0f;
    [NonSerialized] public float speedModifier = 0.5f;
    [NonSerialized] public Coroutine moveCoroutine = null;

    [NonSerialized] public AudioManager sfx_AM;

    private void OnValidate()
    {
        // Automatically cache components
        if (interactable == null)
            interactable = GetComponent<Interactable>();
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ChangeState(hoverState);
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }
    }

    public void ChangeState(State newState)
    {
        Debug.Log("Changing state to " + newState);

        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = newState;
        currentState.OnStateEnter(this);
    }
}

public abstract class State
{
    public StateController sc;

    public void OnStateEnter(StateController stateController)
    {
        // Code placed here will always run
        sc = stateController;
        OnEnter();
    }

    protected virtual void OnEnter()
    {
        // Code placed here can be overridden
    }

    public void OnStateUpdate()
    {
        // Code placed here will always run
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        // Code placed here can be overridden
    }

    public void OnStateHurt()
    {
        // Code placed here will always run
        OnHurt();
    }

    protected virtual void OnHurt()
    {
        // Code placed here can be overridden
    }

    public void OnStateExit()
    {
        // Code placed here will always run
        OnExit();
    }

    protected virtual void OnExit()
    {
        // Code placed here can be overridden
    }
}