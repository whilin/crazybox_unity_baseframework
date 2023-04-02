using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class cxCachedImageLoader : cxSingleton<cxCachedImageLoader> {

    class CachedImage {
        public string path;
        public Texture2D texture;
        public Sprite sprite;

        // 0 = , 1=loading , 2=loaded, 3= failed
        public BehaviorSubject<int> state = new BehaviorSubject<int> (0);

        public async Task WaitForLoad () {
            await new WaitUntil (() => state.Value == 2 || state.Value == 3);
        }
    }

    Dictionary<string, CachedImage> cachedImages = new Dictionary<string, CachedImage> ();
    public async Task<Sprite> LoadSprite (string url) {

        var cached = await LoadImpl(url, true);

        if (cached != null) {
            if (cached.sprite == null && cached.state.Value == 2) {
                cached.sprite = Sprite.Create (cached.texture, new Rect (0, 0, cached.texture.width, cached.texture.height), new Vector2 (0.5f, 0.5f), 100);
            }

            return cached.sprite;
        }

        return null;
    }

    public async Task<Texture2D> LoadTexture (string url) {
        var cached = await LoadImpl(url, false);

        if(cached !=null)
            return cached.texture;
        else
            return null;
    }

    private async Task<CachedImage> LoadImpl (string url, bool sprite) {
        if (string.IsNullOrEmpty (url))
            return null;

        var cached = Find (url);

        if (cached == null) {

            lock (cachedImages) {
                cached = new CachedImage () {
                    path = url.ToLower (),
                    texture = null,
                    sprite = null,
                };
                cached.state.OnNext (1);
                cachedImages.Add (cached.path, cached);
            }

            bool isNet = cxResourceNaming.IsNet(url);
            
            if(cxResourceNaming.IsStreaming(url, out string path)){
                isNet = true;
                url = Path.Combine( Application.streamingAssetsPath, path);
            }

            Texture2D myTexture =  null;
            Sprite mySprite = null;

            if (isNet) {
                var op = UnityWebRequestTexture.GetTexture (url).SendWebRequest ();
                await new WaitUntil (() => op.isDone);

                if (op.webRequest.result == UnityWebRequest.Result.Success) {
                    myTexture = ((DownloadHandlerTexture) op.webRequest.downloadHandler).texture;

                    cached.texture = myTexture;
                    cached.state.OnNext (2);
                } else {
                    Debug.LogWarning ($"cxCachedImageLoader http load failed: {url}");
                    cached.state.OnNext (3);
                }
            } else {

                if(sprite) {
                    mySprite= Resources.Load<Sprite> (url);
                    if (mySprite) {
                        cached.sprite = mySprite;
                        cached.state.OnNext (2);

                    } else {
                        Debug.LogWarning ($"cxCachedImageLoader local load failed: {url}");
                        cached.state.OnNext (3);
                    }
                } else {
                    myTexture = Resources.Load<Texture2D>(url);
                    if (myTexture) {
                        cached.texture = myTexture;
                        cached.state.OnNext (2);

                    } else {
                        Debug.LogWarning ($"cxCachedImageLoader local load failed: {url}");
                        cached.state.OnNext (3);
                    }
                }
            }
        }

        await cached.WaitForLoad ();

        return cached;
    }

    CachedImage Find (string url) {
        lock (cachedImages) {
            if (cachedImages.TryGetValue (url.ToLower (), out CachedImage image)) {
                return image;
            } else {
                return null;
            }
        }
    }

    // CachedImage Save (string url, Texture2D image, Sprite sprite) {
    //     if (cachedImages.TryGetValue (url.ToLower (), out CachedImage c)) {
    //         Debug.Log ($"cxCachedImageLoader Save warning {url} already cached");
    //         return c;
    //     }

    //     CachedImage cachedImage = new CachedImage () {
    //         path = url.ToLower (),
    //         texture = image,
    //         sprite = sprite
    //     };

    //     cachedImages.Add (cachedImage.path, cachedImage);
    //     return cachedImage;
    // }

}