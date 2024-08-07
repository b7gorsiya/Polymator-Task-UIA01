using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMediaPlayer
{
    GameObject gameObject { get; }
    Transform transform { get; }
    RectTransform GetComponent<RectTransform>();
    void Play();  // Start playback
    void Stop();  // Stop playback
    event System.Action OnPlaybackCompleted;  // Event to signal completion
}
public abstract class MediaPlayer : MonoBehaviour,IMediaPlayer
{
    public event System.Action OnPlaybackCompleted;  // Event to signal playback completion
    public abstract void Play();  // Abstract method for starting playback
    public abstract void Stop();  // Abstract method for stopping playback

    // Method to call when playback is complete
    protected virtual void NotifyPlaybackCompleted()
    {
        OnPlaybackCompleted?.Invoke();  // Trigger the completion event
        gameObject.SetActive(false);    // Disable the GameObject
    }
}
