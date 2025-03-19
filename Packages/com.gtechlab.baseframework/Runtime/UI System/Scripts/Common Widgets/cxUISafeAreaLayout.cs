using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxUISafeAreaLayout : MonoBehaviour
{
    [Obsolete("Use flags instead")]
    public enum StrechType
    {
        Undef,
        Header,
        BodyTop,
        BodyBottom,
        Bottom,   
        BodyTopBottom
    }

    public const int IPHONE_X_HEADER_MARGIN = 70;
    public const int IPHONE_X_BOTTOM_MARGIN = 70;

    public bool top = false;
    public bool bottom = false;
    public bool right = false;
    public bool left = false;   

   // [Obsolete("Use flags instead")]
    //public StrechType strechType = StrechType.Undef;


    // Start is called before the first frame update
    void Start()
    {
        if(IsIphoneX())
            Resize();
    }

    public bool IsIphoneX()
	{
        bool IsIphone = false;

#if UNITY_IPHONE
        IsIphone = true;
#endif

        if (IsIphone) //Application.platform == RuntimePlatform.IPhonePlayer)
        {
            float screenR = (float)Screen.width / (float)Screen.height;
            float iphone8R = 1080.0f / 1920.0f;
            bool isX = (int)(screenR * 100) < (int)(iphone8R * 100);
            return isX;
        } 
        else
        {
            return false;
        }
	}

    /*Left*/    //rectTransform.offsetMin.x;
    /*Right*/   //rectTransform.offsetMax.x;
    /*Top*/     //rectTransform.offsetMax.y;
    /*Bottom*/  //rectTransform.offsetMin.y;

    void Resize()
    {
        var rectTransform = GetComponent<RectTransform>();

        Vector2 size = rectTransform.sizeDelta;
        Vector2 max = rectTransform.offsetMax;
        Vector2 min = rectTransform.offsetMin;

        if(top) {
            max.y -= IPHONE_X_HEADER_MARGIN;
        }

        if(bottom) {
            min.y += IPHONE_X_BOTTOM_MARGIN;
        }

        if(right) {
            max.x -= IPHONE_X_HEADER_MARGIN;
        }

        if(left){
            min.x += IPHONE_X_BOTTOM_MARGIN;
        }
        
        rectTransform.offsetMin = min;
        rectTransform.offsetMax = max;
    }


/*
    void Resize()
    {
      

        var rectTransform = GetComponent<RectTransform>();
        float left_pixel = rectTransform.offsetMin.x;
        float right_pixel = rectTransform.offsetMax.x;
        float top_pixel = rectTransform.offsetMax.y;
        float bottom_pixel = rectTransform.offsetMin.y;
        
        Vector2 size = rectTransform.sizeDelta;


        switch(strechType)
        {
            case StrechType.Header:
            {
                size.y +=IPHONE_X_HEADER_MARGIN;
                rectTransform.sizeDelta = size;
            }
            break;
            case StrechType.Bottom:
            {
                size.y +=IPHONE_X_BOTTOM_MARGIN;
                rectTransform.sizeDelta = size;
            }
            break;
            case StrechType.BodyTop:
            {
                top_pixel -= IPHONE_X_HEADER_MARGIN;
                rectTransform.offsetMin = new Vector2(left_pixel, bottom_pixel);
                rectTransform.offsetMax = new Vector2(right_pixel, top_pixel);
            }
            break;
            case StrechType.BodyBottom:
            {
                bottom_pixel += IPHONE_X_BOTTOM_MARGIN;
                rectTransform.offsetMin = new Vector2(left_pixel, bottom_pixel);
                rectTransform.offsetMax = new Vector2(right_pixel, top_pixel);
            }
            break;
             case StrechType.BodyTopBottom:
            {
                top_pixel -= IPHONE_X_HEADER_MARGIN;
                bottom_pixel += IPHONE_X_BOTTOM_MARGIN;
                rectTransform.offsetMin = new Vector2(left_pixel, bottom_pixel);
                rectTransform.offsetMax = new Vector2(right_pixel, top_pixel);
            }
            break;
        }
    }
*/
}
