using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Toggle))]
[ExecuteInEditMode()]
public class cxUIToggleExt : MonoBehaviour {
    public GameObject off;
    public GameObject on;

    private Toggle toggle;

    public Toggle Toggle  {
        get{
            if(toggle == null)
                toggle = GetComponent<Toggle>();
            
            return toggle;
        }
    }
    
    [ExecuteInEditMode()]
    void Awake () {
       Toggle.onValueChanged.AddListener ((on) => {
            Set (on);
        });
    }

    [ExecuteInEditMode()]
    public void SetIsOnWithoutNotify (bool value) {
        Toggle.SetIsOnWithoutNotify (value);
        Set (value);
    }

    public void SetIsOnWithoutNotifyAndSendGroup (bool value) {
        Toggle.SetIsOnWithoutNotify (value);
        Set (value);

        //!!Ad-hoc 이걸 호출해주지 않으면, 그룹 토글 Off 처리가 안됨
        //!! 만일 false를 받아서 처리하는 리스너가 있다면 ... 무한 회전이 됨!!
        if(value)
            Toggle.group.NotifyToggleOn(Toggle, true);
    }

    [ExecuteInEditMode()]
    void Set (bool value) {
         if(this.on)  this.on?.SetActive (value);
        if(off) off?.SetActive (!value);
    }
}