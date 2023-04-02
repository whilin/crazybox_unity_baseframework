using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class cxUIAuxFrameContainer : MonoBehaviour
{
     public GameObject defaultWidget;
    cxUIFrame activeToolFrame;
    
    cxUIFrame GetActiveAuxFrame () {
        if (activeToolFrame != null && activeToolFrame.IsActive)
            return activeToolFrame;
        else
            return null;
    }

    public void SetDefaultWidget (GameObject defaultWidget) {
        this.defaultWidget = defaultWidget;
    }

    void SetActiveAuxFrame (cxUIFrame activeFrame) {
        activeToolFrame = activeFrame;
        defaultWidget?.SetActive (false);

        activeFrame.OnCloseOnceAsObservale.Subscribe ((result) => {
            activeToolFrame = null;
            defaultWidget?.SetActive (true);
        });
    }

    public void CloseAuxFrame () {
        if (activeToolFrame != null && activeToolFrame.IsActive)
            activeToolFrame?.Pop ();

        activeToolFrame = null;
        defaultWidget?.SetActive (true);
    }

    public void ShowAuxFrame<T> (object showParam = null) where T : cxUIFrame {

        if (!(GetActiveAuxFrame () is T)) {
            CloseAuxFrame ();
            var activeToolFrame = cxUINavigator.Instance.PushFrame<T> (showParam);
            SetActiveAuxFrame (activeToolFrame);
        }
    }
}
