using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDayObject : MonoBehaviour
{
    public DialogueScript dialogueManager; // Drag in inspector or find in Start()

    void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueScript>();
        }
    }

    void OnMouseDown()
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
}
