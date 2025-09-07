using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDayButton : MonoBehaviour
{
    public DialogueScript dialogueManager; // Drag in inspector or find in Start()



    void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueScript>();
        }

    }


    public void EndDay()
    {
        Debug.Log("Day is ending");
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
