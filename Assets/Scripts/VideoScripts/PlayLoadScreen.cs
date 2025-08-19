using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayLoadScreen : MonoBehaviour
{
    VideoPlayer videoPlayer;
    public GameObject hud;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        hud.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        videoPlayer.loopPointReached += OnLoopPointReached;
    }

    void OnLoopPointReached (VideoPlayer source)
    {
        Debug.Log("video ended");
        videoPlayer.Stop();
        hud.SetActive(true);
    }

}
