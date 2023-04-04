#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class cxResourceBuilder {

    [MenuItem ("Assets/G-Tech Lab/Build ResourceBundle(+Upload)", true, 0)]
    public static bool BuildSelectedObjectValid () {
        if (Selection.activeObject is cxResourceDefinition) {
            return true;
        }

        return false;
    }

    [MenuItem ("Assets/G-Tech Lab/Build Resource Bundle(+Upload)", false, 0)]
    public static void BuildSelectedObject () {
        try {
            var prefab = Selection.activeObject as cxResourceDefinition;
            var desc = BuildResource (prefab);
            BuildPackage(desc);
        } catch (Exception ex) {
            Debug.LogException(ex);
            cxEditorWindowUtils.ShowErrorMessage ("Build Resource Bundle", ex.Message);
        }
    }
  

    [MenuItem ("Tools/G-Tech Lab/Build All Resource Bundle(+Upload)")]
    public static void BuildAllResource () {
        try {
            var defs = AssetDatabase.FindAssets ("t:cxResourceDefinition");

            List<cxResourceDescModel> list = new List<cxResourceDescModel>();

            foreach (var guid in defs) {
                var path = AssetDatabase.GUIDToAssetPath (guid);
                var def = AssetDatabase.LoadAssetAtPath<cxResourceDefinition> (path);
                var desc = BuildResource (def);
                list.Add(desc);
            }

            foreach(var desc in list){
                BuildPackage(desc);
            }

        } catch (Exception ex) {
            Debug.LogError(ex);
            cxEditorWindowUtils.ShowErrorMessage ("Build All Resource Bundle", ex.Message);
        }
    }
 [MenuItem ("Tools/G-Tech Lab/Clear Bundle Caching")]
    public static void ClearBundleCache () {
        Caching.ClearCache();
    }
/*
    [MenuItem ("Tools/G-Tech Lab/Build All Asset Bundles(By Bundle name)")]
    public static void BuildAllAssetBundles () {
        try {
            string[] allAssetBundles = AssetDatabase.GetAllAssetBundleNames ();

            List<string> allAssetsInAnAssetBundle = new List<string> ();
            for (int i = 0; i < allAssetBundles.Length; ++i) {
                var curBundleName = allAssetBundles[i];
                var assetsInCurBundle = AssetDatabase.GetAssetPathsFromAssetBundle (curBundleName);

                allAssetsInAnAssetBundle.AddRange (assetsInCurBundle);
            }
        } catch (Exception ex) {
            Debug.LogError (ex);
            cxEditorWindowUtils.ShowErrorMessage ("Build All Resource Bundle", ex.Message);
        }
    }
*/

    static void CheckValidResourceDef (cxResourceDefinition def) {

        var path = AssetDatabase.GetAssetPath (def);
        var fileName = Path.GetFileNameWithoutExtension (path);

        if (string.IsNullOrEmpty (def.resourceId)) {
            throw new Exception ($"{path} resourceId empty");
            //return false;
        }

        if (!fileName.Equals (def.resourceId)) {
            throw new Exception ($"{path} must have same name with resourceId");
           // return false;
        }

        var sceneFiles = cxBundleBuilderUtil.QueryFilesInPath (path, true);
        var assetFiles = cxBundleBuilderUtil.QueryFilesInPath (path, false);

        if (def.resourceType == cxResourceType.Scene) {
            if (sceneFiles.Count == 0) {
                throw new Exception ($"{path} must have one more scene");
              //  return false;
            }
        } else {
            if (assetFiles.Count == 0) {
                throw new Exception ($"{path} must have one more assets");
               // return false;
            }

            if (sceneFiles.Count > 0) {
                throw new Exception ($"{path} can't contain scenes");
               // return false;
            }
        }

       // return true;
    }

    static cxResourceDescModel BuildResource (cxResourceDefinition def) {
        //Debug.Log ($"Build Resource {def.resourceId} ({def.resourceType}) Begin:" + prefabPath);

        var prefabPath = AssetDatabase.GetAssetPath (def);

        CheckValidResourceDef (def);

        var files = cxBundleBuilderUtil.QueryFilesInPath (prefabPath, def.resourceType == cxResourceType.Scene);

        var resourceModel = def.ToModel ();
        resourceModel.date = DateTime.Now;
        resourceModel.resourceLocation = string.Empty; // VaivResourceLoaderUtil.CombineURL (CreatorWorksDefine.ResourceBaseLocation, resourceModel.resourceId);
      
        BuildBundle (resourceModel, files);

        return resourceModel;
    }

    static void BuildBundle (cxResourceDescModel resourceModel, List<string> files) {
        //  try {
        Debug.Log ($"Build Bundle Begin:{resourceModel.resourceId} ({resourceModel.resourceType})");

        var env = cxCreatorEnvironment.GetEnvironment ();

        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild> ();
        AssetBundleBuild bundleBuild = new AssetBundleBuild () {
            assetBundleName = resourceModel.resourceId,
            assetBundleVariant = string.Empty,
            assetNames = files.ToArray ()
        };

        assetBundleBuilds.Add (bundleBuild);

        BundleTargetOutput output = new BundleTargetOutput () {
            includeAssets = { },
        };

        if (env.buildWebGL)
            output = cxBundleBuilderUtil.BuildTarget (env.BuildOutputPath, resourceModel.resourceId, assetBundleBuilds, UnityEditor.BuildTarget.WebGL);
        if (env.buildAndroid)
            output = cxBundleBuilderUtil.BuildTarget (env.BuildOutputPath, resourceModel.resourceId, assetBundleBuilds, UnityEditor.BuildTarget.Android);
        if (env.buildIOS)
            output = cxBundleBuilderUtil.BuildTarget (env.BuildOutputPath, resourceModel.resourceId, assetBundleBuilds, UnityEditor.BuildTarget.iOS);

        List<cxResourceAssetFile> includeFiles = new List<cxResourceAssetFile> ();

        foreach (var assetFile in output.includeScenes) {
                includeFiles.Add (new cxResourceAssetFile () {
                 fileType = cxResourceAssetFileType.BundleScene,
                    filename = assetFile
            });
        }

        foreach (var assetFile in output.includeAssets) {
            includeFiles.Add (new cxResourceAssetFile () {
                fileType = cxResourceAssetFileType.BundleAsset,
                    filename = assetFile
            });
        }
        

        resourceModel.assetFiles = includeFiles.ToArray ();
        resourceModel.bundleHash = output.resourceHash;
        resourceModel.bundleSize = output.resourceSize;

        string descFilePath = Path.Combine (env.BuildOutputPath, resourceModel.resourceId, cxCreatorEnvironment.ResourceJsonFileName);
        using (var file = File.CreateText (descFilePath)) {
            var json = JsonUtility.ToJson (resourceModel);
            file.Write (json);
            file.Flush ();
        }

         Debug.Log ("Build Bundle Completed:" + resourceModel.resourceId);
    }

    static void BuildPackage(cxResourceDescModel resourceModel) {
          var env = cxCreatorEnvironment.GetEnvironment ();

        var pkgFile = cxBundleBuilderUtil.BuildZipPackage (resourceModel.resourceId);
       

        if(!string.IsNullOrEmpty(env.ResourceUploadServerUrl)){
            UploadPackage (resourceModel.resourceId, pkgFile);
        }
    }



    [MenuItem ("Tools/G-Tech Lab/Debug/Upload Test")]
    static void UploadTest () {
        var pkg = "/Users/CRAZYBOX_SVN_ROOT/crazybox_package_project/BundleBuild/com.gtechlab.sample.01.cxpackage";
        UploadPackage ("com.gtechlab.sample.01", pkg);
    }

    public static void UploadPackage (string resourceId, string pkgFile) {
        var env = cxCreatorEnvironment.GetEnvironment ();

        cxResourceUploadApiRepository repo = new cxResourceUploadApiRepository (env.ResourceUploadServerUrl, env.apiToken);
        repo.UploadResourcePackage (resourceId, pkgFile,
            (resourceInfo) => {
                //  cxEditorWindowUtils.ShowMessage("리소스 업로드 완료");
                Debug.Log("UploadPackage completed: "+resourceId);
            },
            (error) => {
                Debug.LogError (error);
                // WindowUtils.ShowErrorMessage ("리소스 업로드 실패", error);
            });
    }

}

#endif