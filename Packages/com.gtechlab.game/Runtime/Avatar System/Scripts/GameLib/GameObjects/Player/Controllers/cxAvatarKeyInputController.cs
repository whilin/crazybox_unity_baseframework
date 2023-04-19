using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxAvatarKeyInputController : MonoBehaviour {
    // public cxAvatarStateOutput stateOutput { get; private set; } = new cxAvatarStateOutput ();    private cxAbstractPlayerCommand playerCommand;
    public cxAvatarStateInput stateInput { get; private set; } = new cxAvatarStateInput ();

    public bool enableClickNMove= true;
    public bool enableDirectMove = true;
    public float rotateKeySensitive = 10.0f;
    private cxAbstractPlayerCommand playerCommand;
    private cxAvatarLocalStateController localStateController;

    private KeyCode lastKeyCode;
    private float lastKeyInputTime;
    private bool doubleTapState;

    private void Awake () {
        playerCommand = GetComponent<cxAbstractPlayerCommand> ();
        localStateController = GetComponent<cxAvatarLocalStateController> ();
    }

    public void HandleInput () {
        if (cxAbstractSceneController.Instance.hasCharacterMovingControl)
            HandleUserInput ();

        HandleMouseInput ();
        // if(stateInput.moveTo) {
        //         stateMachine.SendMessage ((int) MsgID.MoveTo, keyInputController.stateInput.moveToPos, 0);
        // }
    }

    bool CheckDoubleTap (KeyCode input) {

        if (lastKeyCode != input) {
            if (Input.GetKeyDown (input)) {
                lastKeyCode = input;
                lastKeyInputTime = Time.time;
                doubleTapState = false;
            }
            return false;
        }

        if (Input.GetKeyDown (input)) {
            if ((Time.time - lastKeyInputTime) < 0.3f) {
                doubleTapState = true;
                return true;
            } else {
                lastKeyCode = input;
                lastKeyInputTime = Time.time;
                doubleTapState = false;
                return false;
            }
        } else if (Input.GetKey (input)) {
            return doubleTapState;
        } else
            return false;
    }

    private void HandleUserInput () {
        Vector2 move = Vector2.zero;
        Vector2 look = Vector2.zero;
        bool jump = false;
        bool run = false;

        if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
            move.y = 1;
            // Hold(false);
            //     run = CheckDoubleTap (KeyCode.W);
        }

        if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
            move.y = -1;
            // Hold(false);
            //    run = CheckDoubleTap (KeyCode.S);
        }

        if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
            move.x = -1;
            // Hold(false);
        }
        if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
            move.x = 1;
            // Hold(false);
        }

        //Note. 이종옥, 점프 기능 제거 (싱크 이슈)
        if (Input.GetKeyDown (KeyCode.Space)) {
            jump = true;
            // Hold(false);
        }

        if(enableDirectMove) {
            stateInput.MoveInput (move);
            stateInput.SprintInput (run);
            stateInput.JumpInput (jump);
            stateInput.LookInput (look * rotateKeySensitive);
        }
    }

    private void HandleMouseInput () {
        Vector3? clickPoint = null;
        if (Input.GetMouseButtonDown (0)) {
            clickPoint = Input.mousePosition;
        }

        if (clickPoint.HasValue) {
            var physics = gameObject.scene.GetPhysicsScene ();
            var ray = Camera.main.ScreenPointToRay (clickPoint.Value);
            if (!cxUISystemUtil.IsMousePointerOnUI (0)) {
                if (physics.Raycast (ray.origin, ray.direction, out RaycastHit hitted, float.MaxValue)) {
                    int hitLayerMask = 1 << hitted.collider.gameObject.layer;

                    if ((hitLayerMask & localStateController.movingGroundMask.value) != 0) {
                       // stateInput.MoveToInput (hitted.point);
                       if(enableClickNMove)
                            localStateController.SetMoveToPosition(hitted.point);

                    } else if (hitLayerMask == 256) {
                        Debug.Log ("hitted : " + hitted.transform.gameObject.name);
                        var trigger = hitted.transform.gameObject.GetComponent<cxTrigger> ();
                        if (trigger)
                            playerCommand.ExecuteTouchCommand (trigger, hitted.point);
                    }
                }
            }
        }
    }
}