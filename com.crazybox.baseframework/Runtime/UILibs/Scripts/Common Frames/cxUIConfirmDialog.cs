using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class cxUIConfirmDialog : cxUIFrame {
    public delegate void MethodCall (bool ok);
    public class ShowParam {
        public string title;
        public string message;
        public MethodCall method;
    }


    [System.Serializable]
    public class UIWidget {
        public Button submitButton;
        public Button cancelButton;
        public Text title;
        public Text message;
    }

    public UIWidget m_widget;
    public MethodCall m;

    protected override void OnInit () {
        m_widget.submitButton.OnClickAsObservable ().Subscribe (s => {
            m?.Invoke (true);
            PopResult (true);
        }).AddTo (this);

        m_widget.cancelButton.OnClickAsObservable ().Subscribe (s => {
            m?.Invoke (false);
            PopResult(false);
        }).AddTo (this);
    }

    protected override void OnActivated (object showParam) {
        var param = showParam as ShowParam;
        if (param != null)
        {
            m_widget.title.text = param.title;
            m_widget.message.text = param.message;
            m = param.method;
        }
    }

    protected override void OnDeactivated () { }
    protected override void OnGameStateMessage (int msg, object msgParam) { }

}