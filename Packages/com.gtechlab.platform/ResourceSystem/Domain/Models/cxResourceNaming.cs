using System;
using System.IO;
using UnityEngine;

public static class cxResourceNaming {

    public const string builtIn = "";
    public const string resource = "resource://";
    public const string streamming = "streaming://";
    public const string bundle = "bundle://";
    public const string http = "http://";
    public const string https = "https://";

    public static bool IsBuiltin (string url) {
        if (!(url.StartsWith (resource) ||
                url.StartsWith (bundle) ||
                url.StartsWith (streamming) ||
                url.StartsWith (http) ||
                url.StartsWith (https))) {
            return true;
        }
        return false;
    }

    public static bool IsStreaming (string url, out string path) {

        if (url.StartsWith (streamming)) {
            path = url.Substring (streamming.Length);
            return true;
        }

        path = null;
        return false;
    }

    public static bool IsBundle (string url, out string bundleName, out string path) {
        if (url.StartsWith (bundle)) {
            var fullPath = url.Substring (resource.Length);
            var names = fullPath.Split ("/");
            if (names.Length != 2)
                throw new Exception ("Universal Resource Name Exception :" + url);

            bundleName = names[0];
            path = names[1];
            return true;
        }

        bundleName = null;
        path = null;
        return false;
    }

    public static bool IsResource (string url, out string resourceId, out string path) {
        if (url.StartsWith (resource)) {
            var fullPath = url.Substring (resource.Length);
            var names = fullPath.Split ("/");
            // if (names.Length != 2)
            //     throw new Exception ("Universal Resource Name Exception :" + url);

            resourceId = names[0];
            path = (names.Length > 1) ? names[1] : string.Empty;
            return true;
        }

        resourceId = null;
        path = null;
        return false;
    }

    public static bool IsHttp (string url) {
        return url.StartsWith (http) || url.StartsWith (https);
    }

    public static string GetBundleURL (string baseLocation, string resourceId, string platformName) {
        string uri = $"{baseLocation}/{resourceId}/{platformName}/{resourceId}";
        return uri;
    }

    public static string GetActivePlatformName () {
        string platformName = string.Empty;

#if UNITY_ANDROID
        platformName = "Android";
#elif UNITY_IOS
        platformName = "iOS";
#elif UNITY_WEBGL 
        platformName = "WebGL";
#else
        platformName = "Standalone";
#endif 

        return platformName;
    }

    public static string ToAppStreamingPath (string path) {
        var newPath = Path.Combine (Application.streamingAssetsPath, path);

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        newPath = "file://" + newPath;
#elif UNITY_IOS
        newPath = "file://" + newPath;
#endif
        return newPath;
    }
}