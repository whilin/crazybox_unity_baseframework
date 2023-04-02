using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class cxBundleData {

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
        return AssetBundleManager.Instance.HasCachedVersion (bundleName);
    }
}

public class cxBundleLoader : cxSingleton<cxBundleLoader> {

   // private static cxBundleLoader _instnace;

    private List<cxBundleData> chunks = new List<cxBundleData> ();

  //  public static cxBundleLoader Instance { get { return _instnace; } }

    public static cxBundleLoader Create (AssetBundleManager.LoadOption _loadOption, string bundleDownloadURL) {
        AssetBundleManager.Instance.StartWithOption (_loadOption, bundleDownloadURL);
        return cxBundleLoader.Instance;
    }

    public cxBundleData GetBundleData (string bundleName) {
        cxBundleData chunk;
        lock (chunks) {
            chunk = chunks.Find (q => q.bundleName == bundleName);
            if (chunk == null) {
                var bundleDesc = AssetBundleManager.Instance.FindBundleDesc (bundleName);
                if(bundleDesc == null) {
                    Debug.Log("GetBundleData BundleDesc not found:"+bundleName);
                    return null;
                }

                chunk = new cxBundleData () {
                    bundleName = bundleName,
                    size = bundleDesc.size
                };

                var cached = AssetBundleManager.Instance.HasCachedVersion (bundleName);
                chunk.cached = cached;
                chunk.state.OnNext (cxBundleData.BundleLoadState.Unloaded);
                chunks.Add (chunk);
            }
        }

        return chunk;
    }

    public cxBundleData LoadBundle (string bundleName) {

        cxBundleData chunk = GetBundleData (bundleName);
        if(chunk == null)
            return null;

        bool needLoad = chunk.state.Value == cxBundleData.BundleLoadState.Unloaded ||
            chunk.state.Value == cxBundleData.BundleLoadState.Error;
        if (needLoad)
            StartDownload (chunk);

        return chunk;
    }

    async void StartDownload (cxBundleData chunk) {
        chunk.tokenSource = new CancellationTokenSource ();

        try {
            chunk.assetBundle =  await AssetBundleManager.Instance.LoadBundle (chunk.bundleName, (prog) => {
                chunk.loadingProg.OnNext (prog);
            });
            chunk.state.OnNext (cxBundleData.BundleLoadState.Loaded);
        } catch (Exception ex) {
            chunk.errorMsg = ex.Message;
            chunk.state.OnNext (cxBundleData.BundleLoadState.Error);
        } finally {
            chunk.tokenSource.Dispose ();
            chunk.tokenSource = null;
        }
    }
}