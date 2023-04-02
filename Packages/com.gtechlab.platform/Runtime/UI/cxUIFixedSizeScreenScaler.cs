using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//실행 초기 스크린사이즈에 맞추어서, UI 스케일이 결정되도록 지원하는 컨포넌트
//Web의 UI 특성에 맞추기 위한 용도
[RequireComponent(typeof(CanvasScaler))]
public class cxUIFixedSizeScreenScaler : MonoBehaviour {

    public int referenceScreenX = 1920;
    public float minScale = 1.0f;
    public float maxScale = 2.0f;

    private CanvasScaler canvasScaler;

    int screenSizeX;
    int screenSizeY;

    private void Awake() {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    // Start is called before the first frame update
    void Start () {
        screenSizeX = Screen.width;
        screenSizeY = Screen.height;

        LogScreenSize ();
        SetupCanvasScale ();
    }

    // Update is called once per frame
    void Update () {
        if (Screen.width != screenSizeX || Screen.height != screenSizeY) {
            screenSizeX = Screen.width;
            screenSizeY = Screen.height;

            LogScreenSize ();
        }
    }

    void SetupCanvasScale () {
      //  float sizeX = Mathf.Clamp(screenSizeX, minScreenX, maxScreenX);
        float scale = (float) screenSizeX / (float) referenceScreenX;
        scale = Mathf.Clamp(scale,minScale, maxScale);
        canvasScaler.scaleFactor = scale;

        Debug.LogFormat (UnityEngine.LogType.Log, LogOption.NoStacktrace, null, "ScreenSize {0} x {1}, Reset CanvasScale  {2}", screenSizeX, screenSizeY, scale);

    }

    void LogScreenSize () {
        Debug.LogFormat (UnityEngine.LogType.Log, LogOption.NoStacktrace, null, "ScreenSize {0} x {1}", screenSizeX, screenSizeY);
    }
}