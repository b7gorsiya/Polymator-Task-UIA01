using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

public class DownloadMediaHandler : MonoBehaviour
{
    [SerializeField] private string audioAddress;
    [SerializeField] private string videoAddress;
    [SerializeField] ProgressBar downloadProgress;

    public static event Action<string,object> OnDownloadComplete; // Event for download completion

    internal void DownloadMedia(int type) // 1 for video, 2 for audio
    {
        string assetType = (type == 1) ? "Video" : "Audio";
        string address = (type == 1) ? videoAddress : audioAddress;
        StartCoroutine(DownloadAsset(address, assetType));
    }

    private IEnumerator DownloadAsset(string address, string assetType)
    {
        // Determine asset type for handling different download cases
        AsyncOperationHandle downloadHandle;
        if (assetType == "Audio")
        {
            downloadHandle = Addressables.LoadAssetAsync<AudioClip>(address);
        }
        else
        {
            downloadHandle = Addressables.LoadAssetAsync<VideoClip>(address); // Use VideoClip for videos
        }
        downloadProgress.OnDownloadStarted();
        // Wait until the download is complete
        while (!downloadHandle.IsDone)
        {
            float progress = downloadHandle.PercentComplete;
            downloadProgress.UpdateProgressBar(progress, assetType);
            yield return null;
        }
        downloadProgress.OnDownloadFinished();

        // Check the download status
        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"{assetType} downloaded successfully.");
            object downloadedAsset = downloadHandle.Result;
            OnDownloadComplete?.Invoke(assetType,downloadedAsset);
        }
        else
        {
            Debug.LogError($"Failed to download {assetType}: {downloadHandle.OperationException}");
        }

        // Release the download handle only if it was successful
        Addressables.Release(downloadHandle);
    }

    private IEnumerator DownloadAssetRemote(string address, string assetType)
    {
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(address, true);

        while (!downloadHandle.IsDone)
        {
            float progress = downloadHandle.PercentComplete;
            downloadProgress.UpdateProgressBar(progress, assetType);
            yield return null;
        }

        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"{assetType} downloaded successfully.");
        }
        else
        {
            Debug.LogError($"Failed to download {assetType}: {downloadHandle.OperationException}");
        }

        Addressables.Release(downloadHandle);
    }
}
