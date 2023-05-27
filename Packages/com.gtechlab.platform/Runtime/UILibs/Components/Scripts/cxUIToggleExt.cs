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
        GetComponent<Toggle> ()?.onValueChanged.AddListener ((on) => {
            Set (on);
        });
    }

    [ExecuteInEditMode()]
    public void SetIsOnWithoutNotify (bool value) {
        GetComponent<Toggle> ()?.SetIsOnWithoutNotify (value);
        Set (value);
    }

    [ExecuteInEditMode()]
    void Set (bool value) {
         if(this.on)  this.on?.SetActive (value);
        if(off) off?.SetActive (!value);
    }
}