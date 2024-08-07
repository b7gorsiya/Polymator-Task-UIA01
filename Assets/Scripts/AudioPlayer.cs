using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MediaPlayer
{
    private AudioSource audioSource;
    private bool isPlaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if audio finished playing
        if (isPlaying && !audioSource.isPlaying)
        {
            isPlaying = false;
            NotifyPlaybackCompleted();
        }
    }

    public override void Play()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            isPlaying = true;
        }
    }

    public override void Stop()
    {
        if (audioSource != null && isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
        }
    }
}
