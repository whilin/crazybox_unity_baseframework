using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public sealed class cxAvatarLocalStateController : MonoBehaviour {

    public Transform playerCameraRoot;
    public Animator animator;
    public float rotateSpeed = 10.0f;

    public LayerMask movingGroundMask;
    public Collider playerBoundingBox;

    private bool isLocalPlayer = false;
    private bool isAnimatorPlay = false;

    SkinnedMeshRenderer[] myMeshes;

    public cxAvatarStateOutput stateOutput { get; private set; } = new cxAvatarStateOutput ();
    public cxAvatarStateInput stateInput { get; private set; } = new cxAvatarStateInput ();

    private cxAvatarNaviAgentMoveController naviAgentMoveController;
    private cxAvatarDirectMoveController directMoveController;
    private cxAbstractAvatarInputController keyInputController;
    private cxAbstractPlayerObject playerObject;

    CompositeDisposable disposables = new CompositeDisposable ();

    private void Awake () {
        naviAgentMoveController = GetComponent<cxAvatarNaviAgentMoveController> ();
        directMoveController = GetComponent<cxAvatarDirectMoveController> ();
        playerObject = GetComponent<cxAbstractPlayerObject> ();
        keyInputController = GetComponent<cxAbstractAvatarInputController> ();

        animator = GetComponent<Animator> ();

        isAnimatorPlay = false;
        isLocalPlayer = false;
        naviAgentMoveController.enabled = false;
        directMoveController.enabled = false;
    }

    void Start () {
        // if (isSandbox)
        //     StartLocalPlayer ();
    }

    void OnDestroy () {
        disposables.Clear ();
    }

    private void OnEnable () {
        if (isLocalPlayer) {
            // InitAnimatorObserver ();
        }
    }

    public void StartLocalPlayer () {

        isLocalPlayer = true;
        naviAgentMoveController.enabled = true;
        directMoveController.enabled = true;

        InitStateMachine ();
        InitAnimatorObserver ();
    }

    public void RebuildAnimator () {
        InitAnimatorObserver ();
    }

    public void Hold (bool hold) {
        GetComponent<CharacterController> ().enabled = !hold;
    }

    //TODO: Adhoc... 정상적인 상태 관리속으로 들어가야함!!!
    /*
    public async void WarpTo(Vector3 position, Quaternion rotation) {
        directMoveController.enabled = false;
        naviAgentMoveController.enabled = false;
        naviAgentMoveController.ResetNavMeshAgent ();

        await new WaitForSeconds (0.2f);

        transform.position = position;
        transform.rotation = rotation;

        stateMachine.SetState((int) StateID.Default);

        directMoveController.enabled = true;
        naviAgentMoveController.enabled = false;


         var playerCamera = cxAbstractSceneController.Instance.GetPlayerCamera ();
         playerCamera.ResetCamera();
        
                    // var trigger = (Transform) PARAM ();
                    // StartCoroutine (WaitSetPosition (trigger));
                    // stateMachine.SetTimeout (0.5f);
    }
*/
    public async void WarpTo (Transform warp) {
        stateMachine.SendMessage ((int) MsgID.WarpTo, warp, 0);
    }

    public bool CanSitCommand () {
        StateID curState = (StateID) stateMachine.GetCurState ();
        return (curState == StateID.Default || curState == StateID.MoveTo || curState == StateID.PlayEmotion);
    }

    private void OnTriggerEnter (Collider other) {
        if (!isLocalPlayer)
            return;

        if (other.TryGetComponent<cxTrigger> (out cxTrigger trigger)) {
            if (CanTrigger (trigger)) {
                if (trigger.triggerType == cxTriggerType.Warp) {
                    stateMachine.SendMessage ((int) MsgID.WarpTo, trigger.warpPoint, 0);
                } else {
                    playerObject.ExecuteTouchCommand (trigger);
                }
            }
        }
    }

    private void OnTriggerExit (Collider other) {
        if (!isLocalPlayer)
            return;
    }

    private void Update () {
        if (isLocalPlayer) {
            stateMachine?.UpdateStateMachine ();
        }
    }

    private void LateUpdate () {
        CheckCameraInside ();
    }

    bool cameraInner = false;

    void CheckCameraInside () {
        var camPos = Camera.main.transform.position;
        bool inner = playerBoundingBox.bounds.Contains (camPos);

        if (inner != cameraInner) {
            if (myMeshes == null) {
                myMeshes = GetComponentsInChildren<SkinnedMeshRenderer> ();
            }

            if (myMeshes != null) {
                foreach (var m in myMeshes) {
                    m.enabled = !inner;
                }
            }

            cameraInner = inner;
        }
    }

    public void SetMoveToPosition (Vector3 _point) {
        stateMachine.SendMessage ((int) MsgID.MoveTo, _point, 0);
    }

    public bool CanTrigger (cxTrigger trigger) {
        if (trigger.triggerType == cxTriggerType.SitOn) {
            return IsState (StateID.Default);
        } else {
            return true;
        }
    }

    public void PlayEmotion (EmotionCode emoId) {
        stateMachine.SendMessage ((int) MsgID.PlayEmotion, emoId, 0);
    }

    public void PlayLookAt (cxTrigger trigger) {
        stateMachine.SendMessage ((int) MsgID.LookAt, trigger, 0);
    }

    public void PlaySitOn (cxTrigger trigger) {
        stateMachine.SendMessage ((int) MsgID.SitOn, trigger, 0);
    }

    private bool IsState (StateID stateID) {
        return stateMachine.GetCurState () == (int) stateID;
    }

    #region State Machine Core

    private enum StateID {
        Global = 0,
        Default = 1,
        //LookAt,
        SitOn,
        PlayEmotion,
        MoveForSitOn,
        MoveTo,
        WarpTo,
        Jump,
    }

    private enum MsgID {
        Undef,
        LookAt,
        SitOn,
        Move,
        PlayEmotion,
        MoveTo,
        ArrivedTo,
        EndOnceMotion,
        WarpTo,
        Landed
    }

    private StateMachine stateMachine;

    readonly int _animStateStandToSit = Animator.StringToHash ("StandToSit");
    readonly int _animStateSitting = Animator.StringToHash ("Sitting");
    readonly int _animStateCrying = Animator.StringToHash ("Crying");
    readonly int _animStateHandRaising = Animator.StringToHash ("HandRaising");
    readonly int _animStateIdleWalking = Animator.StringToHash ("Idle Walk Run Blend");
    readonly int _amimStateJump = Animator.StringToHash ("JumpStart");
    readonly int _amimStateInAir = Animator.StringToHash ("InAir");
    readonly int _amimStateJumpLand = Animator.StringToHash ("JumpLand");

    readonly int _animStateAngry = Animator.StringToHash ("Angry");
    readonly int _animStateHappy = Animator.StringToHash ("Happy");
    readonly int _animStateFrustration = Animator.StringToHash ("Frustration");
    readonly int _animStateLaugh = Animator.StringToHash ("Laugh");
    readonly int _animStateClap = Animator.StringToHash ("Clap");
    readonly int _animStateGreeting = Animator.StringToHash ("Greeting");
    readonly int _animStateSupriesed = Animator.StringToHash ("Supriesed");

    //Trigger ID
    readonly int _animIDSpeed = Animator.StringToHash ("Speed");
    int _animIDGrounded = Animator.StringToHash ("Grounded");
    int _animIDJump = Animator.StringToHash ("Jump");
    int _animIDFreeFall = Animator.StringToHash ("FreeFall");
    int _animIDMotionSpeed = Animator.StringToHash ("MotionSpeed");

    int _animIDSit = Animator.StringToHash ("Sit");
    int _animIDSitTrigger = Animator.StringToHash ("SitTrigger");
    int _animIDCrying = Animator.StringToHash ("Crying");
    int _animIDHandRaising = Animator.StringToHash ("HandRaising");

    int _animIDAngry = Animator.StringToHash ("Angry");
    int _animIDHappy = Animator.StringToHash ("Happy");
    int _animIDFrustration = Animator.StringToHash ("Frustration");
    int _animIDLaugh = Animator.StringToHash ("Laugh");
    int _animIDClap = Animator.StringToHash ("Clap");
    int _animIDGreeting = Animator.StringToHash ("Greeting");
    int _animIDSupriesed = Animator.StringToHash ("Supriesed");

    void InitAnimatorObserver () {
        disposables.Clear ();
        //Ad-Hoc,  Awake 타임에서의 ObservableStateMachineTrigger 와,
        //           다음 프레임의 ObservableStateMachineTrigger 다른다.
        // 파츠 교체시 Animator.Rebind()의 문제였음
        // await new WaitForUpdate();

        var _stateMachineObservables = animator.GetBehaviour<ObservableStateMachineTrigger> ();

        // int _animStateStandToSit = Animator.StringToHash ("StandToSit");
        // int _animStateSitting = Animator.StringToHash ("Sitting");
        // int _animStateCrying = Animator.StringToHash ("Crying");
        // int _animStateHandRaising = Animator.StringToHash ("HandRaising");
        // int _animStateIdleWalking = Animator.StringToHash ("Idle Walk Run Blend");
        // int _amimStateJump = Animator.StringToHash ("JumpStart");
        // int _amimStateInAir = Animator.StringToHash ("InAir");
        // int _amimStateJumpLand = Animator.StringToHash ("JumpLand");

        // int _animStateAngry = Animator.StringToHash ("Angry");
        // int _animStateHappy = Animator.StringToHash ("Happy");
        // int _animStateFrustration = Animator.StringToHash ("Frustration");
        // int _animStateLaugh = Animator.StringToHash ("Laugh");
        // int _animStateClap = Animator.StringToHash ("Clap");
        // int _animStateGreeting = Animator.StringToHash ("Greeting");
        // int _animStateSupriesed = Animator.StringToHash ("Supriesed");

        // disposables.Clear ();

        _stateMachineObservables.OnStateExitAsObservable ()
            .Where (s => s.StateInfo.shortNameHash == _animStateCrying ||
                s.StateInfo.shortNameHash == _animStateHandRaising ||
                s.StateInfo.shortNameHash == _animStateAngry ||
                s.StateInfo.shortNameHash == _animStateHappy ||
                s.StateInfo.shortNameHash == _animStateFrustration ||
                s.StateInfo.shortNameHash == _animStateLaugh ||
                s.StateInfo.shortNameHash == _animStateClap ||
                s.StateInfo.shortNameHash == _animStateGreeting ||
                s.StateInfo.shortNameHash == _animStateSupriesed
            )
            .DoOnCompleted (() => {
                Debug.Log ("OnStateExitAsObservable DoOnCompleted");
            })
            .DoOnTerminate (() => {
                Debug.Log ("OnStateExitAsObservable DoOnTerminate");
            })
            .DoOnCancel (() => {
                Debug.Log ("OnStateExitAsObservable DoOnCancel");
            })
            .Subscribe (s => {
                Debug.Log ("_stateMachineObservables state exit:" + s.StateInfo.shortNameHash);
                stateMachine.SendMessage ((int) MsgID.EndOnceMotion, s.StateInfo.shortNameHash, 0);
                isAnimatorPlay = false;
            })
            .AddTo (disposables);

        _stateMachineObservables.OnStateExitAsObservable ()
            .Where (s => s.StateInfo.shortNameHash == _amimStateJumpLand)
            .Subscribe (s => {
                stateMachine.SendMessage ((int) MsgID.Landed, null, 0);
            })
            .AddTo (disposables);
    }

    void InitStateMachine () {

        //  var _stateMachineObservables = animator.GetBehaviour<ObservableStateMachineTrigger> ();
        bool _lookAtState = false;

        void _UpdateLookAtState () {
            if (_lookAtState) {
                if (stateInput.hasMoveInput ()) {
                    _ReleaseLookAtState ();
                }
            }
        }

        void _SetLookAtState (cxTrigger trigger) {
            _lookAtState = true;

            //TODO 컨셉 정리 필요
            // Camera lookAtCamera = trigger.lookAtCamera;
            // cameraController.LookAt (lookAtCamera);
            playerObject.ExecuteLookAt (playerObject, trigger);
        }

        void _ReleaseLookAtState () {
            _lookAtState = false;

            //TODO 컨셉 정리 필요
            //cameraController.Follow (transform);
            playerObject.ExecuteLookAtReleased (playerObject);
        }

        void _UpdateInput () {

            keyInputController.AcquireStateInput ();

            if (stateInput.clickGround.HasValue) {
                SetMoveToPosition (stateInput.clickGround.Value);
            }

            if (stateInput.clickTrigger != null) {
                playerObject.ExecuteTouchCommand (stateInput.clickTrigger, stateInput.clickPoint);
            }
        }

        void _UpdateAnimator () {
            animator.SetFloat (_animIDSpeed, stateOutput.speed);

            // if( stateOutput.jump)
            animator.SetBool (_animIDJump, stateOutput.jump);

            // if(stateOutput.freeFall)
            animator.SetBool (_animIDFreeFall, stateOutput.freeFall);

            animator.SetBool (_animIDGrounded, stateOutput.grounded);
        }

        stateMachine = new StateMachine ((int stateId, FSMMsg msg, int param1, object param2) => {

            bool MSG (MsgID m) => msg == FSMMsg.OnMsg && param1 == (int) m;
            bool ENTER () => msg == FSMMsg.OnEnter;
            bool EXIT () => msg == FSMMsg.OnExit;
            bool UPDATE () => msg == FSMMsg.OnUpdate;
            bool STATE (StateID state) => stateId == (int) state;
            object PARAM () => param2;
            void SET_STATE (StateID state, object param2 = null) => stateMachine.SetState ((int) state, param2);

            if (STATE (StateID.Default)) {
                if (ENTER ()) {
                    directMoveController.enabled = true;
                    naviAgentMoveController.enabled = false;
                //    playerObject.ExecuteSpawn (playerObject);
                } else if (EXIT ()) {

                } else if (UPDATE ()) {
                    _UpdateInput ();

                    _UpdateLookAtState ();
                    _UpdateAnimator ();

                    if (stateOutput.jump)
                        SET_STATE (StateID.Jump);

                } else if (MSG (MsgID.LookAt)) {
                    _SetLookAtState ((cxTrigger) PARAM ());
                    return true;
                } else if (MSG (MsgID.SitOn)) {
                    SET_STATE (StateID.SitOn, PARAM ());
                    return true;
                } else if (MSG (MsgID.PlayEmotion)) {
                    SET_STATE (StateID.PlayEmotion, PARAM ());
                    return true;
                } else if (MSG (MsgID.MoveTo)) {
                    SET_STATE (StateID.MoveTo, PARAM ());
                    return true;
                } else if (MSG (MsgID.WarpTo)) {
                    SET_STATE (StateID.WarpTo, PARAM ());
                    return true;
                }
            } else if (STATE (StateID.Jump)) {
                if (ENTER ()) {

                } else if (EXIT ()) {

                } else if (UPDATE ()) {

                    // if(stateOutput.freeFall && stateOutput.grounded)
                    //     SET_STATE(StateID.Default);

                    _UpdateAnimator ();
                } else if (MSG (MsgID.Landed)) {
                    SET_STATE (StateID.Default);
                }
            } else if (STATE (StateID.MoveTo)) {
                if (ENTER ()) {
                    directMoveController.enabled = false;
                    naviAgentMoveController.enabled = true;
                    if (!naviAgentMoveController.MoveTo ((Vector3) PARAM ())) {
                        SET_STATE (StateID.Default);
                    }
                } else if (EXIT ()) {
                    naviAgentMoveController.Stop ();
                } else if (UPDATE ()) {
                    if (naviAgentMoveController.IsStop) {
                        stateMachine.SendMessage ((int) MsgID.ArrivedTo, null, 0);
                    }

                    _UpdateInput ();

                    if (stateInput.hasMoveInput ()) {
                        stateMachine.SendMessage ((int) MsgID.ArrivedTo, null, 0);
                    }

                    _UpdateAnimator ();

                    if (stateOutput.jump)
                        SET_STATE (StateID.Jump);

                } else if (MSG (MsgID.ArrivedTo)) {
                    SET_STATE (StateID.Default);
                } else if (MSG (MsgID.MoveTo)) {
                    naviAgentMoveController.MoveTo ((Vector3) PARAM ());
                }
            } else if (STATE (StateID.WarpTo)) {
                if (ENTER ()) {
                    directMoveController.enabled = false;
                    naviAgentMoveController.enabled = false;

                    naviAgentMoveController.ResetNavMeshAgent ();
                    var trigger = (Transform) PARAM ();
                    StartCoroutine (WaitSetPosition (trigger));
                    stateMachine.SetTimeout (0.5f);
                } else if (EXIT ()) {

                } else if (UPDATE ()) {

                } else if (msg == FSMMsg.OnStateTimeout) {
                    SET_STATE (StateID.Default);
                }

            } else if (STATE (StateID.SitOn)) {
                if (ENTER ()) {
                    var trigger = (cxTrigger) PARAM ();
                    var charT = trigger.transform;

                    animator.SetBool (_animIDSit, true);
                    // networkAnimator.SetTrigger (_animIDSitTrigger);

                    directMoveController.enabled = false;
                    naviAgentMoveController.enabled = false;

                    StartCoroutine (WaitSetRotation (trigger));

                } else if (EXIT ()) {
                    animator.SetBool (_animIDSit, false);
                } else if (UPDATE ()) {

                    _UpdateInput ();
                    _UpdateLookAtState ();

                    if (stateInput.hasMoveInput ()) {
                        stateInput.ConsumedActionInput ();
                        stateInput.ConsumedLookInput ();
                        stateInput.stand = true;
                        animator.SetBool (_animIDSit, false);

                        SET_STATE (StateID.Default);
                    }

                } else if (MSG (MsgID.LookAt)) {
                    _SetLookAtState ((cxTrigger) PARAM ());
                    return true;
                } else if (MSG (MsgID.MoveTo)) {
                    SET_STATE (StateID.MoveTo, PARAM ());
                    return true;
                }
            } else if (STATE (StateID.PlayEmotion)) {
                if (ENTER ()) {
                    //Release LookAt
                    _ReleaseLookAtState ();

                    EmotionCode emoCode = (EmotionCode) PARAM ();
                    if (emoCode == EmotionCode.Angry)
                        animator.SetTrigger (_animIDAngry);
                    else if (emoCode == EmotionCode.Happy)
                        animator.SetTrigger (_animIDHappy);
                    else if (emoCode == EmotionCode.Frustration)
                        animator.SetTrigger (_animIDFrustration);
                    else if (emoCode == EmotionCode.Laugh)
                        animator.SetTrigger (_animIDLaugh);
                    else if (emoCode == EmotionCode.Clap)
                        animator.SetTrigger (_animIDClap);
                    else if (emoCode == EmotionCode.Greeting)
                        animator.SetTrigger (_animIDGreeting);
                    else if (emoCode == EmotionCode.Supriesed)
                        animator.SetTrigger (_animIDSupriesed);
                    else if (emoCode == EmotionCode.Busy)
                        animator.SetTrigger (_animIDHandRaising);
                    else
                        SET_STATE (StateID.Default);

                } else if (UPDATE ()) {

                    //TODO : Exit Time 어떻게 찾아야하지!!
                    // if (playerController.output.exitOnceMotion)
                    //     SET_STATE (StateID.Default);
                } else if (MSG (MsgID.EndOnceMotion)) {
                    SET_STATE (StateID.Default);
                }
            } else if (STATE (StateID.Global)) {
                if (UPDATE ()) {
                    // playerController.output.Consumed ();
                }
            }

            return false;
        });

        stateMachine.SetState ((int) StateID.Default);
        playerObject.ExecuteSpawn(playerObject);
    }

    IEnumerator WaitSetPosition (Transform point) {
        yield return new WaitForSeconds (0.2f);

        transform.rotation = point.rotation;
        transform.position = point.position;
        
         var playerCamera = cxAbstractSceneController.Instance.GetPlayerCamera ();
         playerCamera.ResetCamera();
    }

    IEnumerator WaitSetRotation (cxTrigger trigger) {
        yield return new WaitForSeconds (0.2f);
        var charT = trigger.transform;
        transform.rotation = charT.rotation;
        transform.position = charT.position;
    }
    #endregion
}