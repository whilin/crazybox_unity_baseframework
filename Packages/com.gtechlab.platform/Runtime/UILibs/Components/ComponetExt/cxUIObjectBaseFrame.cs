using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Canvas))]
public class cxUIObjectBaseFrame : MonoBehaviour {

    public enum ViewMode {
        YAxisOnly,
        LookStraight
    }
    [Header("Base Frame Properties")]
    [SerializeField]
    private ViewMode viewMode = ViewMode.YAxisOnly;
    [SerializeField]
    private bool auto = true;
    [SerializeField]
    private cxUITweenPanel panel;
    
    private bool isOpen = false;

    private void Awake () {
        panel.Hide(true);
        isOpen = false;
        
        OnInit ();
        if(auto)
            OpenFrame();
    }

    private void OnEnable () {
        if(auto)
            OpenFrame ();
    }

    private void OnDisable () {
        CloseFrame ();
    }

    public void OpenFrame () {
        if (!isOpen) {
            panel?.Play ();
            OnActivated ();
            isOpen = true;
        }
    }

    public void CloseFrame () {
        if (isOpen) {
            panel?.Hide ();
            OnDeactivated ();
            isOpen = false;
        }
    }

    // Update is called once per frame
    virtual protected void Update () {
        if (viewMode == ViewMode.YAxisOnly) {
            var camPos = Camera.main.transform.position;
            camPos.y = transform.position.y;
            var dir = camPos - transform.position;

            transform.LookAt (transform.position - dir, Vector3.up);
        } else {
            var camPos = Camera.main.transform.position;
            transform.LookAt (camPos, Vector3.up);
        }
    }

    protected virtual void OnInit () {

    }

    protected virtual void OnActivated () { }

    protected virtual void OnDeactivated () { }
}