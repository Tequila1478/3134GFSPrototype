using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DialogueScript : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText; 
    public TextMeshProUGUI headingText;
    public Image characterImage; 
    public RectTransform imageContainer;
    public float delayBetweenLines = 4f;

    public bool dialogueCompleted = false;
    public bool houseClean = true;
    public bool foundDivorcePapers = true;
    public GameObject hud;
    public CustomCursor cursor;
    public string loadNextScene;

    private float waitSystem;


    public List<DialogueLine> startDialogue;

    public List<DialogueLine> endDialogueGood;

    public List<DialogueLine> endDialogueBad;

    public List<DialogueLine> endDialogueFoundDivorcePapers;

    public void Start()
    {
    }

    public void StartDay()
    {
        hud.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(PlayDialogue(startDialogue));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CancelWait();
        }
    }

    public IEnumerator EndDay()
    {
        hud.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        if (houseClean)
        {
            yield return StartCoroutine(PlayDialogue(endDialogueGood));
        }
        else
        {
            yield return StartCoroutine(PlayDialogue(endDialogueBad));
        }

        //Wait for dialogue to finish and then:
        if (foundDivorcePapers)
        {
            yield return StartCoroutine(PlayDialogue(endDialogueFoundDivorcePapers));
        }

        Debug.Log("End of day sequence complete.");
        SceneManager.LoadScene(loadNextScene);
    }

    IEnumerator PlayDialogue(List<DialogueLine> lines)
    {
        dialogueText.gameObject.SetActive(true);
        headingText.gameObject.SetActive(true);
        
        characterImage.gameObject.SetActive(true);
        dialoguePanel.SetActive(true);


        foreach (DialogueLine line in lines)
        {
            dialogueText.text = line.text;
            headingText.text = line.heading;

            if(line.characterSprite == null) characterImage.gameObject.SetActive(false);
            else characterImage.gameObject.SetActive(true);
            characterImage.sprite = line.characterSprite;
            LayoutSprite(line.spriteOnRight);

            yield return StartCoroutine(WaitForSecondsOrTap(delayBetweenLines));
        }

        dialogueCompleted = true;
        dialogueText.gameObject.SetActive(false);
        headingText.gameObject.SetActive(false);
        characterImage.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);

        hud.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator WaitForSecondsOrTap(float seconds)
    {
        waitSystem = seconds;
        while (waitSystem > 0.0)
        {
            waitSystem -= Time.deltaTime;
            yield return 0;
        }
    }

    void OverrideWait(float newTime)
    {
        waitSystem = newTime;
    }

    void CancelWait()
    {
        waitSystem = 0.0f;
    }

    private void LayoutSprite(bool onRight)
    {
        imageContainer.anchorMin = new Vector2(onRight ? 1 : 0, 0.5f);
        imageContainer.anchorMax = new Vector2(onRight ? 1 : 0, 0.5f);
        imageContainer.pivot = new Vector2(onRight ? 1 : 0, 0.5f);
        imageContainer.anchoredPosition = new Vector2(onRight ? -50 : 100, 0);
    }

    public void PlayDialogueList(List<DialogueLine> lines)
    {
        StartCoroutine(PlayDialogue(lines));
    }
}
