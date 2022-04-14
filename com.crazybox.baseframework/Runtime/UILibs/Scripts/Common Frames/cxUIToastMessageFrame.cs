using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUIToastMessageFrame : cxUIFrame
{
    public enum MsgType {
        Normal,
        Warning
    }

    [SerializeField]
    private cxUITweenPanel panel;
    [SerializeField]
    private Image background;

    [SerializeField]
    private float hideTime = 3.0f;

    [SerializeField]
    private Text message;

    [SerializeField]
    private Color normalMessageBg = Color.blue;
    [SerializeField]
    private Color warningMessageBg = Color.yellow;
    
    protected override void OnInit()
    {
        //cxUINavigator.Instance.PushFrame<cxUIToastMessageFrame>();
    }

    protected override void OnActivated(object showParam)
    {
        panel.Hide(true);
    }

    protected override void OnDeactivated()
    {
        
    }

    public void Toast(string message, MsgType msgType = MsgType.Normal ){
        this.message.text = message;
        this.background.color = msgType == MsgType.Normal ? normalMessageBg : warningMessageBg;
        panel.Play();
        panel.ScheduledHide(hideTime);
    }

}
