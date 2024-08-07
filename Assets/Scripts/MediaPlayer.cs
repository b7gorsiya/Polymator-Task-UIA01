using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMediaPlayer
{
    GameObject gameObject { get; }
    Transform transform { get; }
    RectTransform GetComponent<RectTransform>();
    void Play(); 
    void Stop();  
    event System.Action OnPlaybackCompleted; 
}
public abstract class MediaPlayer : MonoBehaviour,IMediaPlayer
{
    public event System.Action OnPlaybackCompleted;  
    public abstract void Play();  
    public abstract void Stop();  

    protected virtual void NotifyPlaybackCompleted()
    {
        OnPlaybackCompleted?.Invoke();  
        gameObject.SetActive(false);    
    }
}
