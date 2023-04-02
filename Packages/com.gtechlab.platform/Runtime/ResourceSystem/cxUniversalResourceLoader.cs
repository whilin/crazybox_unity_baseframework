using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class cxUniversalResourceLoader : cxSingleton<cxUniversalResourceLoader> {

    public async Task<T> LoadAsset<T> (string url, Action<float> onProgress = null) where T : UnityEngine.Object {
        if (cxResourceNaming.IsBuiltin (url, out string path)) {
            var obj = Resources.Load<T> (path);
            return obj;
        } else if (cxResourceNaming.IsBundle (url, out string bundleName, out path)) {
            var bundleData = cxBundleLoader.Instance.LoadBundle (bundleName);

            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

            var obj = bundleData.assetBundle.LoadAsset<T> (path);
            return obj;
        } else if (cxResourceNaming.IsNet (url)) {
            throw new Exception ("LoadAsset not supported URL:" + url);
        } else {
            throw new Exception ("LoadAsset unknown URL:" + url);
        }
    }

    // public async Task<Scene> LoadScene (string url, cxLoader.onProgressCallback onProgress = null) {

    // }

    public async Task<Texture2D> LoadImage (string url, cxResourceNaming.onProgressCallback onProgress = null) {
        if (cxResourceNaming.IsBuiltin (url, out string path)) {
            return await cxCachedImageLoader.Instance.LoadTexture (path);
        } else if (cxResourceNaming.IsNet (url) || cxResourceNaming.IsStreaming (url, out path)) {
            return await cxCachedImageLoader.Instance.LoadTexture (url);
        } else if (cxResourceNaming.IsBundle (url, out string bundleName, out path)) {
            var bundleData = cxBundleLoader.Instance.LoadBundle (bundleName);

            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

            var obj = bundleData.assetBundle.LoadAsset<Texture2D> (path);
            return obj;
        } else {
            throw new Exception ("LoadImage unknown URL:" + url);
        }
    }

    public async Task<Sprite> LoadSprite (string url, cxResourceNaming.onProgressCallback onProgress = null) {
        if (cxResourceNaming.IsBuiltin (url, out string path)) {
            return await cxCachedImageLoader.Instance.LoadSprite (path);
        } else if (cxResourceNaming.IsNet (url) || cxResourceNaming.IsStreaming (url, out path)) {
            return await cxCachedImageLoader.Instance.LoadSprite (url);
        } else if (cxResourceNaming.IsBundle (url, out string bundleName, out path)) {
            var bundleData = cxBundleLoader.Instance.LoadBundle (bundleName);

            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

            var obj = bundleData.assetBundle.LoadAsset<Sprite> (path);
            return obj;
        } else {
            throw new Exception ("LoadSprite unknown URL:" + url);
        }
    }
}