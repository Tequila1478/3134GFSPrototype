using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Image;

public class PauseGame : MonoBehaviour
{
    //public bool isPaused = false;
    public enum PauseState
    {
        Playing,
        Resuming,
        Pausing,
        Paused
    }
    public PauseState pauseState;
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

    public UnityEvent pauseListeners = new UnityEvent();

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

    public void AddActionAsListener(UnityAction action, string name, bool persistent = false)
    {
        Debug.Log("Adding action " + name + " as listener, persistent = " + persistent);
        if (persistent)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(pauseListeners, action);
        }
        else
        {
            pauseListeners.AddListener(action);
        }
    }
    public void RemoveActionAsListener(UnityAction action, string name, bool persistent = false)
    {
        Debug.Log("Removing action " + name + " as listener, persistent = " + persistent);
        if (persistent)
        {
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(pauseListeners, action);
        }
        else
        {
            pauseListeners.RemoveListener(action);
        }
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
                if (pauseState == PauseState.Playing) StartPause();
                else EndPause();
            }
        }
    }

    public void StartPause()
    {
        //Time.timeScale = 0;

        if (pauseAnimator != null)
            pauseAnimator.SetBool("isPaused", true);

        if (startPauseSFX != null)
        {
            sfx_AM.PlaySFX(startPauseSFX);
        }

        pauseState = PauseState.Pausing;
    }

    public void SetHUDActive(bool active)
    {
        HUDScreen.SetActive(active);
    }

    public void SetPauseMenuActive(bool active)
    {
        Time.timeScale = active ? 0 : 1;
        pauseScreen.SetActive(active);
        settingsScreen.SetActive(false);
        howToPlayScreen.SetActive(false);

        pauseState = PauseState.Paused;
        pauseListeners.Invoke(); //Invoke listeners
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

        pauseState = PauseState.Playing;
    }
}