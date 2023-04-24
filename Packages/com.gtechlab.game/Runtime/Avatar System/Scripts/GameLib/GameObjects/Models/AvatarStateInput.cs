using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class cxAvatarStateInput {
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    public bool crying;
    public bool handRaising;

    public bool sit;
    public Vector3 sitPos;
    public Quaternion sitRotation;
    public bool stand;

    public Vector3? clickGround;
    public Vector3 clickPoint;
    public cxTrigger clickTrigger;

    public void MoveInput (Vector2 newMoveDirection) {
        move = newMoveDirection;
    }

    public void LookInput (Vector2 newLookDirection) {
        look = newLookDirection;
    }

    public void JumpInput (bool newJumpState) {
        jump = newJumpState;
    }

    public void SprintInput (bool newSprintState) {
        sprint = newSprintState;
    }

    public bool hasMoveInput () {
        return move.magnitude > 0.01f || jump;
    }

    public void SitOn (Vector3 pos, Quaternion rot) {
        sit = true;
        sitPos = pos;
        sitRotation = rot;
    }

    public void ConsumedActionInput () {
        sit = false;
        crying = false;
        handRaising = false;
        stand = false;
        move = Vector2.zero;
        jump = false;
        clickGround = null;
        clickTrigger = null;
        //moveTo = false;
    }

    public void ConsumedLookInput () {
        look = Vector2.zero;
    }
}
