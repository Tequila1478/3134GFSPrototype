using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlaySettingsScreen : MonoBehaviour
{
    VideoPlayer videoPlayer;
    public GameObject hud;
    public VideoClip transition;
    public VideoClip loop;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.clip = transition;
    }

    // Update is called once per frame
    void Update()
    {
        videoPlayer.loopPointReached += OnLoopPointReached;
    }

    void OnLoopPointReached(VideoPlayer source)
    {
        Debug.Log("video ended");
        if (videoPlayer.clip == transition && b)
        {
            videoPlayer.clip = loop;
        }
    }
}
