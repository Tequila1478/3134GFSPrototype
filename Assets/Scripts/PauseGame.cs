using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public bool isPaused = false;
    public bool isDialogue = false;

    public Animator pauseAnimator;
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject HUDScreen;

    void Start()
    {
        if (pauseAnimator != null)
        {
            // Make sure the animator ignores Time.timeScale
            pauseAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            pauseAnimator.SetBool("isPaused", false);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!isDialogue)
            {
                if (isPaused) EndPause();
                else StartPause();
            }
        }
    }

    private void StartPause()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        HUDScreen.SetActive(false);

        isPaused = true;

        if (pauseAnimator != null)
            pauseAnimator.SetBool("isPaused", true);
    }

    public void EndPause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        HUDScreen.SetActive(true);

        isPaused = false;

        if (pauseAnimator != null)
            pauseAnimator.SetBool("isPaused", false);
    }
}