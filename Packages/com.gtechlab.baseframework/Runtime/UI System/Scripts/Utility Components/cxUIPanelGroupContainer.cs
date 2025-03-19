using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class cxUIPanelGroupContainer : MonoBehaviour
{
    public GameObject defaultWidget;
    public List<cxUITweenPanel> panels;

    cxUITweenPanel activeToolFrame;

    Coroutine coCloseMonitor = null;

    private void Awake() {
        Init();
    }
    
    cxUITweenPanel GetActiveAuxFrame () {
        if (activeToolFrame != null && activeToolFrame.gameObject.activeSelf)
            return activeToolFrame;
        else
            return null;
    }

    public void SetDefaultWidget (GameObject defaultWidget) {
        this.defaultWidget = defaultWidget;
    }

    void SetActiveAuxFrame (cxUITweenPanel activeFrame) {
        activeToolFrame = activeFrame;
        activeToolFrame.Play();
        defaultWidget?.SetActive (false);

        if(coCloseMonitor !=null)
            StopCoroutine(coCloseMonitor);
        coCloseMonitor =  StartCoroutine(CloseMontior());
    }

    public void ClosePanel () {
        if (activeToolFrame != null && activeToolFrame.gameObject.activeSelf)
            activeToolFrame?.Hide ();

        activeToolFrame = null;
        defaultWidget?.SetActive (true);
    }

    public void ShowPanel (cxUITweenPanel panel)  {
        ClosePanel ();
        SetActiveAuxFrame (panel);    
    }

    void Init(){
        foreach(var panel in panels) {
            panel.Hide(true);
        }
    }
    
    IEnumerator CloseMontior(){
        yield return new WaitUntil(() => !activeToolFrame.gameObject.activeSelf);
         activeToolFrame = null;
        defaultWidget?.SetActive (true);
    }
}
