using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class cxUISelectInputEvent : UnityEvent<int> {

}

public class cxUISelectInputWidget : cxUIInputWidget {
    public cxUISelectInputEvent OnInputEvent { get; private set; } = new cxUISelectInputEvent ();

    [SerializeField]
    private List<Toggle> toggles;

    public int Value => toggles.FindIndex(q => q.isOn);

    private void Awake () {
        foreach(var toggle in toggles) {
            toggle.onValueChanged.AddListener((on)=>{
                if(on) {
                    var idx = toggles.FindIndex(q => q == toggle);
                    OnInputEvent.Invoke(idx);
                }
            });
        }
    }

    public void Set(int idx, bool silent) {
        if(silent) {
            toggles[idx].SetIsOnWithoutNotify(true);
        } else {
            toggles[idx].isOn = true;
        }
    }


}