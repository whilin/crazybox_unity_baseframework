using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class cxUITextInputEvent : UnityEvent<string> { }

[RequireComponent (typeof (cxUITouchDelegator))]
public class cxUITextInputWidget : cxUIInputWidget {
    [SerializeField]
    [Tooltip ("레이블(옵션)")]
    private TMPro.TMP_Text labelText;

    [SerializeField]
    private TMPro.TMP_InputField textInput;
    [SerializeField]
    private TMPro.TMP_Text valueText;
    [SerializeField]
    private cxUITouchDelegator touchDelegator;

    public cxUITextInputEvent OnInputEvent { get; private set; } = new cxUITextInputEvent ();

    public string Value => textInput.text;

    private void Awake () {

        textInput.onEndEdit.AddListener ((text) => {
            valueText.text = text;
            OnInputEvent.Invoke (text);
            textInput.gameObject.SetActive (false);
            valueText.gameObject.SetActive (true);
        });

        touchDelegator.onPointerDoubleClick.AddListener (() => {
            ChangeInputMode ();
        });
    }

    public void SetValue (string text, bool inputMode = false) {
        valueText.text = text;
        textInput.SetTextWithoutNotify (text);

        textInput.gameObject.SetActive (inputMode);
        valueText.gameObject.SetActive (!inputMode);
    }

    public void SetLabel (string title) {
        if (labelText != null)
            labelText.text = title;
    }

    public void SetInputHint (string hint) {
        var text = textInput.placeholder.GetComponent<TMPro.TMP_Text> ();
        if (text != null)
            text.text = hint;
    }

    void ChangeInputMode () {
        textInput.gameObject.SetActive (true);
        valueText.gameObject.SetActive (false);
        textInput.ActivateInputField ();
        textInput.Select ();
    }
}