using UnityEngine;
using System.Collections;

public class AdjustCameraAspect : MonoBehaviour {

    public const float minAspect = 16.0f / 9.0f;
    
    public void ApplyQualitySettingInLevel()
    {
        var res = Screen.currentResolution;
        float myAspect = res.width / res.height;

        res.width = 1024;
        res.height = 768;

        if (myAspect < minAspect)
        {
            int newH = (int)(res.width / minAspect);
            Rect newRect = new Rect();
            newRect.width = 1;
            newRect.x = 0;
            newRect.height = ((float)newH / (float)res.height);
            newRect.y = (1.0f - newRect.height) * 0.5f;

            GetComponent<Camera>().rect = newRect;

            /*
            foreach (Camera cam in Camera.allCameras)
            {
                Rect newRect = new Rect();
                newRect.width = 1;
                newRect.x = 0;
                newRect.height = ((float)newH / (float)res.height);
                newRect.y = (1.0f - newRect.height) * 0.5f;

                camera.rect = newRect;
            }
             */
        }
    }


	// Use this for initialization
	void Start () 
    {
        ApplyQualitySettingInLevel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
