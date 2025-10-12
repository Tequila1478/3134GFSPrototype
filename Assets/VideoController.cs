using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    public int[] pauseFrames;
    public bool paused;
    public VideoPlayer videoPlayer;
    public GameObject nextButton;
    public GameObject skipButton;
    public GameObject nextSceneButton;
    private int pauseNumber = 0;

    private void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
    void Update()
    {
        // Ensure the video is playing and we haven't paused yet
        if (videoPlayer.isPlaying && !paused)
        {
            if (videoPlayer.frame >= pauseFrames[pauseNumber])
            {
                videoPlayer.Pause();
                paused = true; // Set flag to true after pausing
                Debug.Log("Video paused at frame: " + videoPlayer.frame);
                pauseNumber++;
                nextButton.SetActive(true);
                skipButton.SetActive(false);
            }
        }

        // Use spacebar to advance
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (nextButton.activeSelf)
            {
                Continue();
            }
            else if (nextSceneButton.activeSelf)
            {
                LoadSceneFirstDay();
            }
            else if (skipButton.activeSelf)
            {
                SkipToNext();
            }
        }
    }

    public void Continue()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(true);
        videoPlayer.Play();
        paused = false;
    }

    public void SkipToNext()
    {
        videoPlayer.frame = pauseFrames[pauseNumber] - 1; // This skips to the next pause frame. Update() then handles pausing like usual.
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        nextSceneButton.SetActive(true);
    }

    public void LoadSceneFirstDay()
    {
        SceneManager.LoadScene("Scene_FirstDay");
    }

}
