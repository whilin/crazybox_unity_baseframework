using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class cxAbstractPlayerCommand : MonoBehaviour {

    public abstract void ExecuteSpawn (cxAbstractPlayerObject playerController);

    public virtual void ExecuteLookAt (cxAbstractPlayerObject playerController, cxTrigger trigger) { }
    public virtual void ExecuteLookAtReleased (cxAbstractPlayerObject playerController) { }
    public virtual void ExecuteTouchCommand (cxTrigger trigger) { }
    public virtual void ExecuteTouchCommand (cxTrigger trigger, Vector3 touchPoint) { }

}