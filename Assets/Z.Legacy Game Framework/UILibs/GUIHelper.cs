using UnityEngine;
using System.Collections;

public class GUIHelper
{

    public static float padding = 32;
    public static float border = 8;

    public static Rect APIGroupRect()
    {
        float contentWidth = Screen.width - padding * 2;
        float contentHeight = Screen.height - padding * 2;

        Rect menuRect = MenuGroupRect();
        float drawWidth = contentWidth - menuRect.width;

        float x = menuRect.x + menuRect.width + padding;
        return new Rect(x, padding, drawWidth, contentHeight);
    }

    public static Rect APIContentRect()
    {
        float contentWidth = Screen.width - padding * 2;
        float contentHeight = Screen.height - padding * 2;

        Rect menuRect = MenuGroupRect();
        float drawWidth = contentWidth - menuRect.width - padding;

        return new Rect(0, padding, drawWidth, contentHeight);
    }

    public static Rect MenuGroupRect()
    {
        float contentWidth = Screen.width - padding * 2;
        float contentHeight = Screen.height - padding * 2;
        float drawWidth = (contentWidth - border * 3) / 4;

        float x = padding;
        return new Rect(x, padding, drawWidth, contentHeight);
    }

    public static Rect MenuContentRect()
    {
        float contentWidth = Screen.width - padding * 2;
        float contentHeight = Screen.height - padding * 2;
        float drawWidth = (contentWidth - border * 3) / 4;
        return new Rect(0, padding, drawWidth, contentHeight);
    }

    public static GUIStyle LoadingStyle()
    {
        GUIStyle loadingStyle = new GUIStyle();
        loadingStyle.alignment = TextAnchor.MiddleCenter;
        loadingStyle.fontStyle = FontStyle.Bold;
        loadingStyle.normal.textColor = Color.white;
        return loadingStyle;
    }

    public static GUIStyle FormStyle()
    {
        GUIStyle formStyle = new GUIStyle();
        formStyle.alignment = TextAnchor.MiddleCenter;
        formStyle.normal.textColor = Color.white;
        return formStyle;
    }
}
