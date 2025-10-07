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
            }
        }
    }

    public void Continue()
    {        
        nextButton.SetActive(false);
        videoPlayer.Play();
        paused = false;
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
