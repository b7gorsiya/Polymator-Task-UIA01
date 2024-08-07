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
    // Method to indicate download start
    public void OnDownloadStarted() => this.gameObject.SetActive(true);

    // Method to indicate download finished
    public void OnDownloadFinished() => this.gameObject.SetActive(false);
}
