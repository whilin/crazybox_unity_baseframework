using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public abstract class cxAbstractSceneController : MonoSingleton<cxAbstractSceneController>
{
    [SerializeField]
    protected cxPlayerCameraController playerCamera;
    
    protected cxAbstractPlayerObject playerObject;

    public bool hasCharacterMovingControl { get; set; } = true;

    public cxPlayerCameraController GetPlayerCamera() { return playerCamera ;}

    public cxAbstractPlayerObject GetPlayerObject(){
        return playerObject;
    }
}
