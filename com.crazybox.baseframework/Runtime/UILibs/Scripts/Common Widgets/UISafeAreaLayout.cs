using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISafeAreaLayout : MonoBehaviour
{
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

    //public int marginOffsetX =  10;
    public StrechType strechType = StrechType.Undef;


    // Start is called before the first frame update
    void Start()
    {
        if(IsIphoneX())
            Resize();
    }

    public bool IsIphoneX()
	{
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            float screenR = (float)Screen.width / (float)Screen.height;
            float iphone8R = 1080.0f / 1920.0f;
            bool isX = (int)(screenR * 100) < (int)(iphone8R * 100);
            return isX;
        } else
        {
            return false;
        }
	}

    void Resize()
    {
        /*Left*/    //rectTransform.offsetMin.x;
        /*Right*/   //rectTransform.offsetMax.x;
        /*Top*/     //rectTransform.offsetMax.y;
        /*Bottom*/  //rectTransform.offsetMin.y;

        var rectTransform = GetComponent<RectTransform>();
        float left = rectTransform.offsetMin.x;
        float right = rectTransform.offsetMax.x;
        float top = rectTransform.offsetMax.y;
        float bottom = rectTransform.offsetMin.y;
        
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
                top -= IPHONE_X_HEADER_MARGIN;
                rectTransform.offsetMin = new Vector2(left, bottom);
                rectTransform.offsetMax = new Vector2(right, top);
            }
            break;
            case StrechType.BodyBottom:
            {
                bottom += IPHONE_X_BOTTOM_MARGIN;
                rectTransform.offsetMin = new Vector2(left, bottom);
                rectTransform.offsetMax = new Vector2(right, top);
            }
            break;
             case StrechType.BodyTopBottom:
            {
                top -= IPHONE_X_HEADER_MARGIN;
                bottom += IPHONE_X_BOTTOM_MARGIN;
                rectTransform.offsetMin = new Vector2(left, bottom);
                rectTransform.offsetMax = new Vector2(right, top);
            }
            break;
        }
    }
}
