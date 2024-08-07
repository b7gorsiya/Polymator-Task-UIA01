using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerCustom : MediaPlayer
{
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoFinished; // Subscribe to event
    }

    public override void Play()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    public override void Stop()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        NotifyPlaybackCompleted();
    }
}
