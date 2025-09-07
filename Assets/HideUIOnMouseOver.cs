using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUIOnMouseOver : MonoBehaviour
{

    [SerializeField] private GameObject triggerUI;
    [SerializeField] private GameObject endDayButtonFade;

    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup canvasGroupFade;
    private CanvasGroup canvasGroupButton;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (triggerUI != null)
        {
            canvasGroupFade = triggerUI.GetComponent<CanvasGroup>();
            if (canvasGroupFade == null)
            {
                canvasGroupFade = triggerUI.AddComponent<CanvasGroup>();
            }

            // Ensure starting state is consistent
            if (!triggerUI.activeSelf)
                canvasGroupFade.alpha = 0f;
        }

        if (endDayButtonFade != null)
        {
            canvasGroupButton = endDayButtonFade.GetComponent<CanvasGroup>();
            if (canvasGroupButton == null)
            {
                canvasGroupButton = endDayButtonFade.AddComponent<CanvasGroup>();
            }

            // Ensure starting state is consistent
            if (!triggerUI.activeSelf)
                canvasGroupButton.alpha = 0f;
        }
    }

    public void EnableUI()
    {
        if (triggerUI == null) return;

        triggerUI.SetActive(true);

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(1f));

        if(endDayButtonFade != null)
        {
            endDayButtonFade.SetActive(true);
        }
    }

    public void DisableUI()
    {
        if (triggerUI == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(0f));
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroupFade.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            canvasGroupFade.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroupFade.alpha = targetAlpha;
        if(canvasGroupButton != null)
        {
            canvasGroupButton.alpha = targetAlpha;
        }

        if (Mathf.Approximately(targetAlpha, 0f))
        {
            // After fade-out, fully disable object
            triggerUI.SetActive(false);
            if(canvasGroupButton != null)
            {
                endDayButtonFade.SetActive(true);
            }
        }

        fadeCoroutine = null; // mark coroutine finished
    }
}
