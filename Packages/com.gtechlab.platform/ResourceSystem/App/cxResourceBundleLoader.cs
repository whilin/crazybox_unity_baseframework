using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class cxResourceBundleData {

    public enum BundleLoadState {
        Unloaded,
        Loading,
        Loaded,
        Error
    }

    // public string bundleName;
    // public long size;

    public cxResourceDescModel resourceDesc;

    public AssetBundle assetBundle;

    public bool cached;
    public BehaviorSubject<BundleLoadState> state = new BehaviorSubject<BundleLoadState> (BundleLoadState.Unloaded);
    public BehaviorSubject<float> loadingProg = new BehaviorSubject<float> (0);
    public string errorMsg = string.Empty;
    public CancellationTokenSource tokenSource;

    public async Task<bool> WaitForLoad (Action<float> onProg) {
        if (state.Value == BundleLoadState.Loaded || state.Value == BundleLoadState.Error)
            return state.Value == BundleLoadState.Loaded;;

        var dispose = loadingProg.Subscribe ((prog) => {
            onProg (prog);
        });
        await new WaitUntil (() => state.Value == BundleLoadState.Loaded || state.Value == BundleLoadState.Error);

        dispose.Dispose ();

        return state.Value == BundleLoadState.Loaded;
    }

    public bool IsReady {
        get => state.Value == BundleLoadState.Loaded;
    }

    public void TryCancel () {
        if (tokenSource != null)
            tokenSource.Cancel ();
    }

    public bool IsCached () {
        return cxResourceBundleLoader.Instance.HasCachedVersion (resourceDesc.resourceId, resourceDesc.bundleHash);
    }

    public string GetMainAssetName () {
        foreach (var fileInfo in resourceDesc.assetFiles) {

            if (resourceDesc.resourceType == cxResourceType.Scene) {
                if (fileInfo.fileType == cxResourceAssetFileType.BundleScene) {
                    var onlyFilename = Path.GetFileNameWithoutExtension (fileInfo.filename);
                    return onlyFilename;
                }
            } else {

                var onlyFilename = Path.GetFileNameWithoutExtension (fileInfo.filename);
                return onlyFilename;
            }
        }

        return null;
    }
}

public class cxResourceBundleLoader : cxSingleton<cxResourceBundleLoader> {

    private cxIResourceInfoRepository resourceInfoRepository;
    private List<cxResourceBundleData> chunks = new List<cxResourceBundleData> ();
    private List<cxResourceDescModel> resourceDescList = new List<cxResourceDescModel> ();

    private string resourceLocation;
    private string platformName;

    public static void Initialize (cxIResourceInfoRepository resourceInfoRepository, string resourceLocation) {
        cxResourceBundleLoader.Instance.resourceInfoRepository = resourceInfoRepository;
        cxResourceBundleLoader.Instance.resourceLocation = resourceLocation;
        cxResourceBundleLoader.Instance.platformName = cxResourceNaming.GetActivePlatformName ();
    }

    private async Task<cxResourceDescModel> FindResourceInfo (string resourceId) {

        var resourceDesc = resourceDescList.Find (q => q.resourceId == resourceId);
        if (resourceDesc != null)
            return resourceDesc;

        resourceDesc = await resourceInfoRepository.FindResourceInfo (resourceId);
        if (resourceDesc != null) {
            resourceDescList.Add (resourceDesc);
            return resourceDesc;
        }

        Debug.Log ("FindResourceInfo not found:" + resourceId);
        return null;
    }

    public async Task<cxResourceBundleData> GetBundleData (string resourceId) {
        cxResourceBundleData chunk;
        lock (chunks) {
            chunk = chunks.Find (q => q.resourceDesc.resourceId == resourceId);
        }

        if (chunk == null) {
            var resourceDesc = await FindResourceInfo (resourceId);
            if (resourceDesc == null) {
                Debug.Log ("GetBundleData BundleDesc not found:" + resourceId);
                return null;
            }

            lock (chunks) {
                chunk = new cxResourceBundleData () {
                    resourceDesc = resourceDesc
                };

                var cached = HasCachedVersion (resourceId, resourceDesc.bundleHash);
                chunk.cached = cached;
                chunk.state.OnNext (cxResourceBundleData.BundleLoadState.Unloaded);

                if (!chunks.Exists (q => q.resourceDesc.resourceId == chunk.resourceDesc.resourceId))
                    chunks.Add (chunk);
                else {
                    Debug.LogWarning ("GetBundleData Add failed: already :" + chunk.resourceDesc.resourceId);
                }
            }
        }

        return chunk;
    }

    public async Task<cxResourceBundleData> LoadBundle (string bundleName) {

        cxResourceBundleData chunk = await GetBundleData (bundleName);
        if (chunk == null)
            return null;

        bool needLoad = chunk.state.Value == cxResourceBundleData.BundleLoadState.Unloaded ||
            chunk.state.Value == cxResourceBundleData.BundleLoadState.Error;
        if (needLoad)
            StartDownload (chunk);

        return chunk;
    }

    private async void StartDownload (cxResourceBundleData chunk) {
        chunk.tokenSource = new CancellationTokenSource ();

        try {
            chunk.assetBundle = await LoadBundle (chunk.resourceDesc.resourceId, chunk.resourceDesc.bundleHash, (prog) => {
                chunk.loadingProg.OnNext (prog);
            }, chunk.tokenSource.Token);
            chunk.state.OnNext (cxResourceBundleData.BundleLoadState.Loaded);
        } catch (Exception ex) {
            chunk.errorMsg = ex.Message;
            chunk.state.OnNext (cxResourceBundleData.BundleLoadState.Error);
        } finally {
            chunk.tokenSource.Dispose ();
            chunk.tokenSource = null;
        }
    }

    public bool HasCachedVersion (string resourceId, string hash) {

#if !UNITY_WEBGL
       
        string bundleName =  resourceId.Replace(".","_"); 
        var hash128 = Hash128.Parse (hash);

        List<Hash128> cachedVersions = new List<Hash128> ();
        Caching.GetCachedVersions (bundleName, cachedVersions);
        var cacheVersion = cachedVersions.Find (q => q.Equals (hash128));

        return cacheVersion.isValid;
#endif
        return false;
    }

    private async Task<AssetBundle> LoadBundle (string resourceId, string hash, Action<float> downloadState = null, CancellationToken? cancellationToken = null) {
        string uri = cxResourceNaming.GetBundleURL (resourceLocation, resourceId, platformName);

        try {

            CachedAssetBundle cached = new CachedAssetBundle(resourceId.Replace(".", "_"), Hash128.Parse (hash));
            var req = UnityWebRequestAssetBundle.GetAssetBundle (uri, cached, 0);
            
            var asyncOp = req.SendWebRequest ();

            while (!asyncOp.isDone && req.result == UnityWebRequest.Result.InProgress) {
                if (downloadState != null)
                    downloadState (req.downloadProgress);

                await new WaitForEndOfFrame ();

                if (cancellationToken.HasValue) {
                    if (cancellationToken.Value.IsCancellationRequested) {
                        req.Abort ();
                        throw new Exception ($"{resourceId} download aborted by user");
                    }
                }
            }

         
            if (req.result != UnityWebRequest.Result.Success) {
                throw new Exception ("Bundle Load Exception :" + resourceId + " exception:" + req.error);
            }

            if (downloadState != null)
                downloadState (0);

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent (req);

            return bundle;
        } catch (Exception ex) {
            Debug.LogException (ex);
            throw ex;
        }
    }

}