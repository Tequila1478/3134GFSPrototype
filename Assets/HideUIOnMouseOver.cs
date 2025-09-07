using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUIOnMouseOver : MonoBehaviour
{

    [SerializeField] private GameObject triggerUI;
    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (triggerUI != null)
        {
            canvasGroup = triggerUI.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = triggerUI.AddComponent<CanvasGroup>();
            }

            // Ensure starting state is consistent
            if (!triggerUI.activeSelf)
                canvasGroup.alpha = 0f;
        }
    }

    public void EnableUI()
    {
        if (triggerUI == null) return;

        triggerUI.SetActive(true);

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(1f));
    }

    public void DisableUI()
    {
        if (triggerUI == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(0f));
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (Mathf.Approximately(targetAlpha, 0f))
        {
            // After fade-out, fully disable object
            triggerUI.SetActive(false);
        }

        fadeCoroutine = null; // mark coroutine finished
    }
}
