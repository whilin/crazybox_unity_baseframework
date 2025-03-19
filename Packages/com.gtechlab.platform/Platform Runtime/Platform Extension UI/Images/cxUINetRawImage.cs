using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(RawImage))]
public class cxUINetRawImage : MonoBehaviour {
   
    public string url;
    [Tooltip("로딩 전 , 로딩 실패시 설정되는 이미지")]
    public Texture defaultImg;
    [Tooltip("로딩 전 , 로딩 실패시, defaultImage 설정이 없을 경우 배경색")]
    public Color defaultColor = Color.gray;

    private RawImage image;
    private AspectRatioFitter aspectRatio;

    void Awake () {
        image = GetComponent<RawImage>();
        aspectRatio = GetComponent<AspectRatioFitter>();
    }

    private void Start () {
        if (!string.IsNullOrEmpty (url))
            Load (url);
        else
            SetEmpty ();
    }

    public async void Load (string url) {
        this.url = url;
        SetEmpty ();

        if (!string.IsNullOrEmpty (url)) {
            try {
                var loaded = await cxUniversalResourceLoader.Instance.LoadAsset<Texture2D>(url);
                if(loaded) {
                    image.texture = loaded;
                    aspectRatio.aspectRatio = (float) loaded.width / (float) loaded.height;
                }
            } catch(System.Exception ex) {
                Debug.LogWarning($"cxUINetRawImage({url}) load failed:"+ ex.Message);
            }
        }
    }

    void SetEmpty () {
        if (defaultImg) {
            image.color = Color.white;
            image.texture = defaultImg;
        } else {
            image.color = defaultColor;
            image.texture = null;
        }
    }
}