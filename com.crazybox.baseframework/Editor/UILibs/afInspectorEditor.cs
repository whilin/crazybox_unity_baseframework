#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
public class afInspectorEditor : Editor
{
    int drawIndx = 0;
    Rect initRect;
    float nextX = 0;

    protected void InitRect(Rect rect)
    {
        rect.height += 30;
        initRect = rect;

        nextX = 0;
    }

    protected Rect NextRect(float w)
    {
        var r = new Rect(initRect.x + nextX, initRect.y, w, EditorGUIUtility.singleLineHeight);
        nextX += w + 5;
        return r;
    }

    protected string PopupStringList(string selected, string[] selList)
    {
        int sel = -1;
        for (int i = 0; i < selList.Length; i++)
        {
            if (selList[i] == selected)
            {
                sel = i;
                break;
            }
        }

        sel = EditorGUILayout.Popup(sel, selList, GUILayout.Width(120));

        return sel >= 0 ? selList[sel] : string.Empty;

    }
}



#endif
