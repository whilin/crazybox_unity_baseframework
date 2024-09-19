using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class cxUISearchInputEvent : UnityEvent<string> {

}

public class cxUISearchInputWidget : cxUIInputWidget
{
    public Action<cxUISearchInputWidget> OnExecuteSearch;

    [SerializeField]
    private TMPro.TMP_InputField textInput;

    [SerializeField]
    private Button searchButton;

    public cxUISearchInputEvent OnInputEvent {get;private set;} = new cxUISearchInputEvent();
    public string Value => textInput.text;


    private void Awake() {
        searchButton.onClick.AddListener(()=>{
            if(OnExecuteSearch !=null) {
                OnExecuteSearch(this);
            }
        });

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

    public void SetEnable(bool enable) {
        textInput.interactable = enable;
    }
}
