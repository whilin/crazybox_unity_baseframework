using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AssetBundleManager : MonoSingleton<AssetBundleManager> {

    [SerializeField]
    private string bundleBaseURL;
    [SerializeField]
    private bool useEditorAssets = false;

    public bool Ready { get; private set; } = false;
    //public string bundleEditorPath;

    private List<AssetBundleDesc> bundleTable = new List<AssetBundleDesc> ();
    private List<AssetBundle> bundles = new List<AssetBundle> ();
    private string platformName;

    private void Start () {

// #if !UNITY_EDITOR
//         useEditorAssets = false;
// #endif

#if UNITY_ANDROID
        platformName = "Android";
#elif UNITY_IOS
        platformName = "IOS";
#elif UNITY_WEBGL 
        platformName = "WebGL";
#else
        platformName = "Standalone";
#endif 

        if (useEditorAssets)
            LoadBundleTableInEditor ();
        else
            LoadBundleTable ();

    }

    AssetBundleDesc FindSceneBundleDesc (string sceneName) {
        var sceneFileName = sceneName + ".unity";
        var desc = bundleTable.Find (q => q.scenes.Exists (s => s.EndsWith (sceneFileName)));
        return desc;
    }

    AssetBundleDesc FindObjectBundleDesc (string objName) {
        var desc = bundleTable.Find (q => q.assets.Exists ((s) => {
            var file = Path.GetFileNameWithoutExtension(s);
            return string.Equals (file, objName, StringComparison.OrdinalIgnoreCase);
        }));
        return desc;
    }

    public bool CheckNeedDownload (string sceneName) {
        var bundleDesc = FindSceneBundleDesc (sceneName);
        if (bundleDesc == null) {
            return false;
        } else {
            var find = bundles.Find (q => q.name == bundleDesc.bundle);
            return (find == null);
        }
    }

    public AssetBundle FindBundle (string bundleName) {
        return bundles.Find (q => q.name == bundleName);
    }

    public async Task LoadAllBundles (Action<float> downloadState) {
        downloadState (0);
        foreach (var bundleDesc in bundleTable) {
            var bundle = await LoadBundle (bundleDesc.bundle, bundleDesc.hash, downloadState);
            bundles.Add (bundle);
        }
    }

    public async Task LoadSceneBundle (string sceneName, Action<float> downloadState = null) {

        var bundleDesc = FindSceneBundleDesc (sceneName);
        if (bundleDesc != null) {

            var find = bundles.Find (q => q.name == bundleDesc.bundle);

            if (find == null) {
                if (useEditorAssets)
                    find = await LoadBundleInEditor (bundleDesc.bundle, bundleDesc.hash, downloadState);
                else
                    find = await LoadBundle (bundleDesc.bundle, bundleDesc.hash, downloadState);

                if (find != null)
                    bundles.Add (find);
            }

            if (find == null)
                throw new Exception ($"{bundleDesc.bundle} load failed");

            string[] scenePaths = find.GetAllScenePaths ();
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
        }
    }

    async void LoadBundleTable () {
        try {
            var url = $"{bundleBaseURL}/{platformName}/{AssetBundleSystem.TableFileName}";
            UnityWebRequest req = UnityWebRequest.Get (url);
            await req.SendWebRequest ();

            var json = req.downloadHandler.text;
            bundleTable = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssetBundleDesc>> (json);
            Ready = true;
        } catch (Exception ex) {
            Debug.LogException (ex);
        }
    }

    async Task<AssetBundle> LoadBundleInEditor (string bundleName, string hash, Action<float> downloadState = null) {

        var bundleFile = Path.Combine (Application.dataPath, "..", $"{AssetBundleSystem.BaseBundlePath}/{platformName}/{bundleName}");
        var bundle = await AssetBundle.LoadFromFileAsync (bundleFile);

        return bundle;
    }

    async Task<AssetBundle> LoadBundle (string bundleName, string hash, Action<float> downloadState = null) {
        string uri = $"{bundleBaseURL}/{platformName}/{bundleName}";

        try {
            var req = UnityWebRequestAssetBundle.GetAssetBundle (uri, Hash128.Parse (hash));
            var asyncOp = req.SendWebRequest ();
            while (!asyncOp.isDone) {
                if (downloadState != null)
                    downloadState (req.downloadProgress);
                await new WaitForEndOfFrame ();
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