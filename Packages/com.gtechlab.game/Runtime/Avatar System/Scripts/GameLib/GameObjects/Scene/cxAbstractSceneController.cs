using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public abstract class cxAbstractSceneController : MonoSingleton<cxAbstractSceneController>
{
    [SerializeField]
    protected cxPlayerCameraController playerCamera;

    [Header("Debug View")]
    [SerializeField]
    protected cxAbstractPlayerObject playerObject;

    //public bool hasCharacterMovingControl { get; set; } = true;

    private GameObject focusObject = null;
    public cxPlayerCameraController GetPlayerCamera() { return playerCamera ;}
    public cxAbstractPlayerObject GetPlayerObject(){ return playerObject; }

    public void AcquireFocus(GameObject focus) { focusObject = focus;}
    public bool HasFocus(GameObject obj) { return focusObject == obj; }
    public bool FocusReleased() { return focusObject == null;}
}
