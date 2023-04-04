using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Canvas))]
[RequireComponent (typeof (GraphicRaycaster))]
public abstract partial class cxUIFrame : MonoBehaviour {

    static private List<cxUIFrame> s_activeFrames = new List<cxUIFrame> ();
    static private List<cxUIFrame> s_auxFrames = new List<cxUIFrame> ();
    static private int s_topFrameSortOrder = 0;

    public static cxUIFrame ActiveFrame {
        get {
            if (s_activeFrames.Count > 0)
                return s_activeFrames[s_activeFrames.Count - 1];
            else
                return null;
        }
    }

    public static void InitFrameStatus () {
        s_activeFrames.Clear ();
        s_auxFrames.Clear ();
        s_topFrameSortOrder = 0;
    }

    public static void CloseAllFrames () {
        foreach (var f in s_auxFrames)
            f.Close ();

        foreach (var f in s_activeFrames)
            f.Close ();

        s_activeFrames.Clear ();
        s_auxFrames.Clear ();
        s_topFrameSortOrder = 0;
    }

    public static void CloseFrames () {
        foreach (var f in s_activeFrames)
            f.Close ();

        s_activeFrames.Clear ();
        s_topFrameSortOrder = 0;
    }

    public static T FindFrame<T> () where T : cxUIFrame {
        cxUIFrame find;
        find = s_activeFrames.FindLast (q => q is T);
        if (!find)
            find = s_auxFrames.FindLast (q => q is T);

        return find as T;
    }

    static void IncTopFrameOrder () {

        if (ActiveFrame) {
            cxUIFrame top;

            if (ActiveFrame.popupFrames.Count > 0)
                top = ActiveFrame.popupFrames[ActiveFrame.popupFrames.Count - 1];
            else
                top = ActiveFrame;

            var canvas = top.GetComponent<Canvas> ();
            canvas.overrideSorting = true;
            canvas.sortingOrder = s_topFrameSortOrder + 10;
            s_topFrameSortOrder = canvas.sortingOrder;
        }
    }

    static void DecTopFrameOrder () {

        cxUIFrame top = null;
        if (ActiveFrame) {
            if (ActiveFrame.popupFrames.Count > 0)
                top = ActiveFrame.popupFrames[ActiveFrame.popupFrames.Count - 1];
            else
                top = ActiveFrame;
        }

        if (top) {
            s_topFrameSortOrder = top.GetComponent<Canvas> ().sortingOrder;
        } else {
            s_topFrameSortOrder = 0;
        }
    }

    private static void Navigate (cxUIFrame frame, bool stack) {
        if (frame.m_frameType == FrameType.Frame) {
            if (stack) {

                if (ActiveFrame) {
                    ActiveFrame.Hide (true);
                    ActiveFrame.OnBecomeInvisible ();
                }

                s_activeFrames.Add (frame);

            } else {
                if (ActiveFrame)
                    //ActiveFrame.Hide ();
                    ActiveFrame.Pop ();

                s_activeFrames.Add (frame);
            }
        } else if (frame.m_frameType == FrameType.Dialog) {
            if (ActiveFrame != null) {
                ActiveFrame.ActivePopup?.OnBecomeInvisible ();
                ActiveFrame.popupFrames.Add (frame);
            }
        } else if (frame.m_frameType == FrameType.Aux) {
            s_auxFrames.Add (frame);
        }

        IncTopFrameOrder ();
    }

    private static void PopNavigate (cxUIFrame frame) {
        if (frame.m_frameType == FrameType.Frame) {
            foreach (var d in frame.popupFrames)
                d.Hide ();

            s_activeFrames.Remove (frame);

            if (ActiveFrame) {
                ActiveFrame.Show (true);
                ActiveFrame.OnBecomeVisible ();
            }
        } else if (frame.m_frameType == FrameType.Dialog) {
            int last = ActiveFrame.popupFrames.Count - 1;
            if (last < 0 || ActiveFrame.popupFrames[last] != frame) {
                Debug.LogWarning ("Hide Popup is Not Top popup!");
            }

            if (!ActiveFrame.popupFrames.Find (q => q == frame)) {
                Debug.LogWarning ("Hide Popup is Not active popup!");
            }

            ActiveFrame.popupFrames.Remove (frame);
            ActiveFrame.ActivePopup?.OnBecomeVisible ();

        } else if (frame.m_frameType == FrameType.Aux) {
            if (!s_auxFrames.Find (q => q == frame)) {
                Debug.LogWarning ("Hide AuxFrame is Not active frame!");
            }

            s_auxFrames.Remove (frame);
        }

        DecTopFrameOrder ();
    }

    public static void HandleGameStateMessage (int msg, object msgParam) {
        if (msg == cxReservedGameMsg.OnBack) {
            if (ActiveFrame != null) {
                if (ActiveFrame.ActivePopup)
                    ActiveFrame.ActivePopup.OnGameStateMessage (msg, msgParam);
                else
                    ActiveFrame.OnGameStateMessage (msg, msgParam);
            }
        } else {
            for (int i = 0; i < s_auxFrames.Count; i++)
                s_auxFrames[i].OnGameStateMessage (msg, msgParam);

            if (ActiveFrame != null)
                ActiveFrame.OnGameStateMessage (msg, msgParam);
        }
    }

}