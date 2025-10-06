using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FinishDayObject : MonoBehaviour, IHoverable, IClickable
{
    public DialogueScript dialogueManager; // Drag in inspector or find in Start()

    public bool isHovered = false;
    public PlayerInteraction pi;
    public CustomCursor cursor;
    public Material outlineMat;
    public Material originalMat;
    private Renderer objectRenderer;


    void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueScript>();
        }

        if(pi == null)
        {
            pi = FindObjectOfType<PlayerInteraction>();
        }
        cursor = FindObjectOfType<CustomCursor>();

        objectRenderer = GetComponent<Renderer>();

        if (outlineMat != null && objectRenderer != null)
        {
            outlineMat.SetTexture("_MainTex", objectRenderer.material.mainTexture);
        }

    }

    void OnMouseDown()
    {
        
    }

    public void OnHoverEnter()
    {
        if (isHovered) return;
        isHovered = true;

        if (!pi.isHolding)
        {
            HighlightObject();
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
        }
        if (pi.itemHeld == this)
        {
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
        }
    }

    public void OnHoverExit()
    {
        if (!isHovered) return;
        isHovered = false;

        if (!pi.isHolding)
        {
            UnhighlightObject();
            //cursor?.ChangeVisual(0);
            CursorScript.instance.UpdateCursor("Default");
        }
        if (pi.itemHeld == this)
        {
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
        }
    }

    public void OnClick()
    {
        if (dialogueManager != null)
        {
            StartCoroutine(dialogueManager.EndDay());
        }
        else
        {
            Debug.LogWarning("DayDialogueManager not assigned or found.");
        }
    }

    private void HighlightObject()
    {
        if (outlineMat != null && objectRenderer != null)
        {
            outlineMat.SetTexture("_MainTex", objectRenderer.material.mainTexture);
            objectRenderer.material = outlineMat;
        }
    }

    public void OnRelease()
    {
        
    }

    private void UnhighlightObject()
    {
        if (originalMat != null && objectRenderer != null)
        {
            objectRenderer.material = originalMat;
        }
    }
}
