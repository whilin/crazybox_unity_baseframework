using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (Collider))]
public sealed class cxTrigger : MonoBehaviour {

    public cxTriggerType triggerType;
    public Transform nameTagPivot;
    public GameObject icon;

    [Header("Trigger Parameter")]
    public Camera lookAtCamera;
    public Transform sitOn;
    public string urlString;
    public Transform warpPoint;
    public string warpSceneURL;

    private void OnTriggerEnter (Collider other) {
    }

    private void OnTriggerExit (Collider other) {
    }

    private float timeCount = 0;
    private void Update()
    {
        if (icon != null)
        {
            timeCount += Time.deltaTime;
            if (timeCount > 1f)
            {
                timeCount = 0;
                icon.SetActive(IsPlayerInActiveRange());
            }
        }
    }

    bool IsPlayerInActiveRange(){
        var player = cxAbstractSceneController.Instance.GetPlayerObject();
        if(player) {
         return Vector3.Distance(player.transform.position, transform.position) < 5.0f;
        } else {
            return false;
        } 
    }
}