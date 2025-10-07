using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectItem : MonoBehaviour, IClickable, IHoverable
{
    public bool isHovered;
    public CustomCursor cursor;
    private Renderer objectRenderer;
    public bool onlyOnce = false;

    public DialogueScript dialogueScript;

    [Header("Other stuff")]
    public GameObject info;

    private DialogueScript ds;

    public bool isDivorcePapers = false;
    public bool isPhoto = false;
    public Sprite inspectionImage;
    public bool displayInfoImage = true;
    public string inspectionText = "";

    private PlayerInteraction playerInteraction;


    public List<DialogueLine> inspectionDialogue;
    public List<DialogueLine> alternativeDialogue;

    public bool specialCameraReq = false;
    public bool active = false;
    public CinemachineVirtualCamera specialCamera;
    

    public AudioClip InteractNoise;

    public AudioManager audio_AM;

    [Header("Change Image Settings")]
    public bool changeImage = false;
    public Material newImage;
    public GameObject imageObj;
    public AudioClip imageChangeSound;

    private bool hasInteracted;

    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        cursor = FindObjectOfType<CustomCursor>();

        ds = FindObjectOfType<DialogueScript>();


        audio_AM = FindObjectOfType<AudioManager>();
    }


    void Update()
    {
        if (CameraCinemaSwitch.instance.currentSpecialCamera == CameraCinemaSwitch.FindIndexOfSpecialCamera(specialCamera))
        {
            active = true;
        }

        else
        {
            active = false;
        }
    }

    public void OnClick()
    {
        if ((onlyOnce && !hasInteracted) || !onlyOnce)
        {
            if (ds.isMonologuing) return; //Don't do stuff if dialogue is already monologuing
            if (specialCameraReq && !active) return;

            audio_AM.PlaySFX(InteractNoise);
            StartCoroutine(ShowInfo());
            //Show info and dialogue on counter
            //UPdate state

            if (dialogueScript != null && isDivorcePapers)
            {
                dialogueScript.foundDivorcePapers = true;
            }

            if (isPhoto)
            {
                dialogueScript.foundPhoto = true;
            }

            if (changeImage)
            {
                if (!hasInteracted)
                {
                    imageObj.GetComponent<MeshRenderer>().material = newImage;
                    audio_AM.PlaySFX(imageChangeSound);
                }
            }
        }
        hasInteracted = true;
    }

    public IEnumerator ShowInfo()
    {
        if (displayInfoImage)
        {
            info.SetActive(displayInfoImage);
            Transform spriteChild = info.transform.GetChild(0).GetChild(1);
            SpriteRenderer sr = spriteChild.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = inspectionImage;

            Transform textChild = info.transform.GetChild(0).GetChild(0);
            TMPro.TextMeshProUGUI tmp = textChild.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = inspectionText;
        }

        if (dialogueScript.foundDivorcePapers)
            ds.PlayDialogueList(alternativeDialogue);
        else
            ds.PlayDialogueList(inspectionDialogue);


        // Wait until dialogue finishes
        while (ds.isMonologuing)
        {
            yield return null; // wait a frame
        }

        info.SetActive(false);

        //yield return new WaitForSecondsRealtime(2);
        //info.SetActive(false);
    }

    public void OnRelease()
    {
        //throw new System.NotImplementedException();
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hovering over object");
        if (isHovered) return;

        if (!playerInteraction.isHolding)
        {
            HighlightObject();
            Debug.Log("hi"); ;
            //cursor?.ChangeVisual(1);
            CursorScript.instance.UpdateCursor("Interact");
            isHovered = true;
        }
    }

    public void OnHoverExit()
    {
        //throw new System.NotImplementedException();
    }

    private void HighlightObject()
    {

    }

    private void UnhighlightObject()
    {
    }
}
