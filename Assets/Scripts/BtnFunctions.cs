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
    private AudioManager audio_AM;
    public bool isMusic;
    public bool isSFX;
    public bool isDialogue;

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
        nextUI.SetActive(true);

        if (playAudioOnClick)
        {
            audio_AM.PlaySFX(onClickAudio);
        }
    }

    public void LoadSceneOnClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);

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
        Debug.Log("Functionality needed");

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
}
