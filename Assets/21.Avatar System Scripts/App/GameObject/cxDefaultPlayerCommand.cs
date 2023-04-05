using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxDefaultPlayerCommand : cxAbstractPlayerCommand {
    public override void ExecuteSpawn (cxAbstractPlayerObject playerController) {
        var camera = cxAbstractSceneController.Instance.GetPlayerCamera ();
        camera.Follow (playerController.playerCameraTarget);
    }

    public override void ExecuteLookAt (cxAbstractPlayerObject playerController, cxTrigger trigger) {
        Camera lookAtCamera = trigger.lookAtCamera;
        var camera = cxAbstractSceneController.Instance.GetPlayerCamera ();
        camera.LookAt (lookAtCamera);
        //      cxUINavigator.Instance.FindFrame<mcUILobbySceneV2Frame> ()?.SetActiveNameTag (false);
    }

    public override void ExecuteLookAtReleased (cxAbstractPlayerObject playerController) {
        var camera = cxAbstractSceneController.Instance.GetPlayerCamera ();
        //        cxUINavigator.Instance.FindFrame<mcUILobbySceneV2Frame> ()?.SetActiveNameTag (true);
        camera.Follow (playerController.playerCameraTarget);
    }

    public override void ExecuteTouchCommand (cxTrigger trigger) {
        //  var touchPoint = Camera.main.WorldToScreenPoint (trigger.nameTagPivot.position);
        //                     new TriggerTouchCommand (trigger, touchPoint).Execute ();
    }
    public override void ExecuteTouchCommand (cxTrigger trigger, Vector3 touchPoint) {
        //  var touchPoint = Camera.main.WorldToScreenPoint (trigger.nameTagPivot.position);
        //                     new TriggerTouchCommand (trigger, touchPoint).Execute ();
    }
}