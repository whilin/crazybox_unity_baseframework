using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUINavigator : MonoSingleton<cxUINavigator> {
    [SerializeField]
    private Canvas rootCanvas;

    [SerializeField]
    private List<cxUIFrame> prefabs;

    private List<cxUIFrame> frames;

    protected override void Awake () {
        base.Awake ();

        frames = new List<cxUIFrame> ();
        foreach (var p in prefabs) {
            var frame = ScriptUtil.PrefabCreateUGUI<cxUIFrame> (p, rootCanvas.gameObject);
            frames.Add (frame);
        }
    }

    public T FindFrame<T> () where T : cxUIFrame {
        return cxUIFrame.FindFrame<T> ();
    }

    public cxUIFrame ReplacePushFrame<T> (object showParam = null) where T : cxUIFrame {
        var frame = frames.Find (q => q.GetType () == typeof (T));
        if (frame == null)
            throw new Exception ("ReplacePushFrame FrameType not found type:" + typeof (T));
        if (frame.IsDialog)
            throw new Exception ("ReplacePushFrame FrameType is Not Frame type:" + typeof (T));

        cxUIContext context = new cxUIContext (showParam);
        frame.Replace (context, showParam);
        return frame;
    }

    public cxUIFrame PushFrame<T> (object showParam = null) where T : cxUIFrame {
        var frame = frames.Find (q => q.GetType () == typeof (T));

        if (frame == null)
            throw new Exception ("PushFrame FrameType not found type:" + typeof (T));
        if (frame.IsDialog)
            throw new Exception ("PushFrame FrameType is Not Frame type:" + typeof (T));

        cxUIContext context = new cxUIContext (showParam);
        frame.Push (context, showParam);

        return frame;
    }

    public void PopFrame () {
        if (cxUIFrame.ActiveFrame)
            cxUIFrame.ActiveFrame.Pop ();
    }

    public void PopAllFrames(){
        while(cxUIFrame.ActiveFrame){
            cxUIFrame.ActiveFrame.Pop();
        }
    }

    //Note.
    //  1. 팝업은 단독으로 나타날 수 없으며, 프레임위에서만 보여진다.
    //  2. 팝업의 라이프사이클은 베이스프레임에 종속되어 있다.
    public cxUIFrame ShowDialog<T> (object showParam = null) where T : cxUIFrame {
        var frame = frames.Find (q => q.GetType () == typeof (T));

        if (frame == null)
            throw new Exception ("PushFrame FrameType not found type:" + typeof (T));
        if (!frame.IsDialog)
            throw new Exception ("PushFrame FrameType is Not Dialog type:" + typeof (T));

        cxUIContext context = new cxUIContext (showParam);
        frame.PushPopup (context, showParam);

        return frame;
    }

    public void PopDialog () {
        if (cxUIFrame.ActiveFrame?.ActivePopup)
            cxUIFrame.ActiveFrame?.ActivePopup.Pop ();
    }
}