using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Threading;

public class AssetBundleManager : MonoSingleton<AssetBundleManager> {

    public enum LoadOption {
        FromBuildSetting,
        FromLocalBundles,
        FromRemoteBundles
    }

    [SerializeField]
    public LoadOption loadOption = LoadOption.FromBuildSetting;

    [SerializeField]
    private string bundleBaseURL;
    // [SerializeField]
    // private bool useEditorAssets = false;

    public bool Ready { get; private set; } = false;
    //public string bundleEditorPath;

    private List<AssetBundleDesc> bundleTable = new List<AssetBundleDesc> ();
    private List<AssetBundle> bundles = new List<AssetBundle> ();
    private string platformName;

    private void Start () {
        // #if UNITY_EDITOR
        //         StartWithOption(loadOption);
        // #endif
    }

    public void StartWithOption (LoadOption _loadOption, string bundleDownloadURL=null) {

        loadOption = _loadOption;
        bundleBaseURL = bundleDownloadURL ?? bundleBaseURL;

#if UNITY_ANDROID
        platformName = "Android";
#elif UNITY_IOS
        platformName = "IOS";
#elif UNITY_WEBGL 
        platformName = "WebGL";
#else
        platformName = "Standalone";
#endif 

        if (loadOption == LoadOption.FromLocalBundles)
            LoadBundleTableInEditor ();
        else if (loadOption == LoadOption.FromRemoteBundles)
            LoadBundleTable ();
        else
            Ready = true;
    }

    AssetBundleDesc FindSceneBundleDesc (string sceneName) {
        var sceneFileName = sceneName + ".unity";
        var desc = bundleTable.Find (q => q.scenes.Exists (s => s.EndsWith (sceneFileName)));
        return desc;
    }

    AssetBundleDesc FindObjectBundleDesc (string objName) {
        var desc = bundleTable.Find (q => q.assets.Exists ((s) => {
            var file = Path.GetFileNameWithoutExtension (s);
            return string.Equals (file, objName, StringComparison.OrdinalIgnoreCase);
        }));
        return desc;
    }

    public AssetBundleDesc FindBundleDesc(string bundleName) {
        return bundleTable.Find(q => q.bundle == bundleName);
    }
    
    public bool CheckNeedDownload (string sceneName) {
        if (loadOption == LoadOption.FromBuildSetting)
            return false;

        var bundleDesc = FindSceneBundleDesc (sceneName);
        if (bundleDesc == null) {
            return false;
        } else {
            var find = bundles.Find (q => q.name == bundleDesc.bundle);
            return (find == null);
        }
    }

    public AssetBundle FindBundle (string bundleName) {
        if (loadOption == LoadOption.FromBuildSetting)
            return null;

        return bundles.Find (q => q.name == bundleName);
    }

    public async Task LoadAllBundles (Action<float> downloadState) {
        if (loadOption == LoadOption.FromBuildSetting) {
            downloadState (0);
            downloadState (1);
            return;
        }

        downloadState (0);
        foreach (var bundleDesc in bundleTable) {

            try {
                Debug.LogFormat ("LoadAllBundles {0} Load begin", bundleDesc.bundle);

                AssetBundle bundle;

                if (loadOption == LoadOption.FromLocalBundles)
                    bundle = await LoadBundleInEditor (bundleDesc.bundle, bundleDesc.hash, downloadState);
                else
                    bundle = await LoadBundle (bundleDesc.bundle, bundleDesc.hash, downloadState);

                bundles.Add (bundle);
                Debug.LogFormat ("LoadAllBundles {0} Load completed", bundleDesc.bundle);
            } catch (Exception ex) {
                Debug.LogErrorFormat ("LoadAllBundles {0} Load failed ex:{1}", bundleDesc.bundle, ex.Message);
            }
        }

        downloadState (1);
    }


    public async Task<AssetBundle> LoadBundle (string bundleName, Action<float> downloadState = null) {
        if (loadOption == LoadOption.FromBuildSetting) {
            if (downloadState != null) downloadState (0);
            if (downloadState != null) downloadState (1);
            return null;
        }

        var bundleDesc = bundleTable.Find(q => q.bundle == bundleName);
        if (bundleDesc != null) {

            var find = bundles.Find (q => q.name == bundleDesc.bundle);

            if (find == null) {
                if (loadOption == LoadOption.FromLocalBundles)
                    find = await LoadBundleInEditor (bundleDesc.bundle, bundleDesc.hash, downloadState);
                else
                    find = await LoadBundle (bundleDesc.bundle, bundleDesc.hash, downloadState);

                if (find != null)
                    bundles.Add (find);
            }

            if (find == null)
                throw new Exception ($"{bundleDesc.bundle} load failed");
            
            return find;
        } else {
            throw new Exception($"{bundleName} bundle desc not found");
        }
    }

    public async Task LoadSceneBundle (string sceneName, Action<float> downloadState = null) {
        if (loadOption == LoadOption.FromBuildSetting) {
            if (downloadState != null) downloadState (0);
            if (downloadState != null) downloadState (1);
            return;
        }

        var bundleDesc = FindSceneBundleDesc (sceneName);
        if (bundleDesc != null) {

            var find = bundles.Find (q => q.name == bundleDesc.bundle);

            if (find == null) {
                if (loadOption == LoadOption.FromLocalBundles)
                    find = await LoadBundleInEditor (bundleDesc.bundle, bundleDesc.hash, downloadState);
                else
                    find = await LoadBundle (bundleDesc.bundle, bundleDesc.hash, downloadState);

                if (find != null)
                    bundles.Add (find);
            }

            if (find == null)
                throw new Exception ($"{bundleDesc.bundle} load failed");

            string[] scenePaths = find.GetAllScenePaths ();
        }else {
            throw new Exception($"scene({sceneName}) bundle desc not found");
        }
    }

    void LoadBundleTableInEditor () {
        try {
            var bundleFile = Path.Combine (Application.dataPath, "..", $"{AssetBundleSystem.BaseBundlePath}/{platformName}/{AssetBundleSystem.TableFileName}");

            using (var reader = File.OpenText (bundleFile)) {
                var json = reader.ReadToEnd ();
                bundleTable = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssetBundleDesc>> (json);
            }

            Ready = true;
        } catch (Exception ex) {
            Debug.LogException (ex);
            throw ex;
        }
    }

    async void LoadBundleTable () {
        try {
            var url = $"{bundleBaseURL}/{platformName}/{AssetBundleSystem.TableFileName}";
            UnityWebRequest req = UnityWebRequest.Get (url);
            await req.SendWebRequest ();

            if (req.result != UnityWebRequest.Result.Success) {
                throw new Exception ("Load Bundle Table failed");
            }

            var json = req.downloadHandler.text;
            bundleTable = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssetBundleDesc>> (json);
            Ready = true;
        } catch (Exception ex) {
            Debug.LogException (ex);
            throw ex;
        }
    }

    async Task<AssetBundle> LoadBundleInEditor (string bundleName, string hash, Action<float> downloadState = null) {

        var bundleFile = Path.Combine (Application.dataPath, "..", $"{AssetBundleSystem.BaseBundlePath}/{platformName}/{bundleName}");
        var bundle = await AssetBundle.LoadFromFileAsync (bundleFile);

        return bundle;
    }

    public bool HasCachedVersion(string bundleName){
        
        var bundleDesc = bundleTable.Find( q => q.bundle == bundleName);
        if(bundleDesc == null) {
            return false;
        }

        var hash128 = Hash128.Parse(bundleDesc.hash);

        List<Hash128> cachedVersions = new List<Hash128>();
        Caching.GetCachedVersions(bundleName, cachedVersions);
        var cacheVersion = cachedVersions.Find(q => q.Equals(hash128));

        return cacheVersion.isValid;
    }


    async Task<AssetBundle> LoadBundle (string bundleName, string hash, Action<float> downloadState = null, CancellationToken? cancellationToken = null) {
        string uri = $"{bundleBaseURL}/{platformName}/{bundleName}";

        try {
           
            var req = UnityWebRequestAssetBundle.GetAssetBundle (uri, Hash128.Parse (hash));
            var asyncOp = req.SendWebRequest ();

            while (!asyncOp.isDone && req.result == UnityWebRequest.Result.InProgress) {
                if (downloadState != null)
                    downloadState (req.downloadProgress);
                
                await new WaitForEndOfFrame ();

                if(cancellationToken.HasValue){
                    if(cancellationToken.Value.IsCancellationRequested) {
                       // cancellationToken.Value.ThrowIfCancellationRequested();
                       req.Abort();
                       throw new Exception($"{bundleName} download aborted by user");
                    }
                }
            }

            if (req.result != UnityWebRequest.Result.Success) {
                throw new Exception ("Bundle Load Exception :" + bundleName + " exception:" + req.error);
            }

            if (downloadState != null)
                downloadState (0);

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent (req);

            return bundle;
        } catch (Exception ex) {
            Debug.LogException (ex);
            throw ex;
            // return null;
        }
    }
}