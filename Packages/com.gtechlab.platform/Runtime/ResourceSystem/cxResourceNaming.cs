using System;

public static class cxResourceNaming {
    public delegate void onProgressCallback (float progress);

    public const string builtIn = "";
    public const string resource = "resource://";
    public const string streamming = "streaming://";
    public const string bundle = "bundle://";
    public const string http = "http://";
    public const string https = "https://";

    public static bool IsBuiltin (string url, out string path) {
        if (url.StartsWith (resource) || !(
                url.StartsWith (bundle) ||
                url.StartsWith (http) ||
                url.StartsWith (https))) {

            if (url.StartsWith (resource)) {
                path = url.Substring (resource.Length);
            } else {
                path = url;
            }

            return true;
        }

        path = null;
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

    public static bool IsNet (string url) {
        return url.StartsWith (http) || url.StartsWith (https);
    }
}