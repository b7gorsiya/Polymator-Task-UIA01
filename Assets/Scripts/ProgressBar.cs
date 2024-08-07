using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

    internal void UpdateProgressBar(float progress, string assetType)
    {
        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }
    }
    public void OnDownloadStarted() => this.gameObject.SetActive(true);
    public void OnDownloadFinished() => this.gameObject.SetActive(false);
}
