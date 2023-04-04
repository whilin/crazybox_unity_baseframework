using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class cxOnDemandBundleData {

    public enum BundleLoadState {
        Unloaded,
        Loading,
        Loaded,
        Error
    }

    public string bundleName;
    public AssetBundle assetBundle;
    public long size;
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
        return cxAssetBundleManager.Instance.HasCachedVersion (bundleName);
    }
}

public class cxOnDemandBundleLoader {

    private static cxOnDemandBundleLoader _instnace;

    private List<cxOnDemandBundleData> chunks = new List<cxOnDemandBundleData> ();

    public static cxOnDemandBundleLoader Instance { get { return _instnace; } }

    public static void Create (cxAssetBundleManager.LoadOption _loadOption, string bundleDownloadURL) {
        cxAssetBundleManager.Instance.StartWithOption (_loadOption, bundleDownloadURL);
        _instnace = new cxOnDemandBundleLoader ();
    }

    public cxOnDemandBundleData GetBundleData (string bundleName) {
        cxOnDemandBundleData chunk;
        lock (chunks) {
            chunk = chunks.Find (q => q.bundleName == bundleName);
            if (chunk == null) {
                var bundleDesc = cxAssetBundleManager.Instance.FindBundleDesc (bundleName);
                if(bundleDesc == null) {
                    Debug.Log("GetBundleData BundleDesc not found:"+bundleName);
                    return null;
                }

                chunk = new cxOnDemandBundleData () {
                    bundleName = bundleName,
                    size = bundleDesc.size
                };

                var cached = cxAssetBundleManager.Instance.HasCachedVersion (bundleName);
                chunk.cached = cached;
                chunk.state.OnNext (cxOnDemandBundleData.BundleLoadState.Unloaded);
                chunks.Add (chunk);
            }
        }

        return chunk;
    }

    public cxOnDemandBundleData LoadBundle (string bundleName) {

        cxOnDemandBundleData chunk = GetBundleData (bundleName);
        if(chunk == null)
            return null;

        /*
        bool needLoad = false;
        lock (chunks) {
            chunk = chunks.Find (q => q.bundleName == bundleName);
            if (chunk == null) {
                chunk = new cxOnDemandBundleData () {
                bundleName = bundleName
                };
                chunk.state.OnNext (cxOnDemandBundleData.BundleLoadState.Loading);
                needLoad = true;
                chunks.Add (chunk);
            }
        }
        */

        bool needLoad = chunk.state.Value == cxOnDemandBundleData.BundleLoadState.Unloaded ||
            chunk.state.Value == cxOnDemandBundleData.BundleLoadState.Error;
        if (needLoad)
            StartDownload (chunk);

        return chunk;
    }

    async void StartDownload (cxOnDemandBundleData chunk) {
        chunk.tokenSource = new CancellationTokenSource ();

        try {
            chunk.assetBundle =  await cxAssetBundleManager.Instance.LoadBundle (chunk.bundleName, (prog) => {
                chunk.loadingProg.OnNext (prog);
            });
            chunk.state.OnNext (cxOnDemandBundleData.BundleLoadState.Loaded);
        } catch (Exception ex) {
            chunk.errorMsg = ex.Message;
            chunk.state.OnNext (cxOnDemandBundleData.BundleLoadState.Error);
        } finally {
            chunk.tokenSource.Dispose ();
            chunk.tokenSource = null;
        }
    }
}