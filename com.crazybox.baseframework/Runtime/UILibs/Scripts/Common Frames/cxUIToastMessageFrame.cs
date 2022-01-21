using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUIToastMessageFrame : cxUIFrame
{
    [SerializeField]
    private cxUITweenPanel panel;
    [SerializeField]
    private float hideTime = 3.0f;

    [SerializeField]
    private Text message;

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

    public void Toast(string message ){
        this.message.text = message;
        panel.Play();
        panel.ScheduledHide(hideTime);
    }

}
