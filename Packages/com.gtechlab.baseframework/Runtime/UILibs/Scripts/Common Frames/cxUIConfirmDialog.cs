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

        public string confirmText = "OK";
        public string cancelText = "Cancel";
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
            PopResult (false);
        }).AddTo (this);
    }

    protected override void OnActivated (object showParam) {
        var param = showParam as ShowParam;
        if (param != null) {
            m_widget.title.text = param.title;
            m_widget.message.text = param.message;
            m = param.method;

            m_widget.submitButton.GetComponentInChildren<Text> ().text = param.confirmText;

            if (!string.IsNullOrEmpty (param.cancelText)) {
                m_widget.cancelButton.gameObject.SetActive (true);
                m_widget.cancelButton.GetComponentInChildren<Text> ().text = param.cancelText;
            } else {
                m_widget.cancelButton.gameObject.SetActive (false);
            }
        }
    }

    protected override void OnDeactivated () { }
    protected override void OnGameStateMessage (int msg, object msgParam) { }

}