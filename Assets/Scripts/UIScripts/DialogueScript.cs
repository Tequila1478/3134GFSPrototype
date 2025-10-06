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
    public TextMeshProUGUI skipReminderText;
    public Image characterImage; 
    public RectTransform imageContainer;
    public float delayBetweenLines = 4f;

    public bool dialogueCompleted = false;
    public bool houseClean = false;
    public bool foundDivorcePapers = true;
    public GameObject hud;
    public CustomCursor cursor;
    public string loadNextScene;
    public AudioClip nextDialogueSFX;
    public AudioClip[] characterSFX;

    private float waitSystem;
    private AudioManager audio_AM;

    private bool skipLine = false;
    private bool lineCompleted = false;

    public bool isMonologuing = false;


    public List<DialogueLine> startDialogue;

    public List<DialogueLine> endDialogueGood;

    public List<DialogueLine> endDialogueBad;

    public List<DialogueLine> endDialogueFoundDivorcePapers;

    public void Start()
    {
        audio_AM = FindObjectOfType<AudioManager>();
    }

    public void StartDay()
    {
        hud.SetActive(false);

        FindObjectOfType<PauseGame>().isDialogue = true;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(PlayDialogue(startDialogue));
    }

    private void Update()
    {
        // Skip dialogue by clicking or pressing space bar
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            CancelWait();
        }
    }

    public void StartEndDay()
    {
        StartCoroutine(EndDay());
    }

    public IEnumerator EndDay()
    {
        hud.SetActive(false);
        FindObjectOfType<PauseGame>().isDialogue = true;
        Cursor.lockState = CursorLockMode.Locked;

        if (houseClean)
        {
            Debug.Log("House clean");
            if(endDialogueGood != null)
            yield return StartCoroutine(PlayDialogue(endDialogueGood));

            else
            {
                Debug.Log("End dialogue good not set");
            }
        }
        else
        {
            Debug.Log("House not clean");
            if (endDialogueBad != null)
                yield return StartCoroutine(PlayDialogue(endDialogueBad));
            else
            {
                Debug.Log("End dialogue bad not set");
            }
        }
        Debug.Log("Reached after first dialogue sequence");
        //Wait for dialogue to finish and then:
        if (foundDivorcePapers)
        {
            Debug.Log("Divorce papers found");
            yield return StartCoroutine(PlayDialogue(endDialogueFoundDivorcePapers));
        }
        else
        {
            Debug.Log("Divorce papers not found");
        }

        Debug.Log("End of day sequence complete.");
        SceneManager.LoadScene(loadNextScene);
    }

    void OnDestroy()
    {
    }

    IEnumerator PlayDialogue(List<DialogueLine> lines)
    {
        isMonologuing = true;
        Time.timeScale = 0f;

        dialogueText.gameObject.SetActive(true);
        headingText.gameObject.SetActive(true);
        skipReminderText.gameObject.SetActive(true);
        dialoguePanel.SetActive(true);

        yield return null;

        foreach (DialogueLine line in lines)
        {
            skipLine = false;
            lineCompleted = false;

            headingText.text = line.heading;
            string newLine = line.text;
            bool startInNewLine = (headingText.text.Length > 0) ? true: false; // Dialogue text is shifted into next line if there is a heading present

            if(line.characterSprite == null) characterImage.gameObject.SetActive(false);
            else characterImage.gameObject.SetActive(true);

            if(line.characterSprite)
            characterImage.sprite = line.characterSprite;
            LayoutSprite(line.spriteOnRight);

            yield return StartCoroutine(TypeLine(newLine, startInNewLine));


        }

        dialogueCompleted = true;
        dialogueText.gameObject.SetActive(false);
        headingText.gameObject.SetActive(false);
        skipReminderText.gameObject.SetActive(false);
        characterImage.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);

        hud.SetActive(true);
        Time.timeScale = 1;

        FindObjectOfType<PauseGame>().isDialogue = false;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("PlayDialogue finished with " + lines.Count + " lines");
        isMonologuing = false;
    }

    public float textSpeed = 0.05f; // seconds per letter

    IEnumerator TypeLine(string lineText, bool startInNewLine)
    {
        //Check to see if there is a heading and if not add a break to the start
        dialogueText.text = startInNewLine ? "<br>" : string.Empty;

        //Start typing letter by letter
        for (int i = 0; i < lineText.Length; i++)
        {
            dialogueText.text += lineText[i];

            // Play a random audio clip for this character
            if (characterSFX != null && characterSFX.Length > 0)
            {
                AudioClip clip = characterSFX[Random.Range(0, characterSFX.Length)];
                audio_AM.PlaySFX(clip);
            }


            // Wait for a short delay before next letter
            float timer = 0f;
            while (timer < textSpeed)
            {
                if (skipLine)
                {
                    dialogueText.text = startInNewLine ? "<br>" + lineText : lineText;
                    skipLine = false;
                    goto AfterTyping; //Skips to the coroutine AfterTyping: section
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

    AfterTyping:
        // Wait for delay or click
        yield return StartCoroutine(WaitForSecondsOrTap(delayBetweenLines));

        // Optional SFX
        if (nextDialogueSFX != null)
            audio_AM.PlaySFX(nextDialogueSFX);
    }


    IEnumerator WaitForSecondsOrTap(float seconds)
    {
        waitSystem = seconds;
        while (waitSystem > 0.0f)
        {
            if (skipLine)
            {
                skipLine = false; // consume click
                break;
            }
            waitSystem -= Time.unscaledDeltaTime;
            yield return null;
        }
        waitSystem = 0;
    }

    void OverrideWait(float newTime)
    {
        waitSystem = newTime;
    }

    void CancelWait()
    {
        skipLine = true;
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
