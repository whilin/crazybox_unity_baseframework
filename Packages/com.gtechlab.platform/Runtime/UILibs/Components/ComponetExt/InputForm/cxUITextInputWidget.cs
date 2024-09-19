using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class cxUITextInputEvent : UnityEvent<string> {}

public class cxUITextInputWidget : cxUIInputWidget
{
    [SerializeField]
    private TMPro.TMP_InputField textInput;

    public cxUITextInputEvent OnInputEvent {get;private set;} = new cxUITextInputEvent();

    public string Value => textInput.text;

    private void Awake() {
        textInput.onValueChanged.AddListener((text)=>{
            OnInputEvent.Invoke(text);
        });
    }

    public void Set(string text, bool silent = false) {
        if(silent)
            textInput.SetTextWithoutNotify(text);
        else
            textInput.text = text;
    }
}