using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectItem : MonoBehaviour, IClickable, IHoverable
{
    public bool isHovered;
    public CustomCursor cursor;
    private Renderer objectRenderer;

    public DialogueScript dialogueScript;

    [Header("Materials")]
    public Material outlineMat;
    public Material originalMat;
    public GameObject materialObj;
    public GameObject visualisationObj;

    public GameObject info;

    private DialogueScript ds;



    private PlayerInteraction playerInteraction;


    public List<DialogueLine> inspectionDialogue;

    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        cursor = FindObjectOfType<CustomCursor>();
        objectRenderer = materialObj.GetComponent<Renderer>();
        ds = FindObjectOfType<DialogueScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        StartCoroutine(ShowInfo());
        //Show info and dialogue on counter
        //UPdate state

        if (dialogueScript != null)
        {
            dialogueScript.foundDivorcePapers = true;
        }

    }

    public IEnumerator ShowInfo()
    {
        info.SetActive(true);
        ds.PlayDialogueList(inspectionDialogue);
        yield return new WaitForSeconds(2);
        info.SetActive(false);


    }

    public void OnRelease()
    {
        //throw new System.NotImplementedException();
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
        //throw new System.NotImplementedException();
    }

    private void HighlightObject()
    {
        if (outlineMat != null && objectRenderer != null)
        {
            outlineMat.SetTexture("_Texture2D", objectRenderer.material.mainTexture);
            objectRenderer.material = outlineMat;
        }
    }

    private void UnhighlightObject()
    {
        if (originalMat != null && objectRenderer != null)
        {
            objectRenderer.material = originalMat;
        }
    }
}
