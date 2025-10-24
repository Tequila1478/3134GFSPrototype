using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableStateController : MonoBehaviour, IClickable, IHoverable
{
    public State currentState;

    public IdleState idleState = new IdleState();
    public FloatState floatState = new FloatState();
    public PushedState pushedState = new PushedState();
    public PoppedState poppedState = new PoppedState();
    public DunkedState dunkedState = new DunkedState();

    [Header("Debug Settings")]
    public TextMeshProUGUI debugText;

    [Header("Interaction Settings")]
    [NonSerialized] public bool isHovered = false; //Whether object is currently hovered over with mouse
    public Transform offsetPlug;

    public string taskType;
    public bool isRequired;
    public LayerMask interactionLayer; // Set in inspector to only hit interactable objects
    public LayerMask pickupLayer; // Set in inspector to object layers that can be interacted with and picked up
    public LayerMask basketballLayer; // Set in inspector to basketballhoop layers
    [NonSerialized] public string layerWhenUnselected; // Will be set to gameobject's layer in Awake()
    [Tooltip("Object will temporarily switch to this layer while it is selected in-game.")]
    public string layerWhenSelected; // Must be set in the inspector
    [Tooltip("Object will temporarily switch to this layer while it is hovered over in-game.")]
    public string layerWhenHovered = "HoverOutline"; // Must be set in the inspector
    [Tooltip("Put all GameObjects that can change layer here. (This is important for GameObjects with multiple children.) This will always include the GameObject this script is attached to.")]
    public List<GameObject> layerableObjects;

    [Header("Materials")]
    public Material outlineMat;
    public Material originalMat;
    public GameObject materialObj;
    public GameObject visualisationObj;
    [NonSerialized] public Renderer[] renderers;
    [NonSerialized] public MaterialPropertyBlock mpb;
    [NonSerialized] public UnityEngine.Color[] originalColors;
    [NonSerialized] public UnityEngine.Color hoverColor = UnityEngine.Color.white;

    [Header("Floating Settings")]
    [Tooltip("Rate at which the object follows the cursor when selected.")]
    [Range(0.01f, 100f)]
    public float followRate = 20f;
    [Tooltip("Speed of the object when moving to a placement spot. Note that this is also modified by the follow rate.")]
    public float speed = 2f;
    public float height = 0.01f;
    public float rotation = 0.1f;
    public Vector3 rotationOffset = new Vector3(0, 0, 90);
    [NonSerialized] public Quaternion _rotationOffset = new Quaternion(0, 0, 90, 0);
    [Tooltip("Ray Offset controls how far a selected object floats from whatever surfaces you are pointing the cursor at.")]
    public float rayOffset = 2f; // This is the literal offset.
    public float rayVisualOffset = 2f; // This is the offset that can be seen in-game.
    [NonSerialized] public float inspectorOffset = 2f; // Exclusively used for inspector logic
    [NonSerialized] public float minRayOffset;
    [NonSerialized] public float maxRayOffset;

    [Header("Movement")]
    public ObjectInteractions oi;
    [Tooltip("The Placement Spot this object is attached to. Should be left unassigned in inspector, unless needed otherwise.")]
    public PlacementSpot ps;
    [NonSerialized] public bool coroutineFinished = false;

    [Header("References")]
    [NonSerialized] public PlayerInteraction playerInteraction;
    [NonSerialized] public Rigidbody rb;
    [NonSerialized] public Renderer objectRenderer;
    [NonSerialized] public CharacterController charController;

    [Header("Particle References")]
    [Tooltip("These particles show up when object is selected.")]
    public ParticleSystem ghostParticles;
    [Tooltip("These particles also show up when object is selected.")]
    public ParticleSystem secondaryParticles;
    [Tooltip("These particles show up when object is hovered over.")]
    public ParticleSystem hoverParticles;
    [Tooltip("These particles show up when object is placed.")]
    public ParticleSystem placeParticles;

    [Tooltip("This gameObject is activated whenever object is selected. This gameobject should contain multiple particles.")]
    public GameObject floatingParticles;
    [NonSerialized] public ParticleSystem[] floatingParticleSystems;
    
    [Header("AUdio cues")]
    public AudioClip pickUp;
    public AudioClip putDown;
    public AudioClip pushed;

    [NonSerialized] public bool isMoving = false;

    public Vector3 newDirection;
    public Vector3 edgeOfObject;

    //[NonSerialized] public Coroutine moveCoroutine = null;
    [NonSerialized] public bool isInteractive = true;

    [NonSerialized] public AudioManager sfx_AM;

    private void OnValidate()
    {
        if (layerableObjects.Count == 0) // Set a default layerableObjects with the gameObject this script is attached to (do NOT programmatically attach child objects here!)
            layerableObjects = new List<GameObject> { gameObject };
        else if (!layerableObjects.Contains(gameObject)) // Readd attached gameObject if it is removed from list
            layerableObjects.Insert(0, gameObject);

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
        minRayOffset = rayOffset; // Update min ray offset for gizmo

        CacheComponents();
    }


    private void CacheComponents()
    {
        // Automatically cache components
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (charController == null) charController = GetComponent<CharacterController>();
        if (playerInteraction == null) playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (oi == null) oi = GetComponent<ObjectInteractions>();

        if (materialObj != null) objectRenderer = materialObj.GetComponent<Renderer>();
        else
        {
            objectRenderer = null;
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

        originalColors = new UnityEngine.Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].sharedMaterial; // or .material if you want a unique instance
            if (mat != null && mat.HasProperty("_OutlineColour"))
            {
                originalColors[i] = mat.GetColor("_OutlineColour");
            }
            else
            {
                originalColors[i] = UnityEngine.Color.clear; // or some default value
            }
        }
        offsetPlug = gameObject.transform.Find("Offset Plug");

    }

    void OnDrawGizmos()
    {
        // Draw sphere of raycastoffset
        Gizmos.color = new UnityEngine.Color(0f, 1f, 0f, 0.1f); // Set the color of the Gizmo to Green with custom alpha
        Gizmos.DrawSphere(transform.position, minRayOffset);
        // Draw a wire sphere outline.
        Gizmos.color = UnityEngine.Color.white;
        Gizmos.DrawWireSphere(transform.position, minRayOffset);
    }


    private void Awake()
    {
        CacheComponents();

        layerWhenUnselected = LayerMask.LayerToName(gameObject.layer);

        minRayOffset = rayOffset; //Update minimum RayOffset to match inspector

        sfx_AM = FindObjectOfType<AudioManager>();
        if (sfx_AM == null)
        {
            Debug.Log("No audio manager");
        }
        floatingParticleSystems = floatingParticles.GetComponentsInChildren<ParticleSystem>(true);
        ToggleParticles();

        edgeOfObject = objectRenderer != null ? objectRenderer.localBounds.extents * transform.localScale.magnitude : new Vector3(1, 1, 1) * transform.localScale.magnitude;

        _rotationOffset = Quaternion.Euler(rotationOffset);

        //if (floatingParticles != null) floatingParticles.SetActive(false);
    }

    private void Start()
    {
        ChangeState(idleState);

    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }

        if (debugText != null)
        {
            debugText.text = currentState.ToString();
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

    // This function checks the current state. If it is a state in which the object is considered placed and thus "complete", this returns true.
    public bool IsComplete()
    {
        return (currentState == poppedState || currentState == dunkedState);
    }

    public void OnClick()
    {
        Debug.Log("Please run OnClick"); //Debug
        currentState.OnClick();
    }

    public void OnRelease()
    {
        Debug.Log("Please run OnRelease"); //Debug
        currentState.OnRelease();
    }

    public void OnLeftClick()
    {
        Debug.Log("Please run OnLeftClick"); //Debug
        currentState.OnStateLeftClick();
    }
    public void OnRightClick()
    {
        Debug.Log("Please run OnRightClick"); //Debug
        currentState.OnStateRightClick();
    }

    public void OnHoverEnter()
    {
        Debug.Log("Please run OnHoverEnter"); //Debug
        currentState.OnHoverEnter(); // Call script of current state
    }
    public void OnHoverExit()
    {
        Debug.Log("Please run OnHoverExit"); //Debug
        currentState.OnHoverExit(); // Call script of current state
    }

    public void ToggleParticles(string mode = "", bool clear = false)
    {
        mode = mode.ToUpper(); // Small failsafe for formatting differences

        if (mode == "PLACE") // For playing particles when placing object
        {
            if (placeParticles != null) placeParticles.Play();
            return;
        }

        if (mode == "FLOAT") // For playing particles when object is floating
        {
            foreach (var particle in floatingParticleSystems) particle.Play();
            if (ghostParticles != null) ghostParticles.Play();
            if (secondaryParticles != null) secondaryParticles.Play();
        } else {
            foreach (var particle in floatingParticleSystems)
            {
                particle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
                if (clear) particle.Clear();
            }
            if (ghostParticles != null) ghostParticles.Stop();
            if (secondaryParticles != null) secondaryParticles.Stop();
        }

        if (mode == "HOVER") // For playing particles when hovered over with cursor
        {
            if (hoverParticles != null) hoverParticles.Play();
        } else {
            if (hoverParticles != null) hoverParticles.Stop();
        }
    }

    public void SetCollidersAsTrigger(bool isTrigger)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.isTrigger = isTrigger;
        }
    }

    public void SetNewLayer(string layerName)
    {
        foreach (GameObject component in layerableObjects)
        {
            component.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public IEnumerator HaltInteractions(float waitTime)
    {
        isInteractive = false;

        yield return new WaitForSeconds(waitTime);

        isInteractive = true;
    }

    // This function is called by BasketballHoop.cs to push an object into a placement spot.
    public void PushObject(PlacementSpot newPlacementSpot)
    {
        ps = newPlacementSpot;
        ChangeState(pushedState);
    }

    // States cannot use MonoBehaviour functions, including Destroy(). DoDestroy() provides a method to run the Destroy() function in states.
    public void DoDestroy()
    {
        Destroy(this);
    }
    public void DoDestroy(GameObject gameobject)
    {
        Destroy(gameobject);
    }
}

public abstract class State
{
    public InteractableStateController sc;

    public void OnStateEnter(InteractableStateController stateController)
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
        if (Input.GetMouseButtonDown(0))
        {
            OnStateLeftClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnStateRightClick();
        }

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

    public void OnStateLeftClick()
    {
        // Code placed here will always run
        OnLeftClick();
    }
    protected virtual void OnLeftClick()
    {
        // Code placed here can be overridden
    }

    public void OnStateRightClick()
    {
        // Code placed here will always run
        OnRightClick();
    }

    protected virtual void OnRightClick()
    {
        // Code placed here can be overridden
        sc.ChangeState(sc.idleState); // Deselect item
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






    // This is run by PlayerInteraction.cs to select the interactable
    public virtual void OnClick()
    {
        Debug.Log("State Controller: Running OnClick");
    }

    // This is run by PlayerInteraction.cs to deselect the interactable
    public virtual void OnRelease()
    {
        Debug.Log("State Controller: Running OnRelease");
    }
    public virtual void OnHoverEnter()
    {
        Debug.Log("StateController: Hovering over object and playerInteraction.isHolding = " + sc.playerInteraction.isHolding); //Debug
        
        if (!sc.playerInteraction.itemHeld) // Check if no valid item is being held
        {
            sc.isHovered = true;

            HighlightObject();
            //CursorScript.instance.UpdateCursor("Interact");
            if (sc.hoverParticles != null) sc.hoverParticles.Play();
        }
    }

    public virtual void OnHoverExit()
    {
        Debug.Log("StateController: No longer hovering over object"); //Debug
        sc.isHovered = false;

        if (sc.hoverParticles != null) sc.hoverParticles.Stop();

        if (sc.playerInteraction.itemHeld == sc)
        {
            //CursorScript.instance.UpdateCursor("Default");
        }
        else if (!sc.playerInteraction.itemHeld) // Check if no valid item is being held
        {
            UnhighlightObject();
            //CursorScript.instance.UpdateCursor("Default");
        }
    }

    private void HighlightObject()
    {
        Debug.Log("StateController: Highlighting object"); //Debug
        foreach (var rend in sc.renderers)
        {
            var mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", sc.hoverColor);
            rend.SetPropertyBlock(mpb);
        }
    }

    private void UnhighlightObject()
    {
        Debug.Log("StateController: Un-highlighting object"); //Debug
        for (int i = 0; i < sc.renderers.Length; i++)
        {
            var mpb = new MaterialPropertyBlock();
            var rend = sc.renderers[i];
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", sc.originalColors[i]);
            rend.SetPropertyBlock(mpb);
        }
    }

}