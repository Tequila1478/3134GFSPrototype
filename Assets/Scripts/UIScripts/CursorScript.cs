using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public static CursorScript instance;

    [Header("Config")]
    public int numberOfCursorTextures = 1;
    public CursorMode cursorMode = CursorMode.Auto;
    [Header("Cursors")]
    public List<Texture2D> cursorTexture;
    public List<string> cursorName = new List<string> { "Default" };
    public List<Vector2> cursorHotspot = new List<Vector2> { Vector2.zero };

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
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCursor(0); // Set up default cursor
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
            Debug.LogError("Attempted to update cursor to non-existent index or name. Will ignore.");
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
