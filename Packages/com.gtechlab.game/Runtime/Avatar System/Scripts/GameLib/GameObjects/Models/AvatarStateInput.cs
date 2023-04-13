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

    // public bool moveTo;
    // public Vector3 moveToPos;

    public void MoveInput (Vector2 newMoveDirection) {
        move = newMoveDirection;
    }
    // public void MoveToInput (Vector2 moveToPos) {
    //     moveTo = true;
    //     this.moveToPos = moveToPos;
    // }

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

    public void Consumed () {
        sit = false;
        crying = false;
        handRaising = false;
        stand = false;
        move = Vector2.zero;
        jump = false;
        //moveTo = false;
    }

    public void Consumed2 () {
        look = Vector2.zero;
    }
}
