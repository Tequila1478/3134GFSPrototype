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
    public GameObject howToPlayScreen;

    public AudioClip startPauseSFX;
    public AudioClip endPauseSFX;

    private CameraCinemaSwitch css;
    private AudioManager sfx_AM;

    void Start()
    {
        if (pauseAnimator != null)
        {
            // Make sure the animator ignores Time.timeScale
            pauseAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            pauseAnimator.SetBool("isPaused", false);
        }
        css = FindObjectOfType<CameraCinemaSwitch>();
        sfx_AM = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!isDialogue)
            {
                if (css.currentSpecialCamera != -1)
                {
                    css.LeaveSpecialCamera();
                    return;
                }
                if (isPaused) EndPause();
                else StartPause();
            }
        }
    }

    public void StartPause()
    {
        Time.timeScale = 0;

        if (pauseAnimator != null)
            pauseAnimator.SetBool("isPaused", true);

        if (startPauseSFX != null)
        {
            sfx_AM.PlaySFX(startPauseSFX);
        }

        isPaused = true;
    }

    public void SetHUDActive(bool active)
    {
        HUDScreen.SetActive(active);
    }

    public void SetPauseMenuActive(bool active)
    {
        pauseScreen.SetActive(active);
        settingsScreen.SetActive(false);
        howToPlayScreen.SetActive(false);
    }

    public void EndPause()
    {
        Time.timeScale = 1;

        if (pauseAnimator != null)
            pauseAnimator.SetBool("isPaused", false);

        if (endPauseSFX != null)
        {
            sfx_AM.PlaySFX(endPauseSFX);
        }

        isPaused = false;
    }
}