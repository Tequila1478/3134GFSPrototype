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
        dialogueManager = FindObjectOfType<DialogueScript>();
        Debug.Log(dialogueManager.foundDivorcePapers);
        Debug.Log("Day is ending");
        if (dialogueManager != null)
        {
            dialogueManager.StartEndDay();
        }
        else
        {
            Debug.LogWarning("DayDialogueManager not assigned or found.");
        }
    }

}
