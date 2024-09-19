using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class cxUITextInputListEvent : UnityEvent<List<string>> { }

public class cxUITextInputListWidget : cxUIInputWidget {
    [SerializeField]
    private List<TMPro.TMP_InputField> textInputs;

    public cxUITextInputListEvent OnInputEvent { get; private set; } = new cxUITextInputListEvent ();

    public List<string> Value => GetInputs ();

    private void Awake () {
        foreach (var input in textInputs) {
            input.onValueChanged.AddListener ((text) => {
                OnInputEvent.Invoke (GetInputs ());
            });
        }

    }

    public void Set(List<string> inputs, bool silent = false) {
        if(silent) {
           for(int i= 0 ; i < inputs.Count && i < textInputs.Count ; i++) {
            textInputs[i].SetTextWithoutNotify(inputs[i]);
           }
        }
        else {
            for(int i= 0 ; i < inputs.Count && i < textInputs.Count ; i++) {
             textInputs[i].text = inputs[i];
           }
        }
    }

    List<string> GetInputs () {
        List<string> values = new List<string> ();
        foreach (var input in textInputs) {
            values.Add (input.text);
        }

        return values;
    }
}