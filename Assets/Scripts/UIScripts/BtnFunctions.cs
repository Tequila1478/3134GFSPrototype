using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnFunctions : MonoBehaviour
{
    public bool playAudioOnClick;

    public GameObject nextUI;
    public GameObject previousUI;
    public string sceneToLoad;

    public GameObject triggerUI;

    public Slider slider;
    public Slider[] sliders;
    private AudioManager audio_AM;
    public bool isMusic;
    public bool isSFX;
    public bool isDialogue;

    public Animator animator;
    public string animationBoolName;
    public bool startingState = true;

    public AudioClip onClickAudio;

    // Start is called before the first frame update --> awake is called before start when the object is initialised
    void Awake()
    {
        audio_AM = FindObjectOfType<AudioManager>();

        if (slider != null)
        {
            if (isMusic) slider.value = audio_AM.musicVol;
            else if (isSFX) slider.value = audio_AM.sfxVol;
            else if (isDialogue) slider.value = audio_AM.dialogueVol;

            else Debug.Log("Please assign audio type in inspector for object " + this.gameObject);

            Debug.Log(slider.value);
            Debug.Log(audio_AM.musicVol);
        }
    }

    public void SetMusicVolumeOnSlider()
    {
        audio_AM.SetMusicVolume(slider.value);
    }

    public void SetSFXVolumeOnSlider()
    {
        audio_AM.SetSFXVolume(slider.value);
    }

    public void GoToNextUIOnClick()
    {
        if(previousUI != null) previousUI?.SetActive(false);
        if(nextUI != null) nextUI.SetActive(true);

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void LoadSceneOnClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
        audio_AM.NextMusicClip();
        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void QuitOnClick()
    {
        Application.Quit();

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void ResetDataOnClick()
    {
        // Reset volume
        audio_AM.SetMusicVolume(audio_AM.startingMusicVol);
        audio_AM.SetSFXVolume(audio_AM.startingSfxVol);

        // Reset sliders
        foreach (Slider s in sliders)
        {
            if (s != null)
            {
                BtnFunctions btnf = s.GetComponent<BtnFunctions>(); // Must have the slider's btnfunctions to work
                if (btnf != null)
                { // Set slider value by checking the setting type
                    if (btnf.isMusic) s.value = audio_AM.startingMusicVol;
                    else if (btnf.isSFX) s.value = audio_AM.startingSfxVol;
                    else if (btnf.isDialogue) s.value = audio_AM.startingDialogueVol;
                }
            }
        }

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void FullscreenToggleOnClick()
    {
        Screen.fullScreen = !Screen.fullScreen;

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void PlayAudioOnSliderRelease()
    {
        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void ResumeGame()
    {
        FindObjectOfType<PauseGame>().EndPause();

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void PauseGame()
    {
        FindObjectOfType<PauseGame>().StartPause();

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void DisableUI()
    {
        triggerUI?.SetActive(false);
        Debug.Log("Disabling UI");
    }

    public void EnableUI()
    {
        triggerUI?.SetActive(true);
    }

    public void SetAnimationBool()
    {
        startingState = !startingState;
        animator.SetBool(animationBoolName, startingState);
    }
}
