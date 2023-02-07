#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BundleBuiler : MonoBehaviour {

        [MenuItem ("Tools/Bundle Builder/Build AssetBundles")]
        public static void BuildAllAssetBundles () {

                BuildTarget buildTarget = BuildTarget.NoTarget;

#if (UNITY_ANDROID)
                buildTarget = BuildTarget.Android;
#elif (UNITY_IOS)
                buildTarget = BuildTarget.iOS;
#elif UNITY_WEBGL
                buildTarget = BuildTarget.WebGL;
#else 
                buildTarget = BuildTarget.StandaloneLinux64;
#endif
                BuildAllAssetBundle (buildTarget);
        }

        public static void BuildAllAssetBundle (BuildTarget buildTarget) {
                AssetBundleManifest manifest;

                string platformName = buildTarget.ToString ();

                switch (buildTarget) {

                        case BuildTarget.Android:
                                platformName = "Android";
                                break;
                        case BuildTarget.iOS:
                                platformName = "IOS";
                                break;
                        case BuildTarget.WebGL:
                                platformName = "WebGL";
                                break;
                        case BuildTarget.StandaloneLinux64:
                        default:
                                platformName = "Standalone";
                                break;
                }

                var outputPath = $"{AssetBundleSystem.BaseBundlePath}/{platformName}";

                var realPath = Path.Combine (Application.dataPath, "..", outputPath);
                if (!Directory.Exists (realPath)) {
                        Directory.CreateDirectory (realPath);
                }

                manifest = BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.None, buildTarget);

                var bundles = new List<AssetBundleDesc> ();

                var bundleNames = manifest.GetAllAssetBundles ();
                foreach (var bundleName in bundleNames) {
                        var bundlefilePath = Path.Combine (Application.dataPath, "..", $"{AssetBundleSystem.BaseBundlePath}/{platformName}", bundleName);

                        var hash = manifest.GetAssetBundleHash (bundleName);
                        var find = AssetBundle.LoadFromFile (bundlefilePath);
                        var assetList = find.GetAllAssetNames ();
                        var sceneList = find.GetAllScenePaths ();

                        var filesize = new FileInfo(bundlefilePath).Length;

                        AssetBundleDesc desc = new AssetBundleDesc () {
                                bundle = bundleName,
                                hash = hash.ToString (),
                                scenes = new List<string> (sceneList),
                                assets = new List<string> (assetList),
                                size = filesize
                        };

                        bundles.Add (desc);
                        find.Unload (true);
                }

                var descFile = Path.Combine (Application.dataPath, "..", $"{AssetBundleSystem.BaseBundlePath}/{platformName}", AssetBundleSystem.TableFileName);

                var descJson = Newtonsoft.Json.JsonConvert.SerializeObject (bundles);
                var file = File.CreateText (descFile);
                file.Write (descJson);
                file.Flush ();
                file.Close ();
        }
}

#endif