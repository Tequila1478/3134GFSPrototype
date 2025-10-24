using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public static CursorScript instance;

    [Header("Config")]
    public int numberOfCursorTextures = 1;
    public CursorMode cursorMode = CursorMode.Auto;
    [Tooltip("Layers that contain interactable objects. Will always be counted, regardless of whether an object is held. Layers from onHoldLayer should also be included here.")]
    public LayerMask interactionLayer;
    [Tooltip("Layers that contain interactable objects. Layers put here are only counted while an interactable object is being held. The Basketball Hoop layer should be included here.")]
    public LayerMask onHoldLayer;
    [Tooltip("Layers that contain interactable objects. Layers put here are only counted while an interactable object is NOT being held.")]
    public LayerMask offHoldLayer;
    [Header("Components")]
    public List<Texture2D> cursorTexture;
    public List<string> cursorName = new List<string> { "Default" };
    public List<Vector2> cursorHotspot = new List<Vector2> { Vector2.zero };

    private PauseGame pg;
    private PlayerInteraction playerInteraction;

    private void OnValidate()
    {
        int change;

        if (numberOfCursorTextures < 1) numberOfCursorTextures = 1; // There must always be at least one cursor texture

        //Update arrays to match the configured number of cursor textures
        change = numberOfCursorTextures - cursorTexture.Count;
        if (change > 0)
        {
            var addthis = new Texture2D[change];
            Debug.Log("Added " + addthis.Length + " entries to cursorTexture");
            cursorTexture.AddRange(addthis);
        }
        else if (change < 0)
        {
            Debug.Log("Removed " + change + " entries from cursorTexture");
            cursorTexture.RemoveRange(numberOfCursorTextures, Mathf.Abs(change));
        }

        change = numberOfCursorTextures - cursorName.Count;
        if (change > 0)
        {
            var addthis = new string[change];
            Debug.Log("Added " + addthis.Length + " entries to cursorName");
            cursorName.AddRange(addthis);
        }
        else if (change < 0)
        {
            Debug.Log("Removed " + change + " entries from cursorName");
            cursorName.RemoveRange(numberOfCursorTextures, Mathf.Abs(change));
        }

        change = numberOfCursorTextures - cursorHotspot.Count;
        if (change > 0)
        {
            var addthis = new Vector2[change];
            Debug.Log("Added " + addthis.Length + " entries to cursorHotspot");
            cursorHotspot.AddRange(addthis);
        }
        else if (change < 0)
        {
            Debug.Log("Removed " + change + " entries from cursorHotspot");
            cursorHotspot.RemoveRange(numberOfCursorTextures, Mathf.Abs(change));
        }

        cursorName[0] = "Default"; //First cursor name should always be Default


        CacheComponents();
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        CacheComponents();
    }

    private void CacheComponents()
    {
        if (playerInteraction == null) playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (pg == null) pg = FindObjectOfType<PauseGame>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCursor(0); // Set up default cursor
    }

    private void FixedUpdate()
    {
        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1) return;
        if (!pg || pg.pauseState != PauseGame.PauseState.Paused) // If not paused (or no pause menu exists)
        {
            ProcessCursorUpdate();
        }
    }

    public void ProcessCursorUpdate()
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, interactionLayer))
        {
            var obj = hit.transform.gameObject; // Get gameobject that was hit by raycast
            if (obj)
            {
                int thisObjMask = 1 << obj.layer; // Create bitshift for comparing gameobject's layer to layermasks
                if ((onHoldLayer & thisObjMask) != 0) // Check if object's layer affects cursor while item is held; Compare bits of layermask and object's layer, continue if layer is in onHoldLayer layermask
                {
                    bool b;
                    var ps = obj.GetComponent<BasketballHoop>().placementSpot; //Get placement spot if specifically hovering over basketball hoop

                    if (ps && playerInteraction.itemHeld) // Do if placement spot and a held item exist
                    {
                        b = (ps.spotType.ToString() == playerInteraction.itemHeld.taskType); // Basketball hoop behaviour; Set bool based on if placement spot type matches held item type
                    }
                    else
                    {
                        b = (bool)playerInteraction.itemHeld; // Default behaviour; Set bool based on whether an item is held
                    }
                    UpdateCursor(b ? 1 : 0); // Set cursor based on resulting bool
                    return;
                }
                if ((offHoldLayer & thisObjMask) != 0) // Check if object's layer affects cursor while item is NOT held; Compare bits of layermask and object's layer, continue if layer is in offHoldLayer layermask
                {
                    bool b;

                    b = (bool)playerInteraction.itemHeld;

                    UpdateCursor(b ? 0 : 1); // Set cursor based on resulting bool
                    return;
                }
            }

            UpdateCursor(1); // Set cursor to interaction if any raycast was found
        }
        else
        {
            UpdateCursor(0); // Set cursor to default if no raycast found
        }
    }

    public void ResetCursor()
    {
        UpdateCursor(0);
    }
    public void UpdateCursor(string newName)
    {
        int newIndex = cursorName.IndexOf(newName); // Find the index of the given cursor name
        UpdateCursor(newIndex); // Update cursor for found index
    }

    public void UpdateCursor(int newIndex)
    {
        if (newIndex < 0) // Override if index is non-existent
        {
            Debug.LogWarning("Attempted to update cursor to negative index or non-existent name. Setting cursor to default.");
            Cursor.SetCursor(cursorTexture[0], cursorHotspot[0], cursorMode);
            return;
        }
        if (newIndex >= numberOfCursorTextures) // Override if index is invalid
        {
            Debug.LogWarning("Attempted to update cursor to index outside of list size. Setting cursor to default.");
            Cursor.SetCursor(cursorTexture[0], cursorHotspot[0], cursorMode);
            return;
        }

        // Update cursor to new texture with its hotspot, and use the preset cursor mode
        Cursor.SetCursor(cursorTexture[newIndex], cursorHotspot[newIndex], cursorMode);
    }
}
