using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public struct BundleTargetOutput {
    public int resourceSize;
    public string resourceHash;
    public string[] includeAssets;
}

public abstract class cxBundleBuilderUtil {

    public static BundleTargetOutput BuildTarget (string baseOuputPath, string resourceName, List<AssetBundleBuild> assetBundleBuilds, BuildTarget buildTarget) {

        string outputPath2 = Path.Combine (baseOuputPath, resourceName, buildTarget.ToString ());
        if (!Directory.Exists (outputPath2))
            Directory.CreateDirectory (outputPath2);

        var manifest = BuildPipeline.BuildAssetBundles (outputPath2, assetBundleBuilds.ToArray (), BuildAssetBundleOptions.ForceRebuildAssetBundle, buildTarget);
        var bundleNames = manifest.GetAllAssetBundles ();

        BundleTargetOutput output = new BundleTargetOutput ();

        foreach (var bundleName in bundleNames) {
            var hash = manifest.GetAssetBundleHash (bundleName);
            var bundleFileName = Path.Combine (Application.dataPath, "..", outputPath2, bundleName);
            var file = File.OpenRead (bundleFileName);
            var fileSize = file.Length;
            file.Close ();

            var bundle = AssetBundle.LoadFromFile (Path.Combine (Application.dataPath, "..", outputPath2, bundleName));
            var files = new List<string>();
            files.AddRange(bundle.GetAllScenePaths());
            files.AddRange(bundle.GetAllAssetNames());

            output.includeAssets =files.ToArray();
            output.resourceSize = (int) fileSize; // (int) file.Seek (0, SeekOrigin.End);
            output.resourceHash = manifest.GetAssetBundleHash (resourceName).ToString ();
        }

        return output;
    }

    public static string BuildZipPackage (string resourceId) {

        var env = cxCreatorEnvironment.GetEnvironment();

        string inputFolder = Path.Combine (Application.dataPath, "..", env.BuildOutputPath, resourceId);
        string pkgName = resourceId + ".cxpackage";
        string outputPath = Path.Combine (Application.dataPath, "..", env.BuildOutputPath, pkgName);

        ZipUtility.CompressFolderToZip (outputPath, null, inputFolder);

        return outputPath;
    }

    public static List<string> QueryFilesInPath (string path, bool scene) {
        string dir = Path.GetDirectoryName (path);
        return _QueryFilesInPath (dir, scene);
    }

    static List<string> _QueryFilesInPath (string path, bool scene) {

        List<string> files = new List<string> ();

        foreach (var file in Directory.EnumerateFiles (path)) {
            if (file.EndsWith (".meta"))
                continue;

            bool isScene = file.EndsWith (".unity");
            if (scene) {
                if (!isScene)
                    continue;
            } else {
                if (isScene)
                    continue;
            }

            files.Add (file);
        }

        foreach (var dir in Directory.EnumerateDirectories (path)) {
            files.AddRange (_QueryFilesInPath (dir, scene));
        }

        return files;
    }


    // public static void AskOpenViewer (string resourceId, int version, cxResourceType resourceType) {
    //     if (WindowUtils.AskOpenViewer ()) {
    //         string url = CreatorUtils.GetViewerURL (resourceId, version, resourceType);
    //         Application.OpenURL (url);
    //     }
    // }
}