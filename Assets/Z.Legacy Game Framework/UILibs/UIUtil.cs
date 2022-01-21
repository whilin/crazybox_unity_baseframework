using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum AnchorType
{
	//위아래 - 왼쪽오른쪽
	
	TopLeft,
	TopRight,
	BottomLeft,
	BottomRight,
	
	TopCenter,
	BottonCenter,
	
	CenterLeft,
	CenterRight,
	
	Center
}

public class UIUtil
{
	public static Rect GetRect(AnchorType anchor, float rel_x, float rel_y, float rel_w, float rel_h)
	{
		int sx=Screen.width;
		int sy=Screen.height;
		
		float x, y, w, h;
		x=0;
		y=0;
		w = rel_w;
		h = rel_h;
		
		switch(anchor)
		{
		case AnchorType.TopLeft:
			x = rel_x;
			y = rel_y;
			break;
		case AnchorType.TopRight:
			x = (sx - rel_w) - rel_x;
			y = rel_y;
			break;
		case AnchorType.BottomLeft:
			x = rel_x;
			y = (sy - rel_h) - rel_y;
			break;
		case AnchorType.BottomRight:
			x = (sx - rel_w) - rel_x;
			y = (sy - rel_h) - rel_y;
			break;
		case AnchorType.TopCenter:
			x = (sx - rel_w) * 0.5f + rel_x;
			y = rel_y;
			break;
		case AnchorType.BottonCenter:
			x = (sx - rel_w) * 0.5f + rel_x;
			y = (sy - rel_h) - rel_y;
			break;
		case AnchorType.CenterLeft:
			x = rel_x;
			y = (sy - rel_h) * 0.5f + rel_y;
			break;
		case AnchorType.CenterRight:
			x = (sx - rel_w) - rel_x;
			y = (sy - rel_h) * 0.5f + rel_y;
			break;
		case AnchorType.Center:
			x = (sx - rel_w) * 0.5f + rel_x;
			y = (sy - rel_h) * 0.5f + rel_y;
			break;
		}
		
		return new Rect(x,y,w,h);
	}
	
	
	public static void DrawImpactEffect(TweenUtil trigger,Texture tex, float width, float height, AnchorType anchorType, float offsetx=0, float offsety=0)
	{		
		Rect texcoord=new Rect(0,0,1,1);
		Color color=Color.gray;
		Rect rect;
		
		float s;
		float a;
		
		s = trigger.FloatValue(TweenFunc.POW32_FAST, 0.75f, 1.0f);
		a = trigger.FloatValue(TweenFunc.POW_SLOW, 1.0f, 0.0f);
		
		color.a = a;
		rect=UIUtil.GetRect(anchorType, offsetx, offsety, width * s, height * s);
		Graphics.DrawTexture(rect, tex, texcoord,0,0,0,0, color);
		
		s = trigger.FloatValue(TweenFunc.POW_FAST, 1.0f, 1.5f);
		a = trigger.FloatValue(TweenFunc.POW_FAST, 1.0f, 0.0f);

		color.a = a;
		rect=UIUtil.GetRect(anchorType, offsetx, offsety, width * s, height * s);
		Graphics.DrawTexture(rect, tex, texcoord,0,0,0,0, color);
	}

    static GameObject cameraFadeObject;

	// public static void ApplyCameraFadeEffect(float time)
	// {
    //     /*
	// 	Hashtable args = new Hashtable();
	// 	args.Add("name", "CameraFade");
	// 	args.Add ("amount", 0.7f);
	// 	args.Add ("time", time);
	// 	args.Add ("easetype", iTween.EaseType.easeOutQuad);
        
	// 	iTween.CameraFadeAdd();
    //     iTween.CameraFadeFrom(args);
    //     */

    //     Hashtable args = new Hashtable();
    //     args.Add("name", "CameraFade");
    //     args.Add("amount", 0.0f);
    //     args.Add("time", time);
    //     args.Add("easetype", iTween.EaseType.easeOutQuad);

    //     var _cameraFadeObject = iTween.FadeFrom.CameraFadeAdd();
    //     if (_cameraFadeObject != null)
    //         cameraFadeObject = _cameraFadeObject;

    //     var gui = cameraFadeObject.GetComponent<Image>();
    //     gui.color = new Color(1,1,1, 0.7f);

    //     iTween.FadeTo(args);
    // }


    public static string WaitingDot()
    {
        int sec=(int) (Time.time * 2.0f);
        int n = sec % 3;

        if (n == 0)
            return ".";
        else if (n == 1)
            return "..";
        else if (n == 2)
            return "...";
        else
            return "";
    }
	
}
