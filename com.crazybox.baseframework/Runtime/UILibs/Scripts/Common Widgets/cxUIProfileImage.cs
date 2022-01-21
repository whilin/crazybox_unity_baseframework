using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Networking;

[RequireComponent(typeof(Mask))]
public class cxUIProfileImage : MonoBehaviour
{
    [SerializeField]
    private RawImage photoImage;
    [SerializeField]
    private string url;

    void Awake()
    {

    }
    
    private void Start()
    {
        if (!string.IsNullOrEmpty(url))
            Load(url);
        else
            SetEmpty();
    }

    public void Load(string url)
    {
        this.url = url;
        SetEmpty();

        if (!string.IsNullOrEmpty(url))
            UnityWebRequestTexture.GetTexture(url)
                .SendWebRequest()
                .AsObservable()
                .Subscribe(op =>
                {
                    UnityWebRequestAsyncOperation op2 = (UnityWebRequestAsyncOperation)op;

                    if (op2.isDone && op2.webRequest.result == UnityWebRequest.Result.Success)
                    {
                        Texture myTexture = ((DownloadHandlerTexture)op2.webRequest.downloadHandler).texture;
                        photoImage.color = Color.white;
                        photoImage.texture = myTexture;
                    }

                }).AddTo(this);
    }

    void SetEmpty()
    {
        photoImage.color = Color.gray;
        photoImage.texture = null;
    }
}
