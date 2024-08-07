using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;
[System.Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid) { }
}
[System.Serializable]
public class AssetReferenceVideoClip : AssetReferenceT<VideoClip>
{
    public AssetReferenceVideoClip(string guid) : base(guid) { }
}
public class DownloadMediaHandler : MonoBehaviour
{
    [SerializeField] private AssetReferenceAudioClip audioReference; 
    [SerializeField] private AssetReferenceVideoClip videoReference;
    [SerializeField] ProgressBar downloadProgress;

    public static event Action<string,object> OnDownloadComplete; // Event for download completion

    internal void DownloadMedia(int type) // 1 for video, 2 for audio
    {
        AssetReference assetReference = (type == 1) ? videoReference : audioReference;
        string assetType = (type == 1) ? "Video" : "Audio";
        StartCoroutine(DownloadAsset(assetReference, assetType));
    }

    private IEnumerator DownloadAsset(AssetReference assetReference, string assetType)
    {
        if (!assetReference.RuntimeKeyIsValid())
        {
            Debug.LogError($"{assetType} reference is not valid.");
            yield break;
        }
        // Determine asset type for handling different download cases
        AsyncOperationHandle downloadHandle;
        if (assetType == "Audio")
        {
            downloadHandle = Addressables.LoadAssetAsync<AudioClip>(assetReference);
        }
        else
        {
            downloadHandle = Addressables.LoadAssetAsync<VideoClip>(assetReference); // Use VideoClip for videos
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
       // Addressables.Release(downloadHandle);
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
