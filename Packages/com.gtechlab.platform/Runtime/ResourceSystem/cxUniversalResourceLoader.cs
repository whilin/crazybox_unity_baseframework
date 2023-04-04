using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class cxUniversalResourceLoader : cxSingleton<cxUniversalResourceLoader> {

    public delegate void onProgressCallback (float progress);

    public async Task < (bool need, int size) > IsRequireDownload (string url) {
        if (cxResourceNaming.IsBundle (url, out string bundleName, out string path)) {
            var bundleData = cxOnDemandBundleLoader.Instance.LoadBundle (bundleName);
            return (!(bundleData.IsCached () || bundleData.IsReady), (int) bundleData.size);
        } else if (cxResourceNaming.IsResource (url, out string resourceId, out path)) {
            var bundleData = await cxResourceBundleLoader.Instance.LoadBundle (bundleName);
            return (!(bundleData.IsCached () || bundleData.IsReady), (int) bundleData.resourceDesc.bundleSize);
        } else if (cxResourceNaming.IsHttp (url)) {
            return (!cxCachedWebContentLoader.Instance.HasCache (url), 0);
        } else {
            return (false, 0);
        }
    }

    public async Task<T> LoadAsset<T> (string url, Action<float> onProgress = null) where T : UnityEngine.Object {
        if (cxResourceNaming.IsBuiltin (url)) {
            var obj = Resources.Load<T> (url);
            return obj;
        } else if (cxResourceNaming.IsBundle (url, out string bundleName, out string path)) {
            var bundleData = cxOnDemandBundleLoader.Instance.LoadBundle (bundleName);

            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

            var obj = bundleData.assetBundle.LoadAsset<T> (path);
            return obj;
        } else if (cxResourceNaming.IsResource (url, out string resourceId, out path)) {
            var bundleData = await cxResourceBundleLoader.Instance.LoadBundle (resourceId);

            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

            var obj = bundleData.assetBundle.LoadAsset<T> (path);
            return obj;
        } else if (cxResourceNaming.IsHttp (url) || cxResourceNaming.IsStreaming (url, out path)) {
            var obj = await cxCachedWebContentLoader.Instance.LoadAsset<T> (url);
            return obj;
        }

        throw new Exception ("cxUniversalResourceLoader.LoadAsset unknown URL schema:" + url);
    }

    public async Task<Scene> LoadScene (string url, LoadSceneMode loadSceneMode, onProgressCallback onProgress = null) {

        string path = string.Empty;

        if (cxResourceNaming.IsBuiltin (url)) {
            path = url;
        } else if (cxResourceNaming.IsBundle (url, out string bundleName, out path)) {

            var bundleData = cxOnDemandBundleLoader.Instance.LoadBundle (bundleName);
            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

        } else if (cxResourceNaming.IsResource (url, out string resourceId, out path)) {
            var bundleData = await cxResourceBundleLoader.Instance.LoadBundle (resourceId);

            await bundleData.WaitForLoad ((float p) => {
                if (onProgress != null)
                    onProgress (p);
            });

        } else {
            throw new Exception ("cxUniversalResourceLoader.LoadScene unsupported url schema:" + url);
        }

        var asyncOp = SceneManager.LoadSceneAsync (path, loadSceneMode);
        asyncOp.allowSceneActivation = false;

        bool isDone = false;
        do {
            await new WaitForUpdate ();
            if (onProgress != null)
                onProgress (asyncOp.progress);

            if (asyncOp.progress > 0.9)
                isDone = true;
        } while (!isDone);

        asyncOp.allowSceneActivation = true;

        return SceneManager.GetSceneByName (url);
    }

    /*
        public async Task<Texture2D> LoadImage (string url, cxResourceNaming.onProgressCallback onProgress = null) {
            if (cxResourceNaming.IsBuiltin (url)) {
                return await cxCachedWebContentLoader.Instance.LoadTexture (url);

            } else if (cxResourceNaming.IsBundle (url, out string bundleName, out string path)) {
                var bundleData = cxOnDemandBundleLoader.Instance.LoadBundle (bundleName);

                await bundleData.WaitForLoad ((float p) => {
                    if (onProgress != null)
                        onProgress (p);
                });

                var obj = bundleData.assetBundle.LoadAsset<Texture2D> (path);
                return obj;
            } else if (cxResourceNaming.IsResource (url, out string resourceId, out path)) {
                var bundleData = await cxResourceBundleLoader.Instance.LoadBundle (resourceId);

                await bundleData.WaitForLoad ((float p) => {
                    if (onProgress != null)
                        onProgress (p);
                });

                var obj = bundleData.assetBundle.LoadAsset<Texture2D> (path);
                return obj;
            } else if (cxResourceNaming.IsHttp (url) || cxResourceNaming.IsStreaming (url, out path)) {
                return await cxCachedWebContentLoader.Instance.LoadTexture (url);
            } else {
                throw new Exception ("LoadImage unknown URL:" + url);
            }
        }

        public async Task<Sprite> LoadSprite (string url, cxResourceNaming.onProgressCallback onProgress = null) {
            if (cxResourceNaming.IsBuiltin (url)) {
                return await cxCachedWebContentLoader.Instance.LoadSprite (url);
            } else if (cxResourceNaming.IsBundle (url, out string bundleName, out string path)) {
                var bundleData = await cxResourceBundleLoader.Instance.LoadBundle (bundleName);

                await bundleData.WaitForLoad ((float p) => {
                    if (onProgress != null)
                        onProgress (p);
                });

                var obj = bundleData.assetBundle.LoadAsset<Sprite> (path);
                return obj;
            } else if (cxResourceNaming.IsResource (url, out string resourceId, out path)) {
                var bundleData = await cxResourceBundleLoader.Instance.LoadBundle (resourceId);

                await bundleData.WaitForLoad ((float p) => {
                    if (onProgress != null)
                        onProgress (p);
                });

                var obj = bundleData.assetBundle.LoadAsset<Sprite> (path);
                return obj;
            } else if (cxResourceNaming.IsHttp (url) || cxResourceNaming.IsStreaming (url, out path)) {
                return await cxCachedWebContentLoader.Instance.LoadSprite (url);

            } else {
                throw new Exception ("LoadSprite unknown URL:" + url);
            }
        }
    */
}